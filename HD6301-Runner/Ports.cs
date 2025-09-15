using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HD6301_Runner;

namespace HD6301_Runner
{
    public class Ports
    {
	    private HD6301 CPU;

	    public Ports(HD6301 cpu)
	    {
		    CPU = cpu;
	    }

	    public void Reset()
	    {
		    P1DDR = 0xFE;
		    P2DDR = 0x00;
			// port 1 = indefinite
			// port 2 = indefinite
			P3DDR = 0xFE;
			P4DDR = 0x00;
			// port 3 = indefinite
			// port 4 = indefinite
			TCSR1 = 0x00;
			FRCH = 0x00;
			FRCL = 0x00;
			OCR1H = 0xFF;
			OCR1L = 0xFF;
			CPU.Memory[0x000D] = 0x00; // ICRH
			CPU.Memory[0x000E] = 0x00; // ICRL
			TCSR2 = 0x10;
			RMCR = 0xC0;
			TRCSR1 = 0x20;
			CPU.Memory[0x0012] = 0x00; // RDR
			// tdr = indefinite
			// RP5CR = 0xF8 or 0x78 ?????????
			// port 5 = indefinite
			P6DDR = 0x00;
			// port 6 = indefinite
			// port 7 = indefinite
			OCR2H = 0xFF;
			OCR2L = 0xFF;
			TCSR3 = 0x20;
			TCONR = 0xFF;
			T2CNT = 0x00;
			TRCSR2 = 0x28;
			P5DDR = 0x00;
			P6CSR = 0x07;
	    }

		public byte P1DDR	{								set => CPU.Memory[0x0000] = value; } // Port 1 DDR 
		public byte P2DDR	{								set => CPU.Memory[0x0001] = value; } // Port 2 DDR
	    public byte PORT1	{ get => CPU.Memory[0x0002];	set => CPU.Memory[0x0002] = value; } // Port 1
	    public byte PORT2	{ get => CPU.Memory[0x0003];	set => CPU.Memory[0x0003] = value; } // Port 2
	    public byte P3DDR	{								set => CPU.Memory[0x0004] = value; } // Port 3 DDR
	    public byte P4DDR	{								set => CPU.Memory[0x0005] = value; } // Port 4 DDR
	    public byte PORT3	{ get => CPU.Memory[0x0006];	set => CPU.Memory[0x0006] = value; } // Port 3
	    public byte PORT4	{ get => CPU.Memory[0x0007];	set => CPU.Memory[0x0007] = value; } // Port 4
	    public byte TCSR1	{ get => CPU.Memory[0x0008];	set => CPU.Memory[0x0008] = value; } // Timer Control / Status Register 1
	    public byte FRCH	{ get => CPU.Memory[0x0009];	set => CPU.Memory[0x0009] = value; } // Free Running Counter (MSB)
	    public byte FRCL	{ get => CPU.Memory[0x000A];	set => CPU.Memory[0x000A] = value; } // Free Running Counter (LSB)
	    public byte OCR1H	{ get => CPU.Memory[0x000B];	set => CPU.Memory[0x000B] = value; } // Output Compare Register 1 (MSB)
	    public byte OCR1L	{ get => CPU.Memory[0x000C];	set => CPU.Memory[0x000C] = value; } // Output Compare Register 1 (LSB)
	    public byte ICRH	{ get => CPU.Memory[0x000D]; }										 // Input Capture Register (MSB)
		public byte ICRL	{ get => CPU.Memory[0x000E]; }										 // Input Capture Register (LSB)
	    public byte TCSR2	{ get => CPU.Memory[0x000F];	set => CPU.Memory[0x000F] = value; } // Timer Control / Status Register 2
	    public byte RMCR	{ get => CPU.Memory[0x0010];	set => CPU.Memory[0x0010] = value; } // Rate / Mode Control Register
	    public byte TRCSR1	{ get => CPU.Memory[0x0011];	set => CPU.Memory[0x0011] = value; } // Tx / Rx Control Status Register 1
	    public byte RDR		{ get => CPU.Memory[0x0012]; }										 // Receive Data Register
	    public byte TDR		{								set => CPU.Memory[0x0013] = value; } // Transmit Data Register
	    public byte RP5CR	{ get => CPU.Memory[0x0014];	set => CPU.Memory[0x0014] = value; } // RAM / Port 5 Control Register
	    public byte PORT5	{ get => CPU.Memory[0x0015];	set => CPU.Memory[0x0015] = value; } // Port 5
		public byte P6DDR	{								set => CPU.Memory[0x0016] = value; } // Port 6 DDR
		public byte PORT6	{ get => CPU.Memory[0x0017];	set => CPU.Memory[0x0017] = value; } // Port 6
		public byte PORT7	{ get => CPU.Memory[0x0018];	set => CPU.Memory[0x0018] = value; } // Port 7
		public byte OCR2H	{ get => CPU.Memory[0x0019];	set => CPU.Memory[0x0019] = value; } // Output Compare Register 2 (MSB)
	    public byte OCR2L	{ get => CPU.Memory[0x001A];	set => CPU.Memory[0x001A] = value; } // Output Compare Register 2 (LSB)
	    public byte TCSR3	{ get => CPU.Memory[0x001B];	set => CPU.Memory[0x001B] = value; } // Timer Control / Status Register 3
	    public byte TCONR	{ get => CPU.Memory[0x001C];	set => CPU.Memory[0x001C] = value; } // Time Constant Register
	    public byte T2CNT	{ get => CPU.Memory[0x001D];	set => CPU.Memory[0x001D] = value; } // Timer 2 Up Counter
	    public byte TRCSR2	{ get => CPU.Memory[0x001E];	set => CPU.Memory[0x001E] = value; } // Tx / Rx Control Status Register 2
	    public byte TSTREG	{ get => CPU.Memory[0x001F];	set => CPU.Memory[0x001F] = value; } // Test Register
	    public byte P5DDR	{								set => CPU.Memory[0x0020] = value; } // Port 5 DDR
		public byte P6CSR	{ get => CPU.Memory[0x0021];	set => CPU.Memory[0x0021] = value; } // Port 6 Control / Status Register

		private readonly RegisterAddress[] _portOffsets =
		[
			RegisterAddress.PORT1,
			RegisterAddress.PORT2,
			RegisterAddress.PORT3,
			RegisterAddress.PORT4,
			RegisterAddress.PORT5,
			RegisterAddress.PORT6,
			RegisterAddress.PORT7
		];

		public byte GetPortValue(int portNum)
		{
			return CPU.Memory[(int)_portOffsets[portNum - 1]];
		}

		public void SetPortValue(int portNum, byte val)
		{
			CPU.Memory[(int)_portOffsets[portNum - 1]] = val;
		}
	}

    public enum RegisterAddress : byte
    {
		P1DDR,
		P2DDR,
		PORT1,
		PORT2,
		P3DDR,
		P4DDR,
		PORT3,
		PORT4,
		TCSR1,
		FRCH,
		FRCL,
		OCR1H,
		OCR1L,
		ICRH,
		ICRL,
		TCSR2,
		RMCR,
		TRCSR1,
		RDR,
		TDR,
		RP5CR,
		PORT5,
		P6DDR,
		PORT6,
		PORT7,
		OCR2H,
		OCR2L,
		TCSR3,
		TCONR,
		T2CNT,
		TRCSR2,
		TSTREG,
		P5DDR,
		P6CSR
	}
}
