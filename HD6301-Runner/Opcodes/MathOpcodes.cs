namespace HD6301_Runner.Opcodes
{
    public static class MathOpcodes
    {
	    public static void AND(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var val = cpu.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    cpu.A = (byte)(cpu.A & val);

			    cpu.N = cpu.A.GetBit(7);
			    cpu.Z = cpu.A == 0;
		    }
		    else
		    {
			    cpu.B = (byte)(cpu.B & val);

			    cpu.N = cpu.B.GetBit(7);
			    cpu.Z = cpu.B == 0;
			}

		    cpu.V = false;
	    }

	    public static void OIM(HD6301 cpu, ushort immAddress, ushort address)
	    {
		    var val = cpu.Memory[immAddress] | cpu.Memory[address];

			cpu.Memory[address] = (byte)val;

			cpu.N = val < 0;
			cpu.Z = val == 0;
			cpu.V = false;
	    }

	    public static void AIM(HD6301 cpu, ushort immAddress, ushort address)
	    {
		    var val = cpu.Memory[immAddress] & cpu.Memory[address];

		    cpu.Memory[address] = (byte)val;

		    cpu.N = val < 0;
		    cpu.Z = val == 0;
		    cpu.V = false;
	    }

	    public static void EIM(HD6301 cpu, ushort immAddress, ushort address)
	    {
		    var val = cpu.Memory[immAddress] ^ cpu.Memory[address];

		    cpu.Memory[address] = (byte)val;

		    cpu.N = val < 0;
		    cpu.Z = val == 0;
		    cpu.V = false;
	    }

	    public static void ORA(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var val = cpu.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    cpu.A = (byte)(cpu.A | val);

			    cpu.N = cpu.A.GetBit(7);
			    cpu.Z = cpu.A == 0;
		    }
		    else
		    {
			    cpu.B = (byte)(cpu.B | val);

			    cpu.N = cpu.B.GetBit(7);
			    cpu.Z = cpu.B == 0;
		    }

		    cpu.V = false;
		}

	    public static void EOR(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var val = cpu.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    cpu.A = (byte)(cpu.A ^ val);

			    cpu.N = cpu.A.GetBit(7);
			    cpu.Z = cpu.A == 0;
		    }
		    else
		    {
			    cpu.B = (byte)(cpu.B ^ val);

			    cpu.N = cpu.B.GetBit(7);
			    cpu.Z = cpu.B == 0;
		    }

		    cpu.V = false;
		}

	    public static void BIT(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var accVal = acc == Accumulator.A ? cpu.A : cpu.B;
		    var val = accVal & cpu.Memory[address];

		    cpu.N = val < 0;
		    cpu.Z = val == 0;
		    cpu.V = false;
	    }

	    public static void INC(HD6301 cpu, Accumulator acc)
	    {
		    var x = acc == Accumulator.A ? cpu.A : cpu.B;
		    var r = (byte)(x + 1);

		    cpu.N = r.GetBit(7);
		    cpu.Z = r == 0;
		    cpu.V = !x.GetBit(7) & x.GetBit(6) & x.GetBit(5) & x.GetBit(4) & x.GetBit(3) & x.GetBit(2) & x.GetBit(1) & x.GetBit(0);

			if (acc == Accumulator.A)
		    {
			    cpu.A = r;
		    }
		    else
		    {
			    cpu.B = r;
		    }
	    }

	    public static void INC(HD6301 cpu, ushort address)
	    {
		    var x = cpu.Memory[address];
		    var r = (byte)(x + 1);

		    cpu.N = r.GetBit(7);
		    cpu.Z = r == 0;
		    cpu.V = !x.GetBit(7) & x.GetBit(6) & x.GetBit(5) & x.GetBit(4) & x.GetBit(3) & x.GetBit(2) & x.GetBit(1) & x.GetBit(0);

			cpu.Memory[address] = r;
	    }

	    public static void DEC(HD6301 cpu, Accumulator acc)
	    {
		    var x = acc == Accumulator.A ? cpu.A : cpu.B;
		    var r = (byte)(x - 1);

		    cpu.N = r.GetBit(7);
		    cpu.Z = r == 0;
		    cpu.V = x.GetBit(7) & !x.GetBit(6) & !x.GetBit(5) & !x.GetBit(4) & !x.GetBit(3) & !x.GetBit(2) & !x.GetBit(1) & !x.GetBit(0);

		    if (acc == Accumulator.A)
		    {
			    cpu.A = r;
		    }
		    else
		    {
			    cpu.B = r;
		    }
		}

	    public static void DEC(HD6301 cpu, ushort address)
	    {
		    var x = cpu.Memory[address];
		    var r = (byte)(x - 1);

		    cpu.N = r.GetBit(7);
		    cpu.Z = r == 0;
		    cpu.V = x.GetBit(7) & !x.GetBit(6) & !x.GetBit(5) & !x.GetBit(4) & !x.GetBit(3) & !x.GetBit(2) & !x.GetBit(1) & !x.GetBit(0);

		    cpu.Memory[address] = r;
	    }

	    public static void ASR(HD6301 cpu, Accumulator acc)
	    {
		    if (acc == Accumulator.A)
		    {
			    cpu.C = cpu.A.GetBit(0);
			    cpu.A = (byte)(cpu.A >> 1);
				cpu.N = cpu.A.GetBit(7);
			    cpu.Z = cpu.A == 0;
			    cpu.V = cpu.N ^ cpu.C;
		    }
		    else
		    {
			    cpu.C = cpu.B.GetBit(0);
			    cpu.B = (byte)(cpu.B >> 1);
				cpu.N = cpu.B.GetBit(7);
			    cpu.Z = cpu.B == 0;
			    cpu.V = cpu.N ^ cpu.C;
			}
	    }

	    public static void ASR(HD6301 cpu, ushort address)
	    {
		    var val = cpu.Memory[address];

		    cpu.C = val.GetBit(0);
			cpu.Memory[address] = (byte)(val >> 1);
			cpu.N = cpu.Memory[address].GetBit(7);
		    cpu.Z = cpu.Memory[address] == 0;
		    cpu.V = cpu.N ^ cpu.C;
		}

	    

	    public static void LSR(HD6301 cpu, Accumulator acc)
	    {
		    if (acc == Accumulator.A)
		    {
			    cpu.C = cpu.A.GetBit(0);
			    cpu.A = (byte)(cpu.A >>> 1);
			    cpu.N = cpu.A.GetBit(7);
			    cpu.Z = cpu.A == 0;
			    cpu.V = cpu.N ^ cpu.C;
		    }
		    else
		    {
			    cpu.C = cpu.B.GetBit(0);
			    cpu.B = (byte)(cpu.B >>> 1);
			    cpu.N = cpu.B.GetBit(7);
			    cpu.Z = cpu.B == 0;
			    cpu.V = cpu.N ^ cpu.C;
		    }
		}

	    public static void LSR(HD6301 cpu, ushort address)
	    {
		    var val = cpu.Memory[address];

		    cpu.C = val.GetBit(0);
		    cpu.Memory[address] = (byte)(val >>> 1);
		    cpu.N = cpu.Memory[address].GetBit(7);
		    cpu.Z = cpu.Memory[address] == 0;
		    cpu.V = cpu.N ^ cpu.C;
	    }

	    public static void SUB(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var val = cpu.Memory[address];

		    var accValue = acc == Accumulator.A ? cpu.A : cpu.B;

		    var result = (byte)(accValue - val);

			if (acc == Accumulator.A)
			{
				cpu.A = result;
			}
			else
			{
				cpu.B = result;
			}

			cpu.N = result.GetBit(7);
			cpu.Z = result == 0;
			cpu.V = ComparisonAndTestOpcodes.CalculateV(accValue, val, result);
			cpu.C = ComparisonAndTestOpcodes.CalculateC(accValue, val, result);
		}

	    public static void SBA(HD6301 cpu)
	    {
		    var result = (byte)(cpu.A - cpu.B);

		    cpu.N = result.GetBit(7);
		    cpu.Z = result == 0;
		    cpu.V = ComparisonAndTestOpcodes.CalculateV(cpu.A, cpu.B, result);
		    cpu.C = ComparisonAndTestOpcodes.CalculateC(cpu.A, cpu.B, result);

		    cpu.A = result;
	    }

	    public static void ADD(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var accValue = acc == Accumulator.A ? cpu.A : cpu.B;
		    var val = cpu.Memory[address];
			var result = (byte)(accValue + val);

			cpu.N = result.GetBit(7);
			cpu.Z = result == 0;

			var x3 = accValue.GetBit(3);
		    var m3 = val.GetBit(3);
		    var r3 = result.GetBit(3);

		    var x7 = accValue.GetBit(7);
		    var m7 = val.GetBit(7);
		    var r7 = result.GetBit(7);

		    cpu.V = x7 & m7 & !r7 | !x7 & m7 & r7;
		    cpu.C = x7 & m7 | m7 & !r7 | !r7 & x7;
		    cpu.H = x3 & m3 | m3 & !r3 | !r3 & x3;

		    if (acc == Accumulator.A)
		    {
			    cpu.A = result;
		    }
		    else
		    {
			    cpu.B = result;
		    }
		}

	    public static void ADC(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var accValue = acc == Accumulator.A ? cpu.A : cpu.B;
		    var val = cpu.Memory[address];
		    var carry = cpu.C ? 1 : 0;
		    var result = (byte)(accValue + val + carry);

		    cpu.N = result.GetBit(7);
		    cpu.Z = result == 0;

		    var x3 = accValue.GetBit(3);
		    var m3 = val.GetBit(3);
		    var r3 = result.GetBit(3);

		    var x7 = accValue.GetBit(7);
		    var m7 = val.GetBit(7);
		    var r7 = result.GetBit(7);

		    cpu.V = x7 & m7 & !r7 | !x7 & m7 & r7;
		    cpu.C = x7 & m7 | m7 & !r7 | !r7 & x7;
		    cpu.H = x3 & m3 | m3 & !r3 | !r3 & x3;

		    if (acc == Accumulator.A)
		    {
			    cpu.A = result;
		    }
		    else
		    {
			    cpu.B = result;
		    }
		}

	    public static void XGDX(HD6301 cpu)
	    {
		    (cpu.D, cpu.X) = (cpu.X, cpu.D);
	    }

		public static void ADDD(HD6301 cpu, ushort address)
		{
			var ab = cpu.D;

			var high = cpu.Memory[address];
			var low = cpu.Memory[address + 1];
			var m = cpu.CombineBytes(high, low);

			var r = (ushort)(ab + m);

			var ab15 = ab.GetBit(15);
			var m15 = m.GetBit(15);
			var r15 = r.GetBit(15);

			cpu.N = r15;
			cpu.Z = r == 0;
			cpu.V = ab15 & m15 & !r15 | !ab15 & !m15 & r15;
			cpu.C = ab15 & m15 | m15 & !r15 | !r15 & ab15;

			cpu.D = r;
		}
	}
}
