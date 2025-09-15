namespace HD6301_Runner.Opcodes
{
    public static class LogicalOperationOpcodes
    {
	    public static void TIM(HD6301 cpu, ushort immAddress, ushort address)
	    {
		    var im = cpu.Memory[immAddress];
		    var m = cpu.Memory[address];

		    var r = (byte)(im & m);

		    cpu.N = r.GetBit(7);
		    cpu.Z = r == 0;
		    cpu.V = false;
	    }
	}
}
