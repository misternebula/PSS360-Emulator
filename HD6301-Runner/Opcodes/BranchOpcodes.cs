namespace _6301_runner.Opcodes
{
    public static class BranchOpcodes
    {
	    public static void BRA()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];
		    var newAddress = Program.PC + relAddress;
		    Program.PC = (ushort)newAddress;
		}

	    public static void BCS()
	    {
			var relAddress = (sbyte)Program.Memory[Program.PC++];

			if (Program.C)
			{
				var newAddress = Program.PC + relAddress;
				Program.PC = (ushort)newAddress;
			}
		}

	    public static void BCC()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (!Program.C)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
	    }

		public static void BNE()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (Program.Z == false)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
		}

	    public static void BEQ()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (Program.Z)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
		}

	    public static void BLE()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (Program.Z || (Program.N ^ Program.V))
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
	    }

	    public static void BGT()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if ((Program.Z | (Program.N ^ Program.V)) == false)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
	    }

	    public static void BLS()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (Program.C | Program.Z)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
	    }

	    public static void BMI()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (Program.N)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
	    }

	    public static void BPL()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (!Program.N)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
	    }

	    public static void BGE()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if ((Program.N ^ Program.V) == false)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
		}

	    public static void BSR()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    Program.Memory[Program.SP] = Program.PC.GetLowByte();
			Program.SP--;
			Program.Memory[Program.SP] = Program.PC.GetHighByte();
			Program.SP--;

			var newAddress = Program.PC + relAddress;
			Program.PC = (ushort)newAddress;
		}

	    public static void BHI()
	    {
		    var relAddress = (sbyte)Program.Memory[Program.PC++];

		    if (!Program.C && !Program.Z)
		    {
			    var newAddress = Program.PC + relAddress;
			    Program.PC = (ushort)newAddress;
		    }
	    }

	    public static void BLT()
	    {
			var relAddress = (sbyte)Program.Memory[Program.PC++];

			if (Program.N ^ Program.V)
			{
				var newAddress = Program.PC + relAddress;
				Program.PC = (ushort)newAddress;
			}
		}
	}
}
