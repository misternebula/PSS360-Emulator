using _6301_runner.Opcodes;
using Console = System.Console;

namespace _6301_runner
{
	public class Program
	{
		public static byte[] Memory;

		public static ushort D;

		public static byte A
		{
			get => (byte)((D & 0xFF00) >> 8);
			set
			{
				D = (ushort)(D & 0x00FF);
				D = (ushort)(D | ((value & 0x00FF) << 8));
			}
		}

		public static byte B
		{
			get => (byte)(D & 0x00FF);
			set
			{
				D = (ushort)(D & 0xFF00);
				D = (ushort)(D | (value & 0x00FF));
			}
		}

		public static ushort X; // Index Register
		public static ushort SP; // Stack Pointer
		public static UInt16 PC; // Program Counter

		public static byte ConditionCodeRegister;

		public static bool C
		{
			get => (ConditionCodeRegister & (1 << 0)) != 0;
			set => ConditionCodeRegister = ConditionCodeRegister.SetBit(0, value);
		}

		public static bool V
		{
			get => (ConditionCodeRegister & (1 << 1)) != 0;
			set => ConditionCodeRegister = ConditionCodeRegister.SetBit(1, value);
		}

		public static bool Z
		{
			get => (ConditionCodeRegister & (1 << 2)) != 0;
			set => ConditionCodeRegister = ConditionCodeRegister.SetBit(2, value);
		}

		public static bool N
		{
			get => (ConditionCodeRegister & (1 << 3)) != 0;
			set => ConditionCodeRegister = ConditionCodeRegister.SetBit(3, value);
		}

		public static bool I
		{
			get => (ConditionCodeRegister & (1 << 4)) != 0;
			set => ConditionCodeRegister = ConditionCodeRegister.SetBit(4, value);
		}

		public static bool H
		{
			get => (ConditionCodeRegister & (1 << 5)) != 0;
			set => ConditionCodeRegister = ConditionCodeRegister.SetBit(5, value);
		}

		public static void InjectCode(int startIndex, byte[] asm)
		{
			for (var i = 0; i < asm.Length; i++)
			{
				Memory[startIndex + i] = asm[i];
			}
		}

		public static void Push(byte b)
		{
			Memory[SP] = b;
			SP--;
		}

		public static void Push(ushort s)
		{
			Push(s.GetLowByte());
			Push(s.GetHighByte());
		}

		private static OPL2.OPL2 OPL2Instance;

		static void Main(string[] args)
		{
			OPL2Instance = new OPL2.OPL2();
			OPL2Instance.Start();

			Memory = File.ReadAllBytes("rom.dat");
			PC = 0xC000;

			SetPortValue(2, 0xFF);
			SetPortValue(4, 0xFF);

			Array.Copy(Memory, MemCopy, Memory.Length);

			var instructionIndex = 0;
			var doOtherInterrupt = false;

			while (true)
			{
				if (instructionIndex >= 2000000)
				{
					// enable sustain
					if (GetPortValue(6) == 0b11110111)
					{
						SetPortValue(4, 0b0111111);
					}
					else
					{
						SetPortValue(4, 0b11111111);
					}

					// max rythm and abc volume
					if (GetPortValue(6) == 0b11111110 || GetPortValue(6) == 0b11111101)
					{
						SetPortValue(2, 0b11101111);
					}
					else
					{
						SetPortValue(2, 0b11111111);
					}
				}

				if (instructionIndex >= 2000000 && instructionIndex <= 2200000)
				{
					// press C1
					/*if (GetPortValue(6) == 0b11111110)
					{
						SetPortValue(4, 0b11111110);
					}
					else
					{
						SetPortValue(4, 0b11111111);
					}*/

					// press C5
					/*if (GetPortValue(5) == 0b11111110)
					{
						SetPortValue(4, 0b11111110);
					}
					else
					{
						SetPortValue(4, 0b11111111);
					}*/

					// press E3
					//if (GetPortValue(5) == 0b11110111)
					//{
					//	SetPortValue(4, 0b11111011);
					//}
					//else
					//{
					//	SetPortValue(4, 0b11111111);
					//}

					// press C5 and A4
					/*if (GetPortValue(5) == 0b11111110)
					{
						SetPortValue(4, 0b11110110);
					}
					else
					{
						SetPortValue(4, 0b11111111);
					}*/

					// press demo
					if (GetPortValue(6) == 0b11011111)
					{
						SetPortValue(4, 0b11011111);
					}
					else
					{
						SetPortValue(4, 0b11111111);
					}

					// press start
					/*if (GetPortValue(6) == 0b11111011)
					{
						SetPortValue(4, 0b10111111);
					}
					else
					{
						SetPortValue(4, 0b11111111);
					}*/
				}

				// TODO: replace with actual timers
				if (instructionIndex % 4000 == 0 && I == false && TCSR3 != 0)
				{
					if (doOtherInterrupt)
					{
						Interrupt(0xFFF4);
					}
					else
					{
						Interrupt(0xFFEC);
					}

					doOtherInterrupt = !doOtherInterrupt;
				}

				Execute();
				PortWatcher();
				MemoryWatcher();

				instructionIndex++;
			}
		}

		private static void Interrupt(ushort interruptAddress)
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
			Memory[SP] = ConditionCodeRegister;
			SP--;

			I = true;

			var high = Memory[interruptAddress];
			var low = Memory[interruptAddress + 1];
			PC = CombineBytes(high, low);
		}

		private static byte[] PortOffsets = new byte[]
		{
			0x0002,
			0x0003,
			0x0006,
			0x0007,
			0x0015,
			0x0017,
			0x0018
		};

		private static byte GetPortValue(int portNum)
		{
			return Memory[PortOffsets[portNum - 1]];
		}

		private static void SetPortValue(int portNum, byte val)
		{
			Memory[PortOffsets[portNum - 1]] = val;
		}

		private static byte P1DDR;
		private static byte P2DDR;
		private static byte PORT1;
		private static byte PORT2;
		private static byte P3DDR;
		private static byte P4DDR;
		private static byte PORT3;
		private static byte PORT4;
		public static byte TCSR1;
		//private static byte FRCH;
		//private static byte FRCL;
		public static byte OCR1H;
		public static byte OCR1L;
		//private static byte ICRH;
		//private static byte ICRL;
		public static byte TCSR2;
		//private static byte RMCR;
		//private static byte TRCSR1;
		//private static byte RDR;
		//private static byte TDR;
		private static byte RP5CR;
		private static byte PORT5;
		private static byte P6DDR;
		private static byte PORT6;
		private static byte PORT7;
		public static byte OCR2H;
		public static byte OCR2L;
		public static byte TCSR3;
		public static byte TCONR;
		//private static byte T2CNT;
		//private static byte TRCSR2;
		//private static byte TSTREG;
		private static byte P5DDR;
		//private static byte P6CSR;

		private static bool wasWriteAddressOperation;
		private static bool wasWriteDataOperation;
		private static bool wasReadOperation;

		private static byte OPL2Address;

		private static void PortWatcher()
		{
			CheckRegister("P1DDR", ref P1DDR, 0x0000);
			CheckRegister("P2DDR", ref P2DDR, 0x0001);
			CheckRegister("PORT1", ref PORT1, 0x0002);
			CheckRegister("PORT2", ref PORT2, 0x0003);
			CheckRegister("P3DDR", ref P3DDR, 0x0004);
			CheckRegister("P4DDR", ref P4DDR, 0x0005);
			CheckRegister("PORT3", ref PORT3, 0x0006);
			CheckRegister("PORT4", ref PORT4, 0x0007);
			CheckRegister("Timer Control / Status Register 1", ref TCSR1, 0x0008);
			CheckRegister("Output Compare Register 1 (MSB)", ref OCR1H, 0x000B);
			CheckRegister("Output Compare Register 1 (LSB)", ref OCR1L, 0x000C);
			CheckRegister("Timer Control / Status Register 2", ref TCSR2, 0x000F);
			CheckRegister("RAM / Port 5 Control Register", ref RP5CR, 0x0014);
			CheckRegister("PORT5", ref PORT5, 0x0015);
			CheckRegister("P6DDR", ref P6DDR, 0x0016);
			CheckRegister("PORT6", ref PORT6, 0x0017);
			CheckRegister("PORT7", ref PORT7, 0x0018);
			CheckRegister("Output Compare Register 2 (MSB)", ref OCR2H, 0x0019);
			CheckRegister("Output Compare Register 2 (LSB)", ref OCR2L, 0x001A);
			CheckRegister("Timer Control / Status Register 3", ref TCSR3, 0x001B);
			CheckRegister("Time Constant Register", ref TCONR, 0x001C);
			CheckRegister("P5DDR", ref P5DDR, 0x0020);

			var OPL2ChipSelect = PORT1.GetBit(2);
			var OPL2ReadEnable = PORT7.GetBit(0);
			var OPL2WriteEnable = PORT7.GetBit(1);
			var OPL2A0 = PORT1.GetBit(0);

			var writeAddressOperation = !OPL2ChipSelect && OPL2ReadEnable && !OPL2WriteEnable && !OPL2A0;
			var writeDataOperation = !OPL2ChipSelect && OPL2ReadEnable && !OPL2WriteEnable && OPL2A0;
			var readOperation = !OPL2ChipSelect && !OPL2ReadEnable && OPL2WriteEnable && !OPL2A0;

			if (writeAddressOperation && !wasWriteAddressOperation)
			{
				OPL2Address = PORT3;
			}

			if (writeDataOperation && !wasWriteDataOperation)
			{
				var data = PORT3;
				//Console.WriteLine($"OPL2 - {OPL2Address:X2} = {data:B8}");
				OPL2Instance.Write(OPL2Address, data);
			}

			if (readOperation && !wasReadOperation)
			{
				Console.WriteLine($"Read OPL2 status");
			}

			wasWriteAddressOperation = writeAddressOperation;
			wasWriteDataOperation = writeDataOperation;
			wasReadOperation = readOperation;

			// LEDs are active when pin is low - acts as negative led pin
			var digitalSynth = !PORT2.GetBit(5);
			var programStartEnd = !PORT2.GetBit(6);
			var synchroStart = !PORT2.GetBit(7);

			if (!wasDigitalSynth && digitalSynth)
			{
				//Console.WriteLine("Digital Synthesizer On/Off LED ENABLED");
			}
			else if (wasDigitalSynth && !digitalSynth)
			{
				//Console.WriteLine("Digital Synthesizer On/Off LED DISABLED");
			}

			if (!wasProgramStartEnd && programStartEnd)
			{
				//Console.WriteLine("Program Start/End LED ENABLED");
			}
			else if (wasProgramStartEnd && !programStartEnd)
			{
				//Console.WriteLine("Program Start/End LED DISABLED");
			}

			if (!wasSynchroStart && synchroStart)
			{
				//Console.WriteLine("Synchro Start LED ENABLED");
			}
			else if (wasSynchroStart && !synchroStart)
			{
				//Console.WriteLine("Synchro Start LED DISABLED");
			}

			wasDigitalSynth = digitalSynth;
			wasProgramStartEnd = programStartEnd;
			wasSynchroStart = synchroStart;
		}

		private static bool wasDigitalSynth;
		private static bool wasProgramStartEnd;
		private static bool wasSynchroStart;

		private static byte[] MemCopy = new byte[0x10000];

		private static void MemoryWatcher()
		{
			/*for (var i = 0; i < Memory.Length; i++)
			{
				if (MemCopy[i] != Memory[i])
				{
					//Console.WriteLine($"{i:X4} changed to {Memory[i]:B8}");
				}
			}*/

			if (Memory[0x0098] != MemCopy[0x0098])
			{
				Console.WriteLine($"UNK_STATE set to {Memory[0x0098]:B8}");
			}

			Array.Copy(Memory, MemCopy, Memory.Length);
		}

		private static void CheckRegister(string name, ref byte refValue, byte address)
		{
			var val = Memory[address];
			if (val != refValue)
			{
				if (name is "PORT1" or "PORT3" or "PORT7")
				{

				}
				else
				{
					//Console.WriteLine($"{name} changed to {val:B8}");
				}
			}
			refValue = val;
		}

		private static void Execute()
		{
			var opcode = Memory[PC++];

			switch (opcode)
			{
				case 0x01: // NOP
					break;
				case 0x08: // INX
					IndexRegisterControlOpcodes.INX();
					break;
				case 0x0A: // CLV
					BitControlOpcodes.CLV();
					break;
				case 0x0B: // SEV
					BitControlOpcodes.SEV();
					break;
				case 0x0C: // CLC
					BitControlOpcodes.CLC();
					break;
				case 0x0D: // SEC
					BitControlOpcodes.SEC();
					break;
				case 0x0E: // CLI
					BitControlOpcodes.CLI();
					break;
				case 0x0F: // SEI
					BitControlOpcodes.SEI();
					break;

				case 0x10: // SBA
					MathOpcodes.SBA();
					break;
				case 0x11: // CBA
					ComparisonAndTestOpcodes.CBA();
					break;
				case 0x16: // TAB
				{
					//Console.Write("TAB		");
					B = A;

					N = A.GetBit(7);
					Z = A == 0;
					V = false;
					break;
				}
				case 0x17: // TBA
				{
					//Console.Write("TBA		");
					A = B;

					N = B.GetBit(7);
					Z = B == 0;
					V = false;
					break;
				}
				case 0x18: // XGDX
					MathOpcodes.XGDX();
					break;
				case 0x1B: // ABA
					ArithmeticOperationOpcodes.ABA();
					break;

				case 0x20: // BRA
					BranchOpcodes.BRA();
					break;
				case 0x22: // BHI
					BranchOpcodes.BHI();
					break;
				case 0x23: // BLS
					BranchOpcodes.BLS();
					break;
				case 0x24: // BCC
					BranchOpcodes.BCC();
					break;
				case 0x25: // BCS
					BranchOpcodes.BCS();
					break;
				case 0x26: // BNE
					BranchOpcodes.BNE();
					break;
				case 0x27: // BEQ
					BranchOpcodes.BEQ();
					break;
				case 0x2A: // BPL
					BranchOpcodes.BPL();
					break;
				case 0x2B: // BMI
					BranchOpcodes.BMI();
					break;
				case 0x2C: // BGE
					BranchOpcodes.BGE();
					break;
				case 0x2D:
					BranchOpcodes.BLT();
					break;
				case 0x2E: // BLE
					BranchOpcodes.BGT();
					break;
				case 0x2F: // BLE
					BranchOpcodes.BLE();
					break;

				case 0x32: // PULA
					SP++;
					A = Memory[SP];
					break;
				case 0x33: // PULB
					SP++;
					B = Memory[SP];
					break;
				case 0x36: // PSHA
					Memory[SP] = A;
					SP--;
					break;
				case 0x37: // PSHB
					Memory[SP] = B;
					SP--;
					break;
				case 0x38: // PULX
					SP++;
					var high = Memory[SP];
					SP++;
					var low = Memory[SP];
					X = CombineBytes(high, low);
					break;
				case 0x39: // RTS
					FlowOpcodes.RTS();
					break;
				case 0x3A: // ABX
				{
					//Console.Write("ABX		");
					X = (ushort)(B + X);
					break;
				}
				case 0x3B: // RTI
					FlowOpcodes.RTI();
					break;
				case 0x3C: // PSHX
					Memory[SP] = X.GetLowByte();
					SP--;
					Memory[SP] = X.GetHighByte();
					SP--;
					break;
				case 0x3D: // MUL
					ArithmeticOperationOpcodes.MUL();
					break;

				case 0x43: // COMA
				{
					//Console.Write("COMA		");
					A = (byte)~A;
					N = A.GetBit(7);
					Z = A == 0;
					V = false;
					C = true;
					break;
				}
				case 0x44: // LSRA
					MathOpcodes.LSR(Accumulator.A);
					break;
				case 0x47: // ASRA
					MathOpcodes.ASR(Accumulator.A);
					break;
				case 0x48: // ASLA
				{
					//Console.Write("ASLA		");

					C = A.GetBit(7);
						
					A = (byte)(A << 1);

					N = A.GetBit(7);
					Z = A == 0;
					V = N ^ C;

					break;
				}
				case 0x4A: // DECA
					MathOpcodes.DEC(Accumulator.A);
					break;
				case 0x4C: // INCA
					MathOpcodes.INC(Accumulator.A);
					break;
				case 0x4D: // TSTA
					ComparisonAndTestOpcodes.TST(Accumulator.A);
					break;
				case 0x4F: // CLRA
					ArithmeticOperationOpcodes.CLR(Accumulator.A);
					break;

				case 0x53: // COMB
				{
					//Console.Write("COMB		");
					B = (byte)~B;
					N = B.GetBit(7);
					Z = B == 0;
					V = false;
					C = true;
					break;
				}
				case 0x54: // LSRA
					MathOpcodes.LSR(Accumulator.B);
					break;
				case 0x57: // ASRB
					MathOpcodes.ASR(Accumulator.B);
					break;
				case 0x58: // ASLB
				{
					//Console.Write("ASLB		");

					C = B.GetBit(7);

					B = (byte)(B << 1);

					N = B.GetBit(7);
					Z = B == 0;
					V = N ^ C;

					break;
				}
				case 0x5A: // DECA
					MathOpcodes.DEC(Accumulator.B);
					break;
				case 0x5C: // INCA
					MathOpcodes.INC(Accumulator.B);
					break;
				case 0x5D: // TSTB
					ComparisonAndTestOpcodes.TST(Accumulator.B);
					break;
				case 0x5F: // CLRB
					ArithmeticOperationOpcodes.CLR(Accumulator.B);
					break;

				case 0x61: // AIM Indexed
					MathOpcodes.AIM(GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x62: // OIM Indexed
					MathOpcodes.OIM(GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x64: // LSR Indexed
					MathOpcodes.LSR(GetIndexedAddress());
					break;
				case 0x65: // EIM Indexed
					MathOpcodes.EIM(GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x67: // ASR Indexed
					MathOpcodes.ASR(GetIndexedAddress());
					break;
				case 0x6A: // DEC Indexed
					MathOpcodes.DEC(GetIndexedAddress());
					break;
				case 0x6B: // TIM Indexed
					LogicalOperationOpcodes.TIM(GetImmediateAddressOneByte(), GetIndexedAddress());
					break;
				case 0x6C: // INC Indexed
					MathOpcodes.INC(GetIndexedAddress());
					break;
				case 0x6D: // TST Indexed
					ComparisonAndTestOpcodes.TST(GetIndexedAddress());
					break;
				case 0x6E: // JMP Indexed
					FlowOpcodes.JMP(GetIndexedAddress());
					break;
				case 0x6F: // CLR Index
					ArithmeticOperationOpcodes.CLR(GetIndexedAddress());
					break;

				case 0x71: // AIM Direct
					MathOpcodes.AIM(GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x72: // OIM Direct
					MathOpcodes.OIM(GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x74: // LSR Extended
					MathOpcodes.LSR(GetExtendedAddress());
					break;
				case 0x75: // EIM Direct
					MathOpcodes.EIM(GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x77: // ASR Extended
					MathOpcodes.ASR(GetExtendedAddress());
					break;
				case 0x7A: // DEC Extended
					MathOpcodes.DEC(GetExtendedAddress());
					break;
				case 0x7B: // TIM Direct
					LogicalOperationOpcodes.TIM(GetImmediateAddressOneByte(), GetDirectAddress());
					break;
				case 0x7C: // INC Extended
					MathOpcodes.INC(GetExtendedAddress());
					break;
				case 0x7D: // TST Extended
					ComparisonAndTestOpcodes.TST(GetExtendedAddress());
					break;
				case 0x7E: // JMP Extended
					FlowOpcodes.JMP(GetExtendedAddress());
					break;
				case 0x7F: // CLR Extended
					ArithmeticOperationOpcodes.CLR(GetExtendedAddress());
					break;

				case 0x80: // SUBA Immediate
					MathOpcodes.SUB(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x81: // CMPA Immediate
					ComparisonAndTestOpcodes.CMP(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x84: // ANDA Immediate
					MathOpcodes.AND(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x85: // BITA Immediate
					MathOpcodes.BIT(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x86: // LDAA Immediate
					TransferOpcodes.LDA(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x88: // EORA Immediate
					MathOpcodes.EOR(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x89: // ADCA Immediate
					MathOpcodes.ADC(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x8A: // ORAA Immediate
					MathOpcodes.ORA(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x8B: // ADDA Immediate
					MathOpcodes.ADD(Accumulator.A, GetImmediateAddressOneByte());
					break;
				case 0x8C: // CPX Immediate
				{
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
					BranchOpcodes.BSR();
					break;
				case 0x8E: // LDS Immediate
					LoadOpcodes.LDS(GetImmediateAddressTwoByte());
					break;

				case 0x90: // SUBA Direct
					MathOpcodes.SUB(Accumulator.A, GetDirectAddress());
					break;
				case 0x91: // CMPA Direct
					ComparisonAndTestOpcodes.CMP(Accumulator.A, GetDirectAddress());
					break;
				case 0x94: // ANDA Direct
					MathOpcodes.AND(Accumulator.A, GetDirectAddress());
					break;
				case 0x95: // BITA Direct
					MathOpcodes.BIT(Accumulator.A, GetDirectAddress());
					break;
				case 0x96: // LDAA Direct
					TransferOpcodes.LDA(Accumulator.A, GetDirectAddress());
					break;
				case 0x97: // STAA Direct
					StoreOpcodes.STA(Accumulator.A, GetDirectAddress());
					break;
				case 0x98: // EORA Direct
					MathOpcodes.EOR(Accumulator.A, GetDirectAddress());
					break;
				case 0x99: // ADCA Direct
					MathOpcodes.ADC(Accumulator.A, GetDirectAddress());
					break;
				case 0x9A: // ORAA Direct
					MathOpcodes.ORA(Accumulator.A, GetDirectAddress());
					break;
				case 0x9B: // ADDA Direct
					MathOpcodes.ADD(Accumulator.A, GetDirectAddress());
					break;
				case 0x9D: // JSR Direct
					FlowOpcodes.JSR(GetDirectAddress());
					break;
				case 0x9E: // LDS Direct
					LoadOpcodes.LDS(GetDirectAddress());
					break;
				case 0x9F: // STS Direct
					StoreOpcodes.STS(GetDirectAddress());
					break;

				case 0xA0: // SUBA Indexed
					MathOpcodes.SUB(Accumulator.A, GetIndexedAddress());
					break;
				case 0xA1: // CMPA Indexed
					ComparisonAndTestOpcodes.CMP(Accumulator.A, GetIndexedAddress());
					break;
				case 0xA4: // ANDA Indexed
					MathOpcodes.AND(Accumulator.A, GetIndexedAddress());
					break;
				case 0xA5: // BITA Indexed
					MathOpcodes.BIT(Accumulator.A, GetIndexedAddress());
					break;
				case 0xA6: // LDAA Indexed
					TransferOpcodes.LDA(Accumulator.A, GetIndexedAddress());
					break;
				case 0xA7: // STAA Indexed
					StoreOpcodes.STA(Accumulator.A, GetIndexedAddress());
					break;
				case 0xA8: // EORA Indexed
					MathOpcodes.EOR(Accumulator.A, GetIndexedAddress());
					break;
				case 0xA9: // ADCA Indexed
					MathOpcodes.ADC(Accumulator.A, GetIndexedAddress());
					break;
				case 0xAA: // ORAA Indexed
					MathOpcodes.ORA(Accumulator.A, GetIndexedAddress());
					break;
				case 0xAB: // ADDA Indexed
					MathOpcodes.ADD(Accumulator.A, GetIndexedAddress());
					break;
				case 0xAD: // JSR Indexed
					FlowOpcodes.JSR(GetIndexedAddress());
					break;
				case 0xAE: // LDS Indexed
					LoadOpcodes.LDS(GetIndexedAddress());
					break;
				case 0xAF: // STS Indexed
					StoreOpcodes.STS(GetIndexedAddress());
					break;

				case 0xB0: // SUBA Extended
					MathOpcodes.SUB(Accumulator.A, GetExtendedAddress());
					break;
				case 0xB1: // CMPA Extended
					ComparisonAndTestOpcodes.CMP(Accumulator.A, GetExtendedAddress());
					break;
				case 0xB4: // ANDA Extended
					MathOpcodes.AND(Accumulator.A, GetExtendedAddress());
					break;
				case 0xB5: // BITA Extended
					MathOpcodes.BIT(Accumulator.A, GetExtendedAddress());
					break;
				case 0xB6: // LDAA Extended
					TransferOpcodes.LDA(Accumulator.A, GetExtendedAddress());
					break;
				case 0xB7: // STAA Extended
					StoreOpcodes.STA(Accumulator.A, GetExtendedAddress());
					break;
				case 0xB8: // EORA Extended
					MathOpcodes.EOR(Accumulator.A, GetExtendedAddress());
					break;
				case 0xB9: // ADCA Extended
					MathOpcodes.ADC(Accumulator.A, GetExtendedAddress());
					break;
				case 0xBA: // ORAA Extended
					MathOpcodes.ORA(Accumulator.A, GetExtendedAddress());
					break;
				case 0xBB: // ADDA Extended
					MathOpcodes.ADD(Accumulator.A, GetExtendedAddress());
					break;
				case 0xBD: // JSR Extended
					FlowOpcodes.JSR(GetExtendedAddress());
					break;
				case 0xBE: // LDS Extended
					LoadOpcodes.LDS(GetExtendedAddress());
					break;
				case 0xBF: // STS Extended
					StoreOpcodes.STS(GetExtendedAddress());
					break;

				case 0xC0: // SUBB Immediate
					MathOpcodes.SUB(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC1: // CMPB Immediate
					ComparisonAndTestOpcodes.CMP(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC3: // ADDD Immediate
					MathOpcodes.ADDD(GetImmediateAddressTwoByte());
					break;
				case 0xC4: // ANDB Immediate
					MathOpcodes.AND(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC5: // BITB Immediate
					MathOpcodes.BIT(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC6: // LDAB Immediate
					TransferOpcodes.LDA(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC8: // EORB Immediate
					MathOpcodes.EOR(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xC9: // ADCB Immediate
					MathOpcodes.ADC(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xCA: // ORAB Immediate
					MathOpcodes.ORA(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xCB: // ADDB Immediate
					MathOpcodes.ADD(Accumulator.B, GetImmediateAddressOneByte());
					break;
				case 0xCC: // LDD Immediate
					LoadOpcodes.LDD(GetImmediateAddressTwoByte());
					break;
				case 0xCE: // LDX Immediate
					LoadOpcodes.LDX(GetImmediateAddressTwoByte());
					break;

				case 0xD0: // SUBB Direct
					MathOpcodes.SUB(Accumulator.B, GetDirectAddress());
					break;
				case 0xD1: // CMPB Direct
					ComparisonAndTestOpcodes.CMP(Accumulator.B, GetDirectAddress());
					break;
				case 0xD3: // ADDD Direct
					MathOpcodes.ADDD(GetDirectAddress());
					break;
				case 0xD4: // ANDB Direct
					MathOpcodes.AND(Accumulator.B, GetDirectAddress());
					break;
				case 0xD5: // BITB Direct
					MathOpcodes.BIT(Accumulator.B, GetDirectAddress());
					break;
				case 0xD6: // LDAB Direct
					TransferOpcodes.LDA(Accumulator.B, GetDirectAddress());
					break;
				case 0xD7: // STAB Direct
					StoreOpcodes.STA(Accumulator.B, GetDirectAddress());
					break;
				case 0xD8: // EORB Direct
					MathOpcodes.EOR(Accumulator.B, GetDirectAddress());
					break;
				case 0xD9: // ADCB Direct
					MathOpcodes.ADC(Accumulator.B, GetDirectAddress());
					break;
				case 0xDA: // ORAB Direct
					MathOpcodes.ORA(Accumulator.B, GetDirectAddress());
					break;
				case 0xDB: // ADDB Direct
					MathOpcodes.ADD(Accumulator.B, GetDirectAddress());
					break;
				case 0xDC: // LDD Direct
					LoadOpcodes.LDD(GetDirectAddress());
					break;
				case 0xDD: // STD Direct
					StoreOpcodes.STD(GetDirectAddress());
					break;
				case 0xDE: // LDX Direct
					LoadOpcodes.LDX(GetDirectAddress());
					break;
				case 0xDF: // STX Direct
					StoreOpcodes.STX(GetDirectAddress());
					break;

				case 0xE0: // SUBB Indexed
					MathOpcodes.SUB(Accumulator.B, GetIndexedAddress());
					break;
				case 0xE1: // CMPB Indexed
					ComparisonAndTestOpcodes.CMP(Accumulator.B, GetIndexedAddress());
					break;
				case 0xE3: // ADDD Indexed
					MathOpcodes.ADDD(GetIndexedAddress());
					break;
				case 0xE4: // ANDB Indexed
					MathOpcodes.AND(Accumulator.B, GetIndexedAddress());
					break;
				case 0xE5: // BITB Indexed
					MathOpcodes.BIT(Accumulator.B, GetIndexedAddress());
					break;
				case 0xE6: // LDAB Indexed
					TransferOpcodes.LDA(Accumulator.B, GetIndexedAddress());
					break;
				case 0xE7: // STAB Indexed
					StoreOpcodes.STA(Accumulator.B, GetIndexedAddress());
					break;
				case 0xE8: // EORB Indexed
					MathOpcodes.EOR(Accumulator.B, GetIndexedAddress());
					break;
				case 0xE9: // ADCB Indexed
					MathOpcodes.ADC(Accumulator.B, GetIndexedAddress());
					break;
				case 0xEA: // ORAB Indexed
					MathOpcodes.ORA(Accumulator.B, GetIndexedAddress());
					break;
				case 0xEB: // ADDB Indexed
					MathOpcodes.ADD(Accumulator.B, GetIndexedAddress());
					break;
				case 0xEC: // LDD Indexed
					LoadOpcodes.LDD(GetIndexedAddress());
					break;
				case 0xED: // STD Indexed
					StoreOpcodes.STD(GetIndexedAddress());
					break;
				case 0xEE: // LDX Indexed
					LoadOpcodes.LDX(GetIndexedAddress());
					break;
				case 0xEF: // STX Indexed
					StoreOpcodes.STX(GetIndexedAddress());
					break;

				case 0xF0: // SUBB Extended
					MathOpcodes.SUB(Accumulator.B, GetExtendedAddress());
					break;
				case 0xF1: // CMPB Extended
					ComparisonAndTestOpcodes.CMP(Accumulator.B, GetExtendedAddress());
					break;
				case 0xF3: // ADDD Extended
					MathOpcodes.ADDD(GetExtendedAddress());
					break;
				case 0xF4: // ANDB Extended
					MathOpcodes.AND(Accumulator.B, GetExtendedAddress());
					break;
				case 0xF5: // BITB Extended
					MathOpcodes.BIT(Accumulator.B, GetExtendedAddress());
					break;
				case 0xF6: // LDAB Extended
					TransferOpcodes.LDA(Accumulator.B, GetExtendedAddress());
					break;
				case 0xF7: // STAB Extended
					StoreOpcodes.STA(Accumulator.B, GetExtendedAddress());
					break;
				case 0xF8: // EORB Extended
					MathOpcodes.EOR(Accumulator.B, GetExtendedAddress());
					break;
				case 0xF9: // ADCB Extended
					MathOpcodes.ADC(Accumulator.B, GetExtendedAddress());
					break;
				case 0xFA: // ORAB Extended
					MathOpcodes.ORA(Accumulator.B, GetExtendedAddress());
					break;
				case 0xFB: // ADDB Extended
					MathOpcodes.ADD(Accumulator.B, GetExtendedAddress());
					break;
				case 0xFC: // LDD Extended
					LoadOpcodes.LDD(GetExtendedAddress());
					break;
				case 0xFD: // STD Extended
					StoreOpcodes.STD(GetExtendedAddress());
					break;
				case 0xFE: // LDX Extended
					LoadOpcodes.LDX(GetExtendedAddress());
					break;
				case 0xFF: // STX Extended
					StoreOpcodes.STX(GetExtendedAddress());
					break;

				default:
					throw new NotImplementedException($"Unknown opcode {opcode:X2} at address {(PC - 1):X4}");
			}

			//Console.Write($"{PC:X4}-");

			//Console.WriteLine($"A: {A:X2}, B: {B:X2}, X: {X:X4}, C: {C}, N: {N}, Z: {Z}");
		}

		private static ushort GetImmediateAddressOneByte()
		{
			return PC++;
		}

		private static ushort GetImmediateAddressTwoByte()
		{
			var val = PC++;
			PC++;
			return val;
		}

		private static byte GetDirectAddress()
		{
			return Memory[PC++];
		}

		private static ushort GetExtendedAddress()
		{
			var msb = Memory[PC++];
			var lsb = Memory[PC++];
			return CombineBytes(msb, lsb);
		}

		private static ushort GetIndexedAddress()
		{
			return (ushort)(X + Memory[PC++]);
		}

		public static ushort CombineBytes(byte msb, byte lsb)
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
