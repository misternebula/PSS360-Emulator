namespace HD6301_Runner.Opcodes
{
    public static class TransferOpcodes
    {
	    public static void LDA(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    if (address <= (byte)RegisterAddress.P6CSR)
		    {
				// Reading from built in register.
				cpu.RegisterRead((RegisterAddress)address);
		    }

		    var value = cpu.Memory[address];

		    if (acc == Accumulator.A)
		    {
			    cpu.A = value;
		    }
		    else
		    {
			    cpu.B = value;
		    }

		    cpu.N = value.GetBit(7);
		    cpu.Z = value == 0;
		    cpu.V = false;
	    }
	}
}
