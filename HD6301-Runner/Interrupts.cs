using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD6301_Runner;

public class Interrupts
{
	private HD6301 CPU;

	public Interrupts(HD6301 cpu)
	{
		CPU = cpu;
	}

	public bool Reset;
	public bool IRQ1;
	public bool IRQ2;
	public bool ICI; // Timer 1 Input Capture
	public bool OCI; // Timer 1 Output Compare
	public bool TOI; // Timer 1 Overflow
	public bool CMI; // Timer 2 Counter Match
	public bool SIO;
	public bool TRAP;
	public bool SWI; // Software Interrupt
	public bool NMI;

	public bool MaskableInterruptRequested => !CPU._I && (IRQ1 || IRQ2 || ICI || OCI || TOI || CMI || SIO);
	public bool InterruptRequested => MaskableInterruptRequested || NMI || TRAP || SWI;

	public (ushort msb, ushort lsb) GetInterruptVector()
	{
		if (Reset)
		{
			Console.WriteLine("DEBUG");
			return (0xFFFE, 0xFFFF);
		}
		else if (TRAP)
		{
			Console.WriteLine("TRAP");
			return (0xFFEE, 0xFFEF);
		}
		else if (NMI)
		{
			Console.WriteLine("NMI");
			return (0xFFFC, 0xFFFD);
		}
		else if (SWI)
		{
			Console.WriteLine("SWI");
			return (0xFFFA, 0xFFFB);
		}
		else if (IRQ1)
		{
			Console.WriteLine("IRQ1");
			return (0xFFF8, 0xFFF9);
		}
		else if (ICI)
		{
			Console.WriteLine("ICI");
			return (0xFFF6, 0xFFF7);
		}
		else if (OCI)
		{
			return (0xFFF4, 0xFFF5);
		}
		else if (TOI)
		{
			Console.WriteLine("TOI");
			return (0xFFF2, 0xFFF3);
		}
		else if (CMI)
		{
			Console.WriteLine("CMI");
			return (0xFFEC, 0xFFED);
		}
		else if (IRQ2)
		{
			Console.WriteLine("IRQ2");
			return (0xFFEA, 0xFFEB);
		}
		else if (SIO)
		{
			Console.WriteLine("SIO");
			return (0xFFF0, 0xFFF1);
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}
