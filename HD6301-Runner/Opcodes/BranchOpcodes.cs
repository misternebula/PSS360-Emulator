namespace HD6301_Runner.Opcodes
{
    public static class BranchOpcodes
    {
	    public static void BRA(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];
		    var newAddress = cpu.PC + relAddress;
		    cpu.PC = (ushort)newAddress;
		}

	    public static void BCS(HD6301 cpu)
	    {
			var relAddress = (sbyte)cpu.Memory[cpu.PC++];

			if (cpu.C)
			{
				var newAddress = cpu.PC + relAddress;
				cpu.PC = (ushort)newAddress;
			}
		}

	    public static void BCC(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (!cpu.C)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
	    }

		public static void BNE(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (cpu.Z == false)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
		}

	    public static void BEQ(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (cpu.Z)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
		}

	    public static void BLE(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (cpu.Z || (cpu.N ^ cpu.V))
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
	    }

	    public static void BGT(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if ((cpu.Z | (cpu.N ^ cpu.V)) == false)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
	    }

	    public static void BLS(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (cpu.C | cpu.Z)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
	    }

	    public static void BMI(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (cpu.N)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
	    }

	    public static void BPL(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (!cpu.N)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
	    }

	    public static void BGE(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if ((cpu.N ^ cpu.V) == false)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
		}

	    public static void BSR(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    cpu.Memory[cpu.SP] = cpu.PC.GetLowByte();
			cpu.SP--;
			cpu.Memory[cpu.SP] = cpu.PC.GetHighByte();
			cpu.SP--;

			var newAddress = cpu.PC + relAddress;
			cpu.PC = (ushort)newAddress;
		}

	    public static void BHI(HD6301 cpu)
	    {
		    var relAddress = (sbyte)cpu.Memory[cpu.PC++];

		    if (!cpu.C && !cpu.Z)
		    {
			    var newAddress = cpu.PC + relAddress;
			    cpu.PC = (ushort)newAddress;
		    }
	    }

	    public static void BLT(HD6301 cpu)
	    {
			var relAddress = (sbyte)cpu.Memory[cpu.PC++];

			if (cpu.N ^ cpu.V)
			{
				var newAddress = cpu.PC + relAddress;
				cpu.PC = (ushort)newAddress;
			}
		}
	}
}
