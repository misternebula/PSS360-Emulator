namespace HD6301_Runner.Opcodes
{
    public static class LoadOpcodes
    {
	    public static void LDX(HD6301 cpu, ushort address)
	    {
		    var upper = cpu.Memory[address];
		    var lower = cpu.Memory[address + 1];
		    cpu.X = cpu.CombineBytes(upper, lower);

		    cpu.N = cpu.X.GetBit(7);
		    cpu.Z = cpu.X == 0;
		    cpu.V = false;
	    }

	    public static void LDS(HD6301 cpu, ushort address)
	    {
		    var upper = cpu.Memory[address];
		    var lower = cpu.Memory[address + 1];
		    cpu.SP = cpu.CombineBytes(upper, lower);

		    cpu.N = cpu.SP.GetBit(7);
		    cpu.Z = cpu.SP == 0;
		    cpu.V = false;
		}

	    public static void LDD(HD6301 cpu, ushort address)
	    {
		    var upper = cpu.Memory[address];
		    var lower = cpu.Memory[address + 1];
		    cpu.D = cpu.CombineBytes(upper, lower);

		    cpu.N = cpu.D.GetBit(15);
		    cpu.Z = cpu.D == 0;
		    cpu.V = false;
		}
    }
}
