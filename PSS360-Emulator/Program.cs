using HD6301_Runner;
using NAudio.CoreAudioApi;
using OPL2;

namespace PSS360_Emulator;

internal class Program
{
	private static HD6301 CPU;
	private static OPL2.OPL2 OPL2;

	static void Main(string[] args)
	{
		Console.Write("Reading ROM... ");
		var rom = File.ReadAllBytes("rom.dat");
		Console.WriteLine("Done.");

		CPU = new(rom, 3.5795f);
		CPU.OnInstructionExecuted += InstructionExecuted;

		OPL2 = new();
		OPL2.Start();

		CPU.Start();

		Console.ReadLine();
	}

	private static void InstructionExecuted()
	{
		// max rythm and abc volume
		if (CPU.Ports.GetPortValue(6) == 0b11111110 || CPU.Ports.GetPortValue(6) == 0b11111101)
		{
			CPU.Ports.SetPortValue(2, 0b11101111);
		}
		else
		{
			CPU.Ports.SetPortValue(2, 0b11111111);
		}

		// press "demo" button
		if (CPU.Ports.GetPortValue(6) == 0b11011111)
		{
			CPU.Ports.SetPortValue(4, 0b11011111);
		}
		else
		{
			CPU.Ports.SetPortValue(4, 0b11111111);
		}

		/*if (CPU.Ports.GetPortValue(5) == 0b11110111)
		{
			CPU.Ports.SetPortValue(4, 0b11011111);
		}
		else
		{
			CPU.Ports.SetPortValue(4, 0b11111111);
		}*/

		PortWatcher();
	}

	private static byte OPL2Address;
	private static bool wasWriteAddressOperation;
	private static bool wasWriteDataOperation;

	private static bool wasDigitalSynth;
	private static bool wasProgramStartEnd;
	private static bool wasSynchroStart;

	private static byte TCSR1;
	private static byte OCR1H;
	private static byte OCR1L;
	private static byte TCSR2;
	private static byte RP5CR;
	private static byte OCR2H;
	private static byte OCR2L;
	private static byte TCSR3;
	private static byte TCONR;
	private static byte T2CNT;

	private static void PortWatcher()
	{
		/*if (TCSR1 != CPU.Ports.TCSR1)
		{
			Console.WriteLine($"TCSR1 changed to {CPU.Ports.TCSR1:b8}");
			TCSR1 = CPU.Ports.TCSR1;

			var olvl1 = TCSR1.GetBit(0);
			var iedg = TCSR1.GetBit(1);
			var etoi = TCSR1.GetBit(2);
			var eoci1 = TCSR1.GetBit(3);
			var eici = TCSR1.GetBit(4);
			var tof = TCSR1.GetBit(5);
			var ocf1 = TCSR1.GetBit(6);
			var icf = TCSR1.GetBit(7);

			Console.WriteLine($" - Output Level 1 - {olvl1}");
			Console.WriteLine($" - Input Edge - {iedg}");
			Console.WriteLine($" - Enable Timer Overflow Interrupt - {etoi}");
			Console.WriteLine($" - Enable Timer Output Compare Interrupt 1 - {eoci1}");
			Console.WriteLine($" - Enable Input Capture Interrupt - {eici}");
			Console.WriteLine($" - Timer Overflow Flag - {tof}"); // read only
			Console.WriteLine($" - Output Compare Flag 1 - {ocf1}"); // read only
			Console.WriteLine($" - Input Capture Flag - {icf}"); // read only
		}

		if (OCR1H != CPU.Ports.OCR1H)
		{
			Console.WriteLine($"OCR1H changed to {CPU.Ports.OCR1H:b8}");
			OCR1H = CPU.Ports.OCR1H;
		}

		if (OCR1L != CPU.Ports.OCR1L)
		{
			Console.WriteLine($"OCR1L changed to {CPU.Ports.OCR1L:b8}");
			OCR1L = CPU.Ports.OCR1L;
		}

		if (TCSR2 != CPU.Ports.TCSR2)
		{
			Console.WriteLine($"TCSR2 changed to {CPU.Ports.TCSR2:b8}");
			TCSR2 = CPU.Ports.TCSR2;

			var oe1 = TCSR2.GetBit(0);
			var oe2 = TCSR2.GetBit(1);
			var olvl2 = TCSR2.GetBit(2);
			var eoci2 = TCSR2.GetBit(3);
			var ocf2 = TCSR2.GetBit(5);
			var ocf1 = TCSR2.GetBit(6);
			var icf = TCSR2.GetBit(7);

			Console.WriteLine($" - Output Enable 1 - {oe1}");
			Console.WriteLine($" - Output Enable 2 - {oe2}");
			Console.WriteLine($" - Output Level 2 - {olvl2}");
			Console.WriteLine($" - Enable Output Compare Interrupt 2 - {eoci2}");
			Console.WriteLine($" - Output Compare Flag 2 - {ocf2}"); // read only
			Console.WriteLine($" - Output Compare Flag 1  - {ocf1}"); // read only
			Console.WriteLine($" - Input Capture Flag - {icf}"); // read only
		}

		if (RP5CR != CPU.Ports.RP5CR)
		{
			Console.WriteLine($"RP5CR changed to {CPU.Ports.RP5CR:b8}");
			RP5CR = CPU.Ports.RP5CR;

			var irq1e = RP5CR.GetBit(0);
			var irq2e = RP5CR.GetBit(1);
			var mre = RP5CR.GetBit(2);
			var hlte = RP5CR.GetBit(3);
			var amre = RP5CR.GetBit(4);
			var stbyflag = RP5CR.GetBit(5);
			var rame = RP5CR.GetBit(6);
			var stbypwr = RP5CR.GetBit(7);

			Console.WriteLine($" - IRQ1 Enable - {irq1e}");
			Console.WriteLine($" - IRG2 Enable - {irq2e}");
			Console.WriteLine($" - Memory Ready Enable - {mre}");
			Console.WriteLine($" - Halt Enable - {hlte}");
			Console.WriteLine($" - Auto Memory Ready Enable - {amre}");
			Console.WriteLine($" - Standby Flag  - {stbyflag}");
			Console.WriteLine($" - RAM Enable - {rame}");
			Console.WriteLine($" - Standby Power - {stbypwr}");
		}

		if (OCR2H != CPU.Ports.OCR2H)
		{
			Console.WriteLine($"OCR2H changed to {CPU.Ports.OCR2H:b8}");
			OCR2H = CPU.Ports.OCR2H;
		}

		if (OCR2L != CPU.Ports.OCR2L)
		{
			Console.WriteLine($"OCR2L changed to {CPU.Ports.OCR2L:b8}");
			OCR2L = CPU.Ports.OCR2L;
		}

		if (TCSR3 != CPU.Ports.TCSR3)
		{
			Console.WriteLine($"TCSR3 changed to {CPU.Ports.TCSR3:b8}");
			TCSR3 = CPU.Ports.TCSR3;

			var cks0 = TCSR3.GetBit(0);
			var cks1 = TCSR3.GetBit(1);
			var tos0 = TCSR3.GetBit(2);
			var tos1 = TCSR3.GetBit(3);
			var t2e = TCSR3.GetBit(4);
			var ecmi = TCSR3.GetBit(6);
			var cmf = TCSR3.GetBit(7);

			Console.WriteLine($" - Input Clock Select 0 - {cks0}");
			Console.WriteLine($" - Input Clock Select 1 - {cks1}");
			Console.WriteLine($" - Timer Output Select 0 - {tos0}");
			Console.WriteLine($" - Timer Output Select 1 - {tos1}");
			Console.WriteLine($" - Timer 2 Enable Bit - {t2e}");
			Console.WriteLine($" - Enable Counter Match Interrupt - {ecmi}");
			Console.WriteLine($" - Counter Match Flag - {cmf}"); // read only
		}

		if (TCONR != CPU.Ports.TCONR)
		{
			Console.WriteLine($"TCONR changed to {CPU.Ports.TCONR:b8}");
			TCONR = CPU.Ports.TCONR;
		}

		if (T2CNT != CPU.Ports.T2CNT)
		{
			Console.WriteLine($"T2CNT changed to {CPU.Ports.T2CNT:b8}");
			T2CNT = CPU.Ports.T2CNT;
		}*/

		var OPL2ChipSelect = CPU.Ports.GetPortValue(1).GetBit(2);
		var OPL2ReadEnable = CPU.Ports.GetPortValue(7).GetBit(0);
		var OPL2WriteEnable = CPU.Ports.GetPortValue(7).GetBit(1);
		var OPL2A0 = CPU.Ports.GetPortValue(1).GetBit(0);

		var writeAddressOperation = !OPL2ChipSelect && OPL2ReadEnable && !OPL2WriteEnable && !OPL2A0;
		var writeDataOperation = !OPL2ChipSelect && OPL2ReadEnable && !OPL2WriteEnable && OPL2A0;

		if (writeAddressOperation && !wasWriteAddressOperation)
		{
			OPL2Address = CPU.Ports.GetPortValue(3);
		}

		if (writeDataOperation && !wasWriteDataOperation)
		{
			OPL2.Write(OPL2Address, CPU.Ports.GetPortValue(3));
		}

		wasWriteAddressOperation = writeAddressOperation;
		wasWriteDataOperation = writeDataOperation;

		// LEDs are active when pin is low - acts as negative led pin
		var digitalSynth = !CPU.Ports.GetPortValue(2).GetBit(5);
		var programStartEnd = !CPU.Ports.GetPortValue(2).GetBit(6);
		var synchroStart = !CPU.Ports.GetPortValue(2).GetBit(7);

		if (!wasDigitalSynth && digitalSynth)
		{
			Console.WriteLine("Digital Synthesizer On/Off LED ENABLED");
		}
		else if (wasDigitalSynth && !digitalSynth)
		{
			Console.WriteLine("Digital Synthesizer On/Off LED DISABLED");
		}

		if (!wasProgramStartEnd && programStartEnd)
		{
			Console.WriteLine("Program Start/End LED ENABLED");
		}
		else if (wasProgramStartEnd && !programStartEnd)
		{
			Console.WriteLine("Program Start/End LED DISABLED");
		}

		if (!wasSynchroStart && synchroStart)
		{
			Console.WriteLine("Synchro Start LED ENABLED");
		}
		else if (wasSynchroStart && !synchroStart)
		{
			Console.WriteLine("Synchro Start LED DISABLED");
		}

		wasDigitalSynth = digitalSynth;
		wasProgramStartEnd = programStartEnd;
		wasSynchroStart = synchroStart;
	}
}
