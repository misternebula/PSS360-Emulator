namespace _6301_runner.Opcodes
{
    public static class TransferOpcodes
    {
	    public static void LDA(Accumulator acc, ushort address)
	    {
		    var value = Program.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    Program.A = value;
		    }
		    else
		    {
			    Program.B = value;
		    }

		    Program.N = value.GetBit(7);
		    Program.Z = value == 0;
		    Program.V = false;
	    }
	}
}
