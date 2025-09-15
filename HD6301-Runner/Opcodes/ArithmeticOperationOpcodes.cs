namespace HD6301_Runner.Opcodes
{
    public class ArithmeticOperationOpcodes
    {
	    public static void ABA(HD6301 cpu)
	    {
		    var r = (byte)(cpu.A + cpu.B);

		    cpu.N = r.GetBit(7);
		    cpu.Z = r == 0;

		    var a3 = cpu.A.GetBit(3);
		    var b3 = cpu.B.GetBit(3);
		    var r3 = r.GetBit(3);

		    var a7 = cpu.A.GetBit(7);
		    var b7 = cpu.B.GetBit(7);
		    var r7 = r.GetBit(7);

		    cpu.V = a7 & b7 & !r7 | !a7 & !b7 & r7;
		    cpu.C = a7 & b7 | b7 & !r7 | !r7 & a7;
		    cpu.H = a3 & b3 | b3 & !r3 | !r3 & a3;

		    cpu.A = r;
	    }

		public static void MUL(HD6301 cpu)
	    {
		    cpu.D = (ushort)(cpu.A * cpu.B);
		    cpu.C = cpu.D.GetBit(7);
	    }

		public static void CLR(HD6301 cpu, Accumulator acc)
		{
			if (acc == Accumulator.A)
			{
				cpu.A = 0;
			}
			else
			{
				cpu.B = 0;
			}

			cpu.N = false;
			cpu.Z = true;
			cpu.V = false;
			cpu.C = false;
		}

		public static void CLR(HD6301 cpu, ushort address)
		{
			cpu.Memory[address] = 0;

			cpu.N = false;
			cpu.Z = true;
			cpu.V = false;
			cpu.C = false;
		}
	}
}
