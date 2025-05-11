namespace _6301_runner.Opcodes
{
    public static class LogicalOperationOpcodes
    {
	    public static void TIM(ushort immAddress, ushort address)
	    {
		    var im = Program.Memory[immAddress];
		    var m = Program.Memory[address];

		    var r = (byte)(im & m);

		    Program.N = r.GetBit(7);
		    Program.Z = r == 0;
		    Program.V = false;
	    }
	}
}
