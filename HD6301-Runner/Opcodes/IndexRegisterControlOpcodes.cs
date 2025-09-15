namespace HD6301_Runner.Opcodes
{
    public static class IndexRegisterControlOpcodes
    {
	    public static void INX(HD6301 cpu)
	    {
		    cpu.X++;
		    cpu.Z = cpu.X == 0;
	    }
    }
}
