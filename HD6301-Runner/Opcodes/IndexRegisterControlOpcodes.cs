namespace _6301_runner.Opcodes
{
    public static class IndexRegisterControlOpcodes
    {
	    public static void INX()
	    {
		    Program.X++;
		    Program.Z = Program.X == 0;
	    }
    }
}
