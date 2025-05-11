namespace _6301_runner.Opcodes
{
    public static class LoadOpcodes
    {
	    public static void LDX(ushort address)
	    {
		    var upper = Program.Memory[address];
		    var lower = Program.Memory[address + 1];
		    Program.X = Program.CombineBytes(upper, lower);

		    Program.N = Program.X.GetBit(7);
		    Program.Z = Program.X == 0;
		    Program.V = false;
	    }

	    public static void LDS(ushort address)
	    {
		    var upper = Program.Memory[address];
		    var lower = Program.Memory[address + 1];
		    Program.SP = Program.CombineBytes(upper, lower);

			Console.WriteLine($"Set Stack Pointer to {Program.SP:X4}");

		    Program.N = Program.SP.GetBit(7);
		    Program.Z = Program.SP == 0;
		    Program.V = false;
		}

	    public static void LDD(ushort address)
	    {
		    var upper = Program.Memory[address];
		    var lower = Program.Memory[address + 1];
		    Program.D = Program.CombineBytes(upper, lower);

		    Program.N = Program.D.GetBit(15);
		    Program.Z = Program.D == 0;
		    Program.V = false;
		}
    }
}
