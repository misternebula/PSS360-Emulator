namespace _6301_runner.Opcodes
{
    public static class StoreOpcodes
    {
	    public static void STX(ushort address)
	    {
		    Program.Memory[address] = Program.X.GetHighByte();
		    Program.Memory[address + 1] = Program.X.GetLowByte();

		    Program.N = Program.X.GetBit(15);
		    Program.Z = Program.X == 0;
		    Program.V = false;
	    }

	    public static void STS(ushort address)
	    {
		    Program.Memory[address] = Program.SP.GetHighByte();
		    Program.Memory[address + 1] = Program.SP.GetLowByte();

		    Program.N = Program.SP.GetBit(15);
			Program.Z = Program.SP == 0;
		    Program.V = false;
	    }

		public static void STD(ushort address)
	    {
		    Program.Memory[address] = Program.D.GetHighByte();
		    Program.Memory[address + 1] = Program.D.GetLowByte();

		    Program.N = Program.D.GetBit(15);
			Program.Z = Program.D == 0;
		    Program.V = false;
		}

	    public static void STA(Accumulator acc, ushort address)
	    {
		    var val = acc == Accumulator.A ? Program.A : Program.B;

			Program.Memory[address] = val;

		    Program.N = val.GetBit(7);
		    Program.Z = val == 0;
		    Program.V = false;
		}
	}

    public enum Accumulator { A, B }
}
