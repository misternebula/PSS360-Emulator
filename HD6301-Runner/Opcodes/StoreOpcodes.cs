namespace HD6301_Runner.Opcodes
{
    public static class StoreOpcodes
    {
	    public static void STX(HD6301 cpu, ushort address)
	    {
		    cpu.Memory[address] = cpu.X.GetHighByte();
		    cpu.Memory[address + 1] = cpu.X.GetLowByte();

		    cpu.N = cpu.X.GetBit(15);
		    cpu.Z = cpu.X == 0;
		    cpu.V = false;
	    }

	    public static void STS(HD6301 cpu, ushort address)
	    {
		    cpu.Memory[address] = cpu.SP.GetHighByte();
		    cpu.Memory[address + 1] = cpu.SP.GetLowByte();

		    cpu.N = cpu.SP.GetBit(15);
			cpu.Z = cpu.SP == 0;
		    cpu.V = false;
	    }

		public static void STD(HD6301 cpu, ushort address)
	    {
			if (address <= (byte)RegisterAddress.P6CSR)
			{
				// Storing to built in register.
				cpu.RegisterWrite((RegisterAddress)address);
			}

			cpu.Memory[address] = cpu.D.GetHighByte();
		    cpu.Memory[address + 1] = cpu.D.GetLowByte();

		    cpu.N = cpu.D.GetBit(15);
			cpu.Z = cpu.D == 0;
		    cpu.V = false;
		}

	    public static void STA(HD6301 cpu, Accumulator acc, ushort address)
	    {
		    var val = acc == Accumulator.A ? cpu.A : cpu.B;

			cpu.Memory[address] = val;

		    cpu.N = val.GetBit(7);
		    cpu.Z = val == 0;
		    cpu.V = false;
		}
	}

    public enum Accumulator { A, B }
}
