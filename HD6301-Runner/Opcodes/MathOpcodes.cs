namespace _6301_runner.Opcodes
{
    public static class MathOpcodes
    {
	    public static void AND(Accumulator acc, ushort address)
	    {
		    var val = Program.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    Program.A = (byte)(Program.A & val);

			    Program.N = Program.A.GetBit(7);
			    Program.Z = Program.A == 0;
		    }
		    else
		    {
			    Program.B = (byte)(Program.B & val);

			    Program.N = Program.B.GetBit(7);
			    Program.Z = Program.B == 0;
			}

		    Program.V = false;
	    }

	    public static void OIM(ushort immAddress, ushort address)
	    {
		    var val = Program.Memory[immAddress] | Program.Memory[address];

			Program.Memory[address] = (byte)val;

			Program.N = val < 0;
			Program.Z = val == 0;
			Program.V = false;
	    }

	    public static void AIM(ushort immAddress, ushort address)
	    {
		    var val = Program.Memory[immAddress] & Program.Memory[address];

		    Program.Memory[address] = (byte)val;

		    Program.N = val < 0;
		    Program.Z = val == 0;
		    Program.V = false;
	    }

	    public static void EIM(ushort immAddress, ushort address)
	    {
		    var val = Program.Memory[immAddress] ^ Program.Memory[address];

		    Program.Memory[address] = (byte)val;

		    Program.N = val < 0;
		    Program.Z = val == 0;
		    Program.V = false;
	    }

	    public static void ORA(Accumulator acc, ushort address)
	    {
		    var val = Program.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    Program.A = (byte)(Program.A | val);

			    Program.N = Program.A.GetBit(7);
			    Program.Z = Program.A == 0;
		    }
		    else
		    {
			    Program.B = (byte)(Program.B | val);

			    Program.N = Program.B.GetBit(7);
			    Program.Z = Program.B == 0;
		    }

		    Program.V = false;
		}

	    public static void EOR(Accumulator acc, ushort address)
	    {
		    var val = Program.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    Program.A = (byte)(Program.A ^ val);

			    Program.N = Program.A.GetBit(7);
			    Program.Z = Program.A == 0;
		    }
		    else
		    {
			    Program.B = (byte)(Program.B ^ val);

			    Program.N = Program.B.GetBit(7);
			    Program.Z = Program.B == 0;
		    }

		    Program.V = false;
		}

	    public static void BIT(Accumulator acc, ushort address)
	    {
		    var accVal = acc == Accumulator.A ? Program.A : Program.B;
		    var val = accVal & Program.Memory[address];

		    Program.N = val < 0;
		    Program.Z = val == 0;
		    Program.V = false;
	    }

	    public static void INC(Accumulator acc)
	    {
		    var x = acc == Accumulator.A ? Program.A : Program.B;
		    var r = (byte)(x + 1);

		    Program.N = r.GetBit(7);
		    Program.Z = r == 0;
		    Program.V = !x.GetBit(7) & x.GetBit(6) & x.GetBit(5) & x.GetBit(4) & x.GetBit(3) & x.GetBit(2) & x.GetBit(1) & x.GetBit(0);

			if (acc == Accumulator.A)
		    {
			    Program.A = r;
		    }
		    else
		    {
			    Program.B = r;
		    }
	    }

	    public static void INC(ushort address)
	    {
		    var x = Program.Memory[address];
		    var r = (byte)(x + 1);

		    Program.N = r.GetBit(7);
		    Program.Z = r == 0;
		    Program.V = !x.GetBit(7) & x.GetBit(6) & x.GetBit(5) & x.GetBit(4) & x.GetBit(3) & x.GetBit(2) & x.GetBit(1) & x.GetBit(0);

			Program.Memory[address] = r;
	    }

	    public static void DEC(Accumulator acc)
	    {
		    var x = acc == Accumulator.A ? Program.A : Program.B;
		    var r = (byte)(x - 1);

		    Program.N = r.GetBit(7);
		    Program.Z = r == 0;
		    Program.V = x.GetBit(7) & !x.GetBit(6) & !x.GetBit(5) & !x.GetBit(4) & !x.GetBit(3) & !x.GetBit(2) & !x.GetBit(1) & !x.GetBit(0);

		    if (acc == Accumulator.A)
		    {
			    Program.A = r;
		    }
		    else
		    {
			    Program.B = r;
		    }
		}

	    public static void DEC(ushort address)
	    {
		    var x = Program.Memory[address];
		    var r = (byte)(x - 1);

		    Program.N = r.GetBit(7);
		    Program.Z = r == 0;
		    Program.V = x.GetBit(7) & !x.GetBit(6) & !x.GetBit(5) & !x.GetBit(4) & !x.GetBit(3) & !x.GetBit(2) & !x.GetBit(1) & !x.GetBit(0);

		    Program.Memory[address] = r;
	    }

	    public static void ASR(Accumulator acc)
	    {
		    if (acc == Accumulator.A)
		    {
			    Program.C = Program.A.GetBit(0);
			    Program.A = (byte)(Program.A >> 1);
				Program.N = Program.A.GetBit(7);
			    Program.Z = Program.A == 0;
			    Program.V = Program.N ^ Program.C;
		    }
		    else
		    {
			    Program.C = Program.B.GetBit(0);
			    Program.B = (byte)(Program.B >> 1);
				Program.N = Program.B.GetBit(7);
			    Program.Z = Program.B == 0;
			    Program.V = Program.N ^ Program.C;
			}
	    }

	    public static void ASR(ushort address)
	    {
		    var val = Program.Memory[address];

		    Program.C = val.GetBit(0);
			Program.Memory[address] = (byte)(val >> 1);
			Program.N = Program.Memory[address].GetBit(7);
		    Program.Z = Program.Memory[address] == 0;
		    Program.V = Program.N ^ Program.C;
		}

	    

	    public static void LSR(Accumulator acc)
	    {
		    if (acc == Accumulator.A)
		    {
			    Program.C = Program.A.GetBit(0);
			    Program.A = (byte)(Program.A >>> 1);
			    Program.N = Program.A.GetBit(7);
			    Program.Z = Program.A == 0;
			    Program.V = Program.N ^ Program.C;
		    }
		    else
		    {
			    Program.C = Program.B.GetBit(0);
			    Program.B = (byte)(Program.B >>> 1);
			    Program.N = Program.B.GetBit(7);
			    Program.Z = Program.B == 0;
			    Program.V = Program.N ^ Program.C;
		    }
		}

	    public static void LSR(ushort address)
	    {
		    var val = Program.Memory[address];

		    Program.C = val.GetBit(0);
		    Program.Memory[address] = (byte)(val >>> 1);
		    Program.N = Program.Memory[address].GetBit(7);
		    Program.Z = Program.Memory[address] == 0;
		    Program.V = Program.N ^ Program.C;
	    }

	    public static void SUB(Accumulator acc, ushort address)
	    {
		    var val = Program.Memory[address];

		    var accValue = acc == Accumulator.A ? Program.A : Program.B;

		    var result = (byte)(accValue - val);

			if (acc == Accumulator.A)
			{
				Program.A = result;
			}
			else
			{
				Program.B = result;
			}

			Program.N = result.GetBit(7);
			Program.Z = result == 0;
			Program.V = ComparisonAndTestOpcodes.CalculateV(accValue, val, result);
			Program.C = ComparisonAndTestOpcodes.CalculateC(accValue, val, result);
		}

	    public static void SBA()
	    {
		    var result = (byte)(Program.A - Program.B);

		    Program.N = result.GetBit(7);
		    Program.Z = result == 0;
		    Program.V = ComparisonAndTestOpcodes.CalculateV(Program.A, Program.B, result);
		    Program.C = ComparisonAndTestOpcodes.CalculateC(Program.A, Program.B, result);

		    Program.A = result;
	    }

	    public static void ADD(Accumulator acc, ushort address)
	    {
		    var accValue = acc == Accumulator.A ? Program.A : Program.B;
		    var val = Program.Memory[address];
			var result = (byte)(accValue + val);

			Program.N = result.GetBit(7);
			Program.Z = result == 0;

			var x3 = accValue.GetBit(3);
		    var m3 = val.GetBit(3);
		    var r3 = result.GetBit(3);

		    var x7 = accValue.GetBit(7);
		    var m7 = val.GetBit(7);
		    var r7 = result.GetBit(7);

		    Program.V = x7 & m7 & !r7 | !x7 & m7 & r7;
		    Program.C = x7 & m7 | m7 & !r7 | !r7 & x7;
		    Program.H = x3 & m3 | m3 & !r3 | !r3 & x3;

		    if (acc == Accumulator.A)
		    {
			    Program.A = result;
		    }
		    else
		    {
			    Program.B = result;
		    }
		}

	    public static void ADC(Accumulator acc, ushort address)
	    {
		    var accValue = acc == Accumulator.A ? Program.A : Program.B;
		    var val = Program.Memory[address];
		    var carry = Program.C ? 1 : 0;
		    var result = (byte)(accValue + val + carry);

		    Program.N = result.GetBit(7);
		    Program.Z = result == 0;

		    var x3 = accValue.GetBit(3);
		    var m3 = val.GetBit(3);
		    var r3 = result.GetBit(3);

		    var x7 = accValue.GetBit(7);
		    var m7 = val.GetBit(7);
		    var r7 = result.GetBit(7);

		    Program.V = x7 & m7 & !r7 | !x7 & m7 & r7;
		    Program.C = x7 & m7 | m7 & !r7 | !r7 & x7;
		    Program.H = x3 & m3 | m3 & !r3 | !r3 & x3;

		    if (acc == Accumulator.A)
		    {
			    Program.A = result;
		    }
		    else
		    {
			    Program.B = result;
		    }
		}

	    public static void XGDX()
	    {
		    (Program.D, Program.X) = (Program.X, Program.D);
	    }

		public static void ADDD(ushort address)
		{
			var ab = Program.D;

			var high = Program.Memory[address];
			var low = Program.Memory[address + 1];
			var m = Program.CombineBytes(high, low);

			var r = (ushort)(ab + m);

			var ab15 = ab.GetBit(15);
			var m15 = m.GetBit(15);
			var r15 = r.GetBit(15);

			Program.N = r15;
			Program.Z = r == 0;
			Program.V = ab15 & m15 & !r15 | !ab15 & !m15 & r15;
			Program.C = ab15 & m15 | m15 & !r15 | !r15 & ab15;

			Program.D = r;
		}
	}
}
