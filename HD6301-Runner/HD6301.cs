using System.Diagnostics;
using HD6301_Runner.Opcodes;
using Console = System.Console;

namespace HD6301_Runner
{
	public class HD6301
	{
		public event Action OnInstructionExecuted;

		public event Action<RegisterAddress> OnRegisterRead;
		public event Action<RegisterAddress> OnRegisterWrite;

		public byte[] Memory;
		public Ports Ports;
		public Interrupts Interrupts;
		public Timer1 Timer1;

		public float ExternalClockMHz;

		public ushort D;

		public byte A
		{
			get => (byte)((D & 0xFF00) >> 8);
			set
			{
				D = (ushort)(D & 0x00FF);
				D = (ushort)(D | ((value & 0x00FF) << 8));
			}
		}

		public byte B
		{
			get => (byte)(D & 0x00FF);
			set
			{
				D = (ushort)(D & 0xFF00);
				D = (ushort)(D | (value & 0x00FF));
			}
		}

		public ushort X; // Index Register
		public ushort SP; // Stack Pointer
		public UInt16 PC; // Program Counter
		public long Cycles;

		public byte CCR;

		public bool C
		{
			get => CCR.GetBit(0);
			set => CCR = CCR.SetBit(0, value);
		}

		public bool V
		{
			get => CCR.GetBit(1);
			set => CCR = CCR.SetBit(1, value);
		}

		public bool Z
		{
			get => CCR.GetBit(2);
			set => CCR = CCR.SetBit(2, value);
		}

		public bool N
		{
			get => CCR.GetBit(3);
			set => CCR = CCR.SetBit(3, value);
		}

		public bool _I
		{
			get => CCR.GetBit(4);
			set => CCR = CCR.SetBit(4, value);
		}

		public bool H
		{
			get => CCR.GetBit(5);
			set => CCR = CCR.SetBit(5, value);
		}

		public void InjectCode(int startIndex, byte[] asm)
		{
			for (var i = 0; i < asm.Length; i++)
			{
				Memory[startIndex + i] = asm[i];
			}
		}

		public void Push(byte b)
		{
			Memory[SP] = b;
			SP--;
		}

		public void Push(ushort s)
		{
			Push(s.GetLowByte());
			Push(s.GetHighByte());
		}
		
		public HD6301(byte[] romBytes, float externalClockMHz)
		{
			Ports = new(this);
			Timer1 = new(this);
			Interrupts = new(this);
			Memory = new byte[romBytes.Length];
			Array.Copy(romBytes, Memory, romBytes.Length);
			PC = 0xC000;

			Ports.Reset();

			ExternalClockMHz = externalClockMHz;
		}

		public void Start()
		{
			Ports.Reset();
			Task.Run(ExecutionLoop);
		}

		void ExecutionLoop()
		{
			var cpuFreq = (ExternalClockMHz * 1000000) / 4.0;
			var secondsPerCycle = 1.0 / cpuFreq;

			var stopwatch = Stopwatch.StartNew();
			var targetTime = 0.0;

			while (true)
			{
				var cyclesExecuted = 0;

				for (var i = 0; i < 1000; i++)
				{
					var beforeCycles = Cycles;
					Execute();
					OnInstructionExecuted?.Invoke();
					cyclesExecuted += (int)(Cycles - beforeCycles);
				}

				Timer1.IncrementCounter(cyclesExecuted);
				
				if (Interrupts.InterruptRequested)
				{
					var address = Interrupts.GetInterruptVector();
					Interrupt(address.msb, address.lsb);
				}

				targetTime += cyclesExecuted * secondsPerCycle;

				// long wait
				var remaining = targetTime - stopwatch.Elapsed.TotalSeconds;
				if (remaining > 0.002) // 2ms
				{
					Thread.Sleep((int)(remaining * 1000) - 1); // wait one less ms so we dont overshoot
				}

				// short wait
				while (stopwatch.Elapsed.TotalSeconds < targetTime)
				{
					Thread.SpinWait(10);
				}
			}
		}

		private void Interrupt(ushort msb, ushort lsb)
		{
			Memory[SP] = PC.GetLowByte();
			SP--;
			Memory[SP] = PC.GetHighByte();
			SP--;
			Memory[SP] = X.GetLowByte();
			SP--;
			Memory[SP] = X.GetHighByte();
			SP--;
			Memory[SP] = A;
			SP--;
			Memory[SP] = B;
			SP--;
			Memory[SP] = CCR;
			SP--;

			_I = true;

			var high = Memory[msb];
			var low = Memory[lsb];
			PC = CombineBytes(high, low);
		}

		public void RegisterRead(RegisterAddress register) => OnRegisterRead(register);
		public void RegisterWrite(RegisterAddress register) => OnRegisterWrite(register);
		
		private void Execute()
		{
			var cyclesBefore = Cycles;

			var opcode = Memory[PC++];

			switch (opcode)
			{
				case 0x01: // NOP
					Cycles += 1;
					break;
				case 0x08: // INX
					Cycles += 1;
					IndexRegisterControlOpcodes.INX(this);
					break;
				case 0x0A: // CLV
					Cycles += 1;
					BitControlOpcodes.CLV(this);
					break;
				case 0x0B: // SEV
					Cycles += 1;
					BitControlOpcodes.SEV(this);
					break;
				case 0x0C: // CLC
					Cycles += 1;
					BitControlOpcodes.CLC(this);
					break;
				case 0x0D: // SEC
					Cycles += 1;
					BitControlOpcodes.SEC(this);
					break;
				case 0x0E: // CLI
					Cycles += 1;
					BitControlOpcodes.CLI(this);
					break;
				case 0x0F: // SEI
					Cycles += 1;
					BitControlOpcodes.SEI(this);
					break;

				case 0x10: // SBA
					Cycles += 1;
					MathOpcodes.SBA(this);
					break;
				case 0x11: // CBA
					Cycles += 1;
					ComparisonAndTestOpcodes.CBA(this);
					break;
				case 0x16: // TAB
				{
					Cycles += 1;
					//Console.Write("TAB		");
					B = A;

					N = A.GetBit(7);
					Z = A == 0;
					V = false;
					break;
				}
				case 0x17: // TBA
				{
					Cycles += 1;
					//Console.Write("TBA		");
					A = B;

					N = B.GetBit(7);
					Z = B == 0;
					V = false;
					break;
				}
				case 0x18: // XGDX
					Cycles += 2;
					MathOpcodes.XGDX(this);
					break;
				case 0x1B: // ABA
					Cycles += 1;
					ArithmeticOperationOpcodes.ABA(this);
					break;

				case 0x20: // BRA
					Cycles += 3;
					BranchOpcodes.BRA(this);
					break;
				case 0x22: // BHI
					Cycles += 3;
					BranchOpcodes.BHI(this);
					break;
				case 0x23: // BLS
					Cycles += 3;
					BranchOpcodes.BLS(this);
					break;
				case 0x24: // BCC
					Cycles += 3;
					BranchOpcodes.BCC(this);
					break;
				case 0x25: // BCS
					Cycles += 3;
					BranchOpcodes.BCS(this);
					break;
				case 0x26: // BNE
					Cycles += 3;
					BranchOpcodes.BNE(this);
					break;
				case 0x27: // BEQ
					Cycles += 3;
					BranchOpcodes.BEQ(this);
					break;
				case 0x2A: // BPL
					Cycles += 3;
					BranchOpcodes.BPL(this);
					break;
				case 0x2B: // BMI
					Cycles += 3;
					BranchOpcodes.BMI(this);
					break;
				case 0x2C: // BGE
					Cycles += 3;
					BranchOpcodes.BGE(this);
					break;
				case 0x2D: // BLT
					Cycles += 3;
					BranchOpcodes.BLT(this);
					break;
				case 0x2E: // BGT
					Cycles += 3;
					BranchOpcodes.BGT(this);
					break;
				case 0x2F: // BLE
					Cycles += 3;
					BranchOpcodes.BLE(this);
					break;

				case 0x32: // PULA
					Cycles += 3;
					SP++;
					A = Memory[SP];
					break;
				case 0x33: // PULB
					Cycles += 3;
					SP++;
					B = Memory[SP];
					break;
				case 0x36: // PSHA
					Cycles += 4;
					Memory[SP] = A;
					SP--;
					break;
				case 0x37: // PSHB
					Cycles += 4;
					Memory[SP] = B;
					SP--;
					break;
				case 0x38: // PULX
					Cycles += 4;
					SP++;
					var high = Memory[SP];
					SP++;
					var low = Memory[SP];
					X = CombineBytes(high, low);
					break;
				case 0x39: // RTS
					Cycles += 5;
					FlowOpcodes.RTS(this);
					break;
				case 0x3A: // ABX
				{
					Cycles += 1;
					X = (ushort)(B + X);
					break;
				}
				case 0x3B: // RTI
					Cycles += 10;
					FlowOpcodes.RTI(this);
					break;
				case 0x3C: // PSHX
					Cycles += 5;
					Memory[SP] = X.GetLowByte();
					SP--;
					Memory[SP] = X.GetHighByte();
					SP--;
					break;
				case 0x3D: // MUL
					Cycles += 7;
					ArithmeticOperationOpcodes.MUL(this);
					break;

				case 0x43: // COMA
				{
					Cycles += 1;
					//Console.Write("COMA		");
					A = (byte)~A;
					N = A.GetBit(7);
					Z = A == 0;
					V = false;
					C = true;
					break;
				}
				case 0x44: // LSRA
					Cycles += 1;
					MathOpcodes.LSR(this, Accumulator.A);
					break;
				case 0x47: // ASRA
					Cycles += 1;
					MathOpcodes.ASR(this, Accumulator.A);
					break;
				case 0x48: // ASLA
				{
					Cycles += 1;
					//Console.Write("ASLA		");

					C = A.GetBit(7);
						
					A = (byte)(A << 1);

					N = A.GetBit(7);
					Z = A == 0;
					V = N ^ C;

					break;
				}
				case 0x4A: // DECA
					Cycles += 1;
					MathOpcodes.DEC(this, Accumulator.A);
					break;
				case 0x4C: // INCA
					Cycles += 1;
					MathOpcodes.INC(this, Accumulator.A);
					break;
				case 0x4D: // TSTA
					Cycles += 1;
					ComparisonAndTestOpcodes.TST(this, Accumulator.A);
					break;
				case 0x4F: // CLRA
					Cycles += 1;
					ArithmeticOperationOpcodes.CLR(this, Accumulator.A);
					break;

				case 0x53: // COMB
				{
					Cycles += 1;
					//Console.Write("COMB		");
					B = (byte)~B;
					N = B.GetBit(7);
					Z = B == 0;
					V = false;
					C = true;
					break;
				}
				case 0x54: // LSRB
					Cycles += 1;
					MathOpcodes.LSR(this, Accumulator.B);
					break;
				case 0x57: // ASRB
					Cycles += 1;
					MathOpcodes.ASR(this, Accumulator.B);
					break;
				case 0x58: // ASLB
				{
					Cycles += 1;
					//Console.Write("ASLB		");

					C = B.GetBit(7);

					B = (byte)(B << 1);

					N = B.GetBit(7);
					Z = B == 0;
					V = N ^ C;

					break;
				}
				case 0x5A: // DECB
					Cycles += 1;
					MathOpcodes.DEC(this, Accumulator.B);
					break;
				case 0x5C: // INCB
					Cycles += 1;
					MathOpcodes.INC(this, Accumulator.B);
					break;
				case 0x5D: // TSTB
					Cycles += 1;
					ComparisonAndTestOpcodes.TST(this, Accumulator.B);
					break;
				case 0x5F: // CLRB
					Cycles += 1;
					ArithmeticOperationOpcodes.CLR(this, Accumulator.B);
					break;

				case 0x61: // AIM Indexed
					Cycles += 7;
					MathOpcodes.AIM(this, GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x62: // OIM Indexed
					Cycles += 7;
					MathOpcodes.OIM(this, GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x64: // LSR Indexed
					Cycles += 6;
					MathOpcodes.LSR(this, GetIndexedAddress());
					break;
				case 0x65: // EIM Indexed
					Cycles += 7;
					MathOpcodes.EIM(this, GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x67: // ASR Indexed
					Cycles += 6;
					MathOpcodes.ASR(this, GetIndexedAddress());
					break;
				case 0x6A: // DEC Indexed
					Cycles += 6;
					MathOpcodes.DEC(this, GetIndexedAddress());
					break;
				case 0x6B: // TIM Indexed
					Cycles += 5;
					LogicalOperationOpcodes.TIM(this, GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x6C: // INC Indexed
					Cycles += 6;
					MathOpcodes.INC(this, GetIndexedAddress());
					break;
				case 0x6D: // TST Indexed
					Cycles += 4;
					ComparisonAndTestOpcodes.TST(this, GetIndexedAddress());
					break;
				case 0x6E: // JMP Indexed
					Cycles += 3;
					FlowOpcodes.JMP(this, GetIndexedAddress());
					break;
				case 0x6F: // CLR Index
					Cycles += 5;
					ArithmeticOperationOpcodes.CLR(this, GetIndexedAddress());
					break;

				case 0x71: // AIM Direct
					Cycles += 6;
					MathOpcodes.AIM(this, GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x72: // OIM Direct
					Cycles += 6;
					MathOpcodes.OIM(this, GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x74: // LSR Extended
					Cycles += 6;
					MathOpcodes.LSR(this, GetExtendedAddress());
					break;
				case 0x75: // EIM Direct
					Cycles += 6;
					MathOpcodes.EIM(this, GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x77: // ASR Extended
					Cycles += 6;
					MathOpcodes.ASR(this, GetExtendedAddress());
					break;
				case 0x7A: // DEC Extended
					Cycles += 6;
					MathOpcodes.DEC(this, GetExtendedAddress());
					break;
				case 0x7B: // TIM Direct
					Cycles += 4;
					LogicalOperationOpcodes.TIM(this, GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x7C: // INC Extended
					Cycles += 6;
					MathOpcodes.INC(this, GetExtendedAddress());
					break;
				case 0x7D: // TST Extended
					Cycles += 4;
					ComparisonAndTestOpcodes.TST(this, GetExtendedAddress());
					break;
				case 0x7E: // JMP Extended
					Cycles += 3;
					FlowOpcodes.JMP(this, GetExtendedAddress());
					break;
				case 0x7F: // CLR Extended
					Cycles += 5;
					ArithmeticOperationOpcodes.CLR(this, GetExtendedAddress());
					break;

				case 0x80: // SUBA Immediate
					Cycles += 2;
					MathOpcodes.SUB(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x81: // CMPA Immediate
					Cycles += 2;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x84: // ANDA Immediate
					Cycles += 2;
					MathOpcodes.AND(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x85: // BITA Immediate
					Cycles += 2;
					MathOpcodes.BIT(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x86: // LDAA Immediate
					Cycles += 2;
					TransferOpcodes.LDA(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x88: // EORA Immediate
					Cycles += 2;
					MathOpcodes.EOR(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x89: // ADCA Immediate
					Cycles += 2;
					MathOpcodes.ADC(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x8A: // ORAA Immediate
					Cycles += 2;
					MathOpcodes.ORA(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x8B: // ADDA Immediate
					Cycles += 2;
					MathOpcodes.ADD(this, Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x8C: // CPX Immediate
				{
					Cycles += 3;
					//Console.Write("CPX		");
					var msb = Memory[PC++];
					var lsb = Memory[PC++];
					var compareValue = CombineBytes(msb, lsb);

					var result = (ushort)(X - compareValue);

					N = result.GetBit(15);
					Z = result == 0;

					var ix15 = X.GetBit(15);
					var m15 = compareValue.GetBit(15);
					var r15 = result.GetBit(15);

					V = ix15 & !m15 & !r15 | !ix15 & m15 & r15;
					C = !ix15 & m15 | m15 & r15 | r15 & !ix15;

					break;
				}
				case 0x8D: // BSR
					Cycles += 5;
					BranchOpcodes.BSR(this);
					break;
				case 0x8E: // LDS Immediate
					Cycles += 3;
					LoadOpcodes.LDS(this, GetImmediateAddressTwoByte());
					break;

				case 0x90: // SUBA Direct
					Cycles += 3;
					MathOpcodes.SUB(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x91: // CMPA Direct
					Cycles += 3;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x94: // ANDA Direct
					Cycles += 3;
					MathOpcodes.AND(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x95: // BITA Direct
					Cycles += 3;
					MathOpcodes.BIT(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x96: // LDAA Direct
					Cycles += 3;
					TransferOpcodes.LDA(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x97: // STAA Direct
					Cycles += 3;
					StoreOpcodes.STA(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x98: // EORA Direct
					Cycles += 3;
					MathOpcodes.EOR(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x99: // ADCA Direct
					Cycles += 3;
					MathOpcodes.ADC(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x9A: // ORAA Direct
					Cycles += 3;
					MathOpcodes.ORA(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x9B: // ADDA Direct
					Cycles += 3;
					MathOpcodes.ADD(this, Accumulator.A, GetDirectAddress());
					break;
				case 0x9D: // JSR Direct
					Cycles += 5;
					FlowOpcodes.JSR(this, GetDirectAddress());
					break;
				case 0x9E: // LDS Direct
					Cycles += 4;
					LoadOpcodes.LDS(this, GetDirectAddress());
					break;
				case 0x9F: // STS Direct
					Cycles += 4;
					StoreOpcodes.STS(this, GetDirectAddress());
					break;

				case 0xA0: // SUBA Indexed
					Cycles += 4;
					MathOpcodes.SUB(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xA1: // CMPA Indexed
					Cycles += 4;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xA4: // ANDA Indexed
					Cycles += 4;
					MathOpcodes.AND(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xA5: // BITA Indexed
					Cycles += 4;
					MathOpcodes.BIT(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xA6: // LDAA Indexed
					Cycles += 4;
					TransferOpcodes.LDA(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xA7: // STAA Indexed
					Cycles += 4;
					StoreOpcodes.STA(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xA8: // EORA Indexed
					Cycles += 4;
					MathOpcodes.EOR(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xA9: // ADCA Indexed
					Cycles += 4;
					MathOpcodes.ADC(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xAA: // ORAA Indexed
					Cycles += 4;
					MathOpcodes.ORA(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xAB: // ADDA Indexed
					Cycles += 4;
					MathOpcodes.ADD(this, Accumulator.A, GetIndexedAddress());
					break;
				case 0xAD: // JSR Indexed
					Cycles += 5;
					FlowOpcodes.JSR(this, GetIndexedAddress());
					break;
				case 0xAE: // LDS Indexed
					Cycles += 5;
					LoadOpcodes.LDS(this, GetIndexedAddress());
					break;
				case 0xAF: // STS Indexed
					Cycles += 5;
					StoreOpcodes.STS(this, GetIndexedAddress());
					break;

				case 0xB0: // SUBA Extended
					Cycles += 4;
					MathOpcodes.SUB(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xB1: // CMPA Extended
					Cycles += 4;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xB4: // ANDA Extended
					Cycles += 4;
					MathOpcodes.AND(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xB5: // BITA Extended
					Cycles += 4;
					MathOpcodes.BIT(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xB6: // LDAA Extended
					Cycles += 4;
					TransferOpcodes.LDA(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xB7: // STAA Extended
					Cycles += 4;
					StoreOpcodes.STA(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xB8: // EORA Extended
					Cycles += 4;
					MathOpcodes.EOR(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xB9: // ADCA Extended
					Cycles += 4;
					MathOpcodes.ADC(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xBA: // ORAA Extended
					Cycles += 4;
					MathOpcodes.ORA(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xBB: // ADDA Extended
					Cycles += 4;
					MathOpcodes.ADD(this, Accumulator.A, GetExtendedAddress());
					break;
				case 0xBD: // JSR Extended
					Cycles += 6;
					FlowOpcodes.JSR(this, GetExtendedAddress());
					break;
				case 0xBE: // LDS Extended
					Cycles += 5;
					LoadOpcodes.LDS(this, GetExtendedAddress());
					break;
				case 0xBF: // STS Extended
					Cycles += 5;
					StoreOpcodes.STS(this, GetExtendedAddress());
					break;

				case 0xC0: // SUBB Immediate
					Cycles += 2;
					MathOpcodes.SUB(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC1: // CMPB Immediate
					Cycles += 2;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC3: // ADDD Immediate
					Cycles += 3;
					MathOpcodes.ADDD(this, GetImmediateAddressTwoByte());
					break;
				case 0xC4: // ANDB Immediate
					Cycles += 2;
					MathOpcodes.AND(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC5: // BITB Immediate
					Cycles += 2;
					MathOpcodes.BIT(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC6: // LDAB Immediate
					Cycles += 2;
					TransferOpcodes.LDA(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC8: // EORB Immediate
					Cycles += 2;
					MathOpcodes.EOR(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC9: // ADCB Immediate
					Cycles += 2;
					MathOpcodes.ADC(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xCA: // ORAB Immediate
					Cycles += 2;
					MathOpcodes.ORA(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xCB: // ADDB Immediate
					Cycles += 2;
					MathOpcodes.ADD(this, Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xCC: // LDD Immediate
					Cycles += 3;
					LoadOpcodes.LDD(this, GetImmediateAddressTwoByte());
					break;
				case 0xCE: // LDX Immediate
					Cycles += 3;
					LoadOpcodes.LDX(this, GetImmediateAddressTwoByte());
					break;

				case 0xD0: // SUBB Direct
					Cycles += 3;
					MathOpcodes.SUB(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xD1: // CMPB Direct
					Cycles += 3;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xD3: // ADDD Direct
					Cycles += 4;
					MathOpcodes.ADDD(this, GetDirectAddress());
					break;
				case 0xD4: // ANDB Direct
					Cycles += 3;
					MathOpcodes.AND(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xD5: // BITB Direct
					Cycles += 3;
					MathOpcodes.BIT(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xD6: // LDAB Direct
					Cycles += 3;
					TransferOpcodes.LDA(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xD7: // STAB Direct
					Cycles += 3;
					StoreOpcodes.STA(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xD8: // EORB Direct
					Cycles += 3;
					MathOpcodes.EOR(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xD9: // ADCB Direct
					Cycles += 3;
					MathOpcodes.ADC(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xDA: // ORAB Direct
					Cycles += 3;
					MathOpcodes.ORA(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xDB: // ADDB Direct
					Cycles += 3;
					MathOpcodes.ADD(this, Accumulator.B, GetDirectAddress());
					break;
				case 0xDC: // LDD Direct
					Cycles += 4;
					LoadOpcodes.LDD(this, GetDirectAddress());
					break;
				case 0xDD: // STD Direct
					Cycles += 4;
					StoreOpcodes.STD(this, GetDirectAddress());
					break;
				case 0xDE: // LDX Direct
					Cycles += 4;
					LoadOpcodes.LDX(this, GetDirectAddress());
					break;
				case 0xDF: // STX Direct
					Cycles += 4;
					StoreOpcodes.STX(this, GetDirectAddress());
					break;

				case 0xE0: // SUBB Indexed
					Cycles += 4;
					MathOpcodes.SUB(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xE1: // CMPB Indexed
					Cycles += 4;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xE3: // ADDD Indexed
					Cycles += 5;
					MathOpcodes.ADDD(this, GetIndexedAddress());
					break;
				case 0xE4: // ANDB Indexed
					Cycles += 4;
					MathOpcodes.AND(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xE5: // BITB Indexed
					Cycles += 4;
					MathOpcodes.BIT(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xE6: // LDAB Indexed
					Cycles += 4;
					TransferOpcodes.LDA(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xE7: // STAB Indexed
					Cycles += 4;
					StoreOpcodes.STA(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xE8: // EORB Indexed
					Cycles += 4;
					MathOpcodes.EOR(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xE9: // ADCB Indexed
					Cycles += 4;
					MathOpcodes.ADC(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xEA: // ORAB Indexed
					Cycles += 4;
					MathOpcodes.ORA(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xEB: // ADDB Indexed
					Cycles += 4;
					MathOpcodes.ADD(this, Accumulator.B, GetIndexedAddress());
					break;
				case 0xEC: // LDD Indexed
					Cycles += 5;
					LoadOpcodes.LDD(this, GetIndexedAddress());
					break;
				case 0xED: // STD Indexed
					Cycles += 5;
					StoreOpcodes.STD(this, GetIndexedAddress());
					break;
				case 0xEE: // LDX Indexed
					Cycles += 5;
					LoadOpcodes.LDX(this, GetIndexedAddress());
					break;
				case 0xEF: // STX Indexed
					Cycles += 5;
					StoreOpcodes.STX(this, GetIndexedAddress());
					break;

				case 0xF0: // SUBB Extended
					Cycles += 4;
					MathOpcodes.SUB(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xF1: // CMPB Extended
					Cycles += 4;
					ComparisonAndTestOpcodes.CMP(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xF3: // ADDD Extended
					Cycles += 5;
					MathOpcodes.ADDD(this, GetExtendedAddress());
					break;
				case 0xF4: // ANDB Extended
					Cycles += 4;
					MathOpcodes.AND(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xF5: // BITB Extended
					Cycles += 4;
					MathOpcodes.BIT(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xF6: // LDAB Extended
					Cycles += 4;
					TransferOpcodes.LDA(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xF7: // STAB Extended
					Cycles += 4;
					StoreOpcodes.STA(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xF8: // EORB Extended
					Cycles += 4;
					MathOpcodes.EOR(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xF9: // ADCB Extended
					Cycles += 4;
					MathOpcodes.ADC(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xFA: // ORAB Extended
					Cycles += 4;
					MathOpcodes.ORA(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xFB: // ADDB Extended
					Cycles += 4;
					MathOpcodes.ADD(this, Accumulator.B, GetExtendedAddress());
					break;
				case 0xFC: // LDD Extended
					Cycles += 5;
					LoadOpcodes.LDD(this, GetExtendedAddress());
					break;
				case 0xFD: // STD Extended
					Cycles += 5;
					StoreOpcodes.STD(this, GetExtendedAddress());
					break;
				case 0xFE: // LDX Extended
					Cycles += 5;
					LoadOpcodes.LDX(this, GetExtendedAddress());
					break;
				case 0xFF: // STX Extended
					Cycles += 5;
					StoreOpcodes.STX(this, GetExtendedAddress());
					break;

				default:
					throw new NotImplementedException($"Unknown opcode {opcode:X2} at address {(PC - 1):X4}");
			}

			if (Cycles <= cyclesBefore)
			{
				throw new Exception($"Opcode {opcode:X2} did not affect cycles counter correctly. Before:{cyclesBefore} After:{Cycles}");
			}

			//Console.Write($"{PC:X4}-");

			//Console.WriteLine($"A: {A:X2}, B: {B:X2}, X: {X:X4}, C: {C}, N: {N}, Z: {Z}");
		}

		private ushort GetImmediateAddressOneByte()
		{
			return PC++;
		}

		private ushort GetImmediateAddressTwoByte()
		{
			var val = PC++;
			PC++;
			return val;
		}

		private byte GetDirectAddress()
		{
			return Memory[PC++];
		}

		private ushort GetExtendedAddress()
		{
			var msb = Memory[PC++];
			var lsb = Memory[PC++];
			return CombineBytes(msb, lsb);
		}

		private ushort GetIndexedAddress()
		{
			return (ushort)(X + Memory[PC++]);
		}

		public ushort CombineBytes(byte msb, byte lsb)
		{
			return (ushort)((msb << 8) + lsb);
		}
	}

	public static class Extensions
	{
		public static byte GetHighByte(this ushort i) => (byte)((i & 0xFF00) >> 8);
		public static byte GetLowByte(this ushort i) => (byte)(i & 0x00FF);

		public static byte GetHighByte(this short i) => (byte)((i & 0xFF00) >> 8);
		public static byte GetLowByte(this short i) => (byte)(i & 0x00FF);

		public static byte SetBit(this byte b, int bitPos, bool val)
		{
			var ret = b;

			var mask = (byte)(1 << bitPos);
			if (val)
			{
				ret |= mask;
			}
			else
			{
				ret &= (byte)~mask;
			}

			return ret;
		}

		public static sbyte SetBit(this sbyte b, int bitPos, bool val)
		{
			var ret = (byte)b;

			var mask = (byte)(1 << bitPos);
			if (val)
			{
				ret |= mask;
			}
			else
			{
				ret &= (byte)~mask;
			}

			return (sbyte)ret;
		}

		public static bool GetBit(this byte b, int bitPos)
		{
			return (b & (1 << bitPos)) != 0;
		}

		public static bool GetBit(this sbyte b, int bitPos)
		{
			return (b & (1 << bitPos)) != 0;
		}

		public static bool GetBit(this ushort b, int bitPos)
		{
			return (b & (1 << bitPos)) != 0;
		}
	}
}
