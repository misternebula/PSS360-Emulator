namespace HD6301_Runner.Opcodes
{
    public static class FlowOpcodes
    {
	    public static void JMP(HD6301 cpu, ushort address)
	    {
		    cpu.PC = address;
	    }

	    public static void JSR(HD6301 cpu, ushort address)
	    {
		    cpu.Memory[cpu.SP] = cpu.PC.GetLowByte();
		    cpu.SP--;
		    cpu.Memory[cpu.SP] = cpu.PC.GetHighByte();
		    cpu.SP--;

		    cpu.PC = address;
	    }

	    public static void RTS(HD6301 cpu)
	    {
		    cpu.SP++;
		    var high = cpu.Memory[cpu.SP];
		    cpu.SP++;
		    var low = cpu.Memory[cpu.SP];

		    cpu.PC = cpu.CombineBytes(high, low);
		}

	    public static void RTI(HD6301 cpu)
	    {
		    cpu.SP++;
			cpu.CCR = cpu.Memory[cpu.SP];

			cpu.SP++;
			cpu.B = cpu.Memory[cpu.SP];

			cpu.SP++;
			cpu.A = cpu.Memory[cpu.SP];

			cpu.SP++;
			var ixh = cpu.Memory[cpu.SP];
			cpu.SP++;
			var ixl = cpu.Memory[cpu.SP];
			cpu.X = cpu.CombineBytes(ixh, ixl);

			cpu.SP++;
			var pch = cpu.Memory[cpu.SP];
			cpu.SP++;
			var pcl = cpu.Memory[cpu.SP];
			cpu.PC = cpu.CombineBytes(pch, pcl);
		}
    }
}
