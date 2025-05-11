namespace _6301_runner.Opcodes
{
    public static class ComparisonAndTestOpcodes
    {
	    public static void TST(Accumulator acc)
	    {
		    TSTCommon(acc == Accumulator.A ? Program.A : Program.B);
	    }

	    public static void TST(ushort address)
	    {
			TSTCommon(Program.Memory[address]);
	    }

	    private static void TSTCommon(byte val)
	    {
		    Program.N = val.GetBit(7);
		    Program.Z = val == 0;
		    Program.V = false;
		    Program.C = false;
	    }

	    public static bool CalculateV(byte x, byte m, byte r)
	    {
		    return x.GetBit(7) & !m.GetBit(7) & !r.GetBit(7) | !x.GetBit(7) & m.GetBit(7) & r.GetBit(7);
	    }

	    public static bool CalculateC(byte x, byte m, byte r)
	    {
		    return !x.GetBit(7) & m.GetBit(7) | m.GetBit(7) & r.GetBit(7) | r.GetBit(7) & !x.GetBit(7);
	    }

		public static void CMP(Accumulator acc, ushort address)
	    {
		    var accVal = acc == Accumulator.A ? Program.A : Program.B;

		    var cmpVal = (byte)(accVal - Program.Memory[address]);

		    Program.N = cmpVal.GetBit(7);
		    Program.Z = cmpVal == 0;
		    Program.V = CalculateV(accVal, Program.Memory[address], cmpVal);
		    Program.C = CalculateC(accVal, Program.Memory[address], cmpVal);
		}

	    public static void CBA()
	    {
		    var cmpVal = (byte)(Program.A - Program.B);

		    Program.N = cmpVal.GetBit(7);
			Program.Z = cmpVal == 0;
		    Program.V = CalculateV(Program.A, Program.B, cmpVal);
		    Program.C = CalculateC(Program.A, Program.B, cmpVal);
		}
    }
}
