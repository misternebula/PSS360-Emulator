namespace _6301_runner.Opcodes
{
    public static class BitControlOpcodes
    {
	    public static void CLC()
	    {
		    Program.C = false;
	    }

	    public static void CLI()
	    {
		    Program.I = false;
	    }

	    public static void CLV()
	    {
		    Program.V = false;
	    }

	    public static void SEC()
	    {
		    Program.C = true;
	    }

	    public static void SEI()
	    {
		    Program.I = true;
	    }

	    public static void SEV()
	    {
		    Program.V = true;
	    }
    }
}
