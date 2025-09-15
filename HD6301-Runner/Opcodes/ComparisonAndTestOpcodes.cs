namespace HD6301_Runner.Opcodes
{
    public static class ComparisonAndTestOpcodes
    {
	    public static void TST(HD6301 cpu, Accumulator acc)
	    {
		    TSTCommon(cpu, acc == Accumulator.A ? cpu.A : cpu.B);
	    }

	    public static void TST(HD6301 cpu, ushort address)
	    {
			TSTCommon(cpu, cpu.Memory[address]);
	    }

	    private static void TSTCommon(HD6301 cpu, byte val)
	    {
		    cpu.N = val.GetBit(7);
		    cpu.Z = val == 0;
		    cpu.V = false;
		    cpu.C = false;
	    }

	    public static bool CalculateV(byte x, byte m, byte r)
	    {
		    return x.GetBit(7) & !m.GetBit(7) & !r.GetBit(7) | !x.GetBit(7) & m.GetBit(7) & r.GetBit(7);
	    }

	    public static bool CalculateC(byte x, byte m, byte r)
	    {
		    return !x.GetBit(7) & m.GetBit(7) | m.GetBit(7) & r.GetBit(7) | r.GetBit(7) & !x.GetBit(7);
	    }

		public static void CMP(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var accVal = acc == Accumulator.A ? cpu.A : cpu.B;

		    var cmpVal = (byte)(accVal - cpu.Memory[address]);

		    cpu.N = cmpVal.GetBit(7);
		    cpu.Z = cmpVal == 0;
		    cpu.V = CalculateV(accVal, cpu.Memory[address], cmpVal);
		    cpu.C = CalculateC(accVal, cpu.Memory[address], cmpVal);
		}

	    public static void CBA(HD6301 cpu)
	    {
		    var cmpVal = (byte)(cpu.A - cpu.B);

		    cpu.N = cmpVal.GetBit(7);
			cpu.Z = cmpVal == 0;
		    cpu.V = CalculateV(cpu.A, cpu.B, cmpVal);
		    cpu.C = CalculateC(cpu.A, cpu.B, cmpVal);
		}
    }
}
