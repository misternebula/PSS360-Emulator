namespace _6301_runner.Opcodes
{
    public class ArithmeticOperationOpcodes
    {
	    public static void ABA()
	    {
		    var r = (byte)(Program.A + Program.B);

		    Program.N = r.GetBit(7);
		    Program.Z = r == 0;

		    var a3 = Program.A.GetBit(3);
		    var b3 = Program.B.GetBit(3);
		    var r3 = r.GetBit(3);

		    var a7 = Program.A.GetBit(7);
		    var b7 = Program.B.GetBit(7);
		    var r7 = r.GetBit(7);

		    Program.V = a7 & b7 & !r7 | !a7 & !b7 & r7;
		    Program.C = a7 & b7 | b7 & !r7 | !r7 & a7;
		    Program.H = a3 & b3 | b3 & !r3 | !r3 & a3;

		    Program.A = r;
	    }

		public static void MUL()
	    {
		    Program.D = (ushort)(Program.A * Program.B);

		    Program.C = Program.D.GetBit(7);
	    }

		public static void CLR(Accumulator acc)
		{
			if (acc == Accumulator.A)
			{
				Program.A = 0;
			}
			else
			{
				Program.B = 0;
			}

			Program.N = false;
			Program.Z = true;
			Program.V = false;
			Program.C = false;
		}

		public static void CLR(ushort address)
		{
			Program.Memory[address] = 0;

			Program.N = false;
			Program.Z = true;
			Program.V = false;
			Program.C = false;
		}
	}
}
