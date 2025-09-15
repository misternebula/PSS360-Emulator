namespace HD6301_Runner.Opcodes
{
    public static class BitControlOpcodes
    {
	    public static void CLC(HD6301 cpu)
	    {
		    cpu.C = false;
	    }

	    public static void CLI(HD6301 cpu)
	    {
		    cpu._I = false;
	    }

	    public static void CLV(HD6301 cpu)
	    {
		    cpu.V = false;
	    }

	    public static void SEC(HD6301 cpu)
	    {
		    cpu.C = true;
	    }

	    public static void SEI(HD6301 cpu)
	    {
		    cpu._I = true;
	    }

	    public static void SEV(HD6301 cpu)
	    {
		    cpu.V = true;
	    }
    }
}
