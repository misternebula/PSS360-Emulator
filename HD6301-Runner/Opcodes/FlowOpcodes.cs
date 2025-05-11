namespace _6301_runner.Opcodes
{
    public static class FlowOpcodes
    {
	    public static void JMP(ushort address)
	    {
		    Program.PC = address;
	    }

	    public static void JSR(ushort address)
	    {
			Program.Memory[Program.SP] = Program.PC.GetLowByte();
		    Program.SP--;
		    Program.Memory[Program.SP] = Program.PC.GetHighByte();
			Program.SP--;

			Program.PC = address;
	    }

	    public static void RTS()
	    {
		    Program.SP++;
		    var high = Program.Memory[Program.SP];
		    Program.SP++;
		    var low = Program.Memory[Program.SP];

			Program.PC = Program.CombineBytes(high, low);
		}

	    public static void RTI()
	    {
		    Program.SP++;
			Program.ConditionCodeRegister = Program.Memory[Program.SP];

			Program.SP++;
			Program.B = Program.Memory[Program.SP];

			Program.SP++;
			Program.A = Program.Memory[Program.SP];

			Program.SP++;
			var ixh = Program.Memory[Program.SP];
			Program.SP++;
			var ixl = Program.Memory[Program.SP];
			Program.X = Program.CombineBytes(ixh, ixl);

			Program.SP++;
			var pch = Program.Memory[Program.SP];
			Program.SP++;
			var pcl = Program.Memory[Program.SP];
			Program.PC = Program.CombineBytes(pch, pcl);

			//Console.WriteLine($"RTI");
		}
    }
}
