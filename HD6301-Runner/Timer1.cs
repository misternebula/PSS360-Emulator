using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD6301_Runner;
public class Timer1
{
	private HD6301 CPU;

	public Timer1(HD6301 cpu)
	{
		CPU = cpu;

		cpu.OnRegisterRead += OnRegisterRead;
		cpu.OnRegisterWrite += OnRegisterWrite;
	}

	public ushort FreeRunningCounter
	{
		get => CPU.CombineBytes(CPU.Ports.FRCH, CPU.Ports.FRCL);

		set
		{
			CPU.Ports.FRCH = value.GetHighByte();
			CPU.Ports.FRCL = value.GetLowByte();
		}
	}

	public ushort OutputCompareRegister1
	{
		get => CPU.CombineBytes(CPU.Ports.OCR1H, CPU.Ports.OCR1L);
		set
		{
			CPU.Ports.OCR1H = value.GetHighByte();
			CPU.Ports.OCR1L = value.GetLowByte();
		}
	}

	public ushort OutputCompareRegister2
	{
		get => CPU.CombineBytes(CPU.Ports.OCR2H, CPU.Ports.OCR2L);
		set
		{
			CPU.Ports.OCR2H = value.GetHighByte();
			CPU.Ports.OCR2L = value.GetLowByte();
		}
	}

	#region TCSR1

	public bool OutputLevel1
	{
		get => CPU.Ports.TCSR1.GetBit(0);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(0, value);
	}

	/// <summary>
	/// Whether the rising edge or falling edge of P20 will trigger data transfer from the counter to the ICR.
	/// 0 specifies falling edge, 1 specifies rising edge.
	/// Bit 0 of Port 2's DDR must be cleared for this function to operate.
	/// </summary>
	public bool InputEdge
	{
		get => CPU.Ports.TCSR1.GetBit(1);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(1, value);
	}

	/// <summary>
	/// Setting ETOI to 1 enables timer overflow interrupt (TOI) to trigger an internal interrupt (IRQ3).
	/// When ETOI is cleared, the interrupt is inhibited.
	/// </summary>
	public bool EnableTimerOverflowInterrupt
	{
		get => CPU.Ports.TCSR1.GetBit(2);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(2, value);
	}

	/// <summary>
	/// Setting EOCI1 to 1 enables output compare interrupt 1 (OCI1) to trigger an internal interrupt (IRQ3).
	/// When EOCI1 is cleared, the interrupt is inhibited.
	/// </summary>
	public bool EnableOutputCompareInterrupt1
	{
		get => CPU.Ports.TCSR1.GetBit(3);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(3, value);
	}

	/// <summary>
	/// Setting EICI to 1 enables input capture interrupt (ICI) to trigger an internal interrupt (IRQ3).
	/// When EICI is cleared, the interrupt is inhibited.
	/// </summary>
	public bool EnableInputCaptureInterrupt
	{
		get => CPU.Ports.TCSR1.GetBit(4);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(4, value);
	}

	/// <summary>
	/// TOF is set when the counter value increments from FFFF to 0000.
	/// TOF is cleared when the CPU reads the TCSR1, then the counter's upper byte at 0009.
	/// Read only.
	/// </summary>
	public bool TimerOverflowFlag
	{
		get => CPU.Ports.TCSR1.GetBit(5);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(5, value);
	}

	/// <summary>
	/// OCF1 is set when a match has occurred between the FRC and OCR1.
	/// Writing to OCR1 after reading the TCSR1 or TCSR2 clears OCF1.
	/// Read only.
	/// </summary>
	public bool OutputCompareFlag1
	{
		get => CPU.Ports.TCSR1.GetBit(6);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(6, value);
	}

	/// <summary>
	/// ICF is set when the transition of P20 input signal selected by IEDG causes the counter to transfer its data to the ICR.
	/// Reading the high byte of the ICR after reading TCSR1 or TCSR2 clears ICF.
	/// Read only.
	/// </summary>
	public bool InputCaptureFlag
	{
		get => CPU.Ports.TCSR1.GetBit(7);
		set => CPU.Ports.TCSR1 = CPU.Ports.TCSR1.SetBit(7, value);
	}

	#endregion

	#region TCSR2

	/// <summary>
	/// Setting OE1 to 1 enables OLVL1 to appear at P21 when a match has occurred between the counter and OCR1.
	/// Clearing OE1 makes P21 an I/O port.
	/// </summary>
	public bool OutputEnable1
	{
		get => CPU.Ports.TCSR2.GetBit(0);
		set => CPU.Ports.TCSR2 = CPU.Ports.TCSR2.SetBit(0, value);
	}

	/// <summary>
	/// Setting OE2 to 1 enables OLVL2 to appear at P25 when a match has occurred between the counter and OCR2.
	/// Clearing OE2 makes P25 an I/O port.
	/// </summary>
	public bool OutputEnable2
	{
		get => CPU.Ports.TCSR2.GetBit(1);
		set => CPU.Ports.TCSR2 = CPU.Ports.TCSR2.SetBit(1, value);
	}

	/// <summary>
	/// OLVL2 is transferred to P25 when a match occurs between the counter and OCR2.
	/// If OE2 is set to 1, OLVL2 will be output at P25.
	/// </summary>
	public bool OutputLevel2
	{
		get => CPU.Ports.TCSR2.GetBit(2);
		set => CPU.Ports.TCSR2 = CPU.Ports.TCSR2.SetBit(2, value);
	}

	/// <summary>
	/// Setting EOCI2 to 1 enables output compare interrupt 2 (OCI2) to trigger an internal interrupt (IRQ3).
	/// When EOCI2 is cleared, the interrupt is inhibited.
	/// </summary>
	public bool EnableOutputCompareInterrupt2
	{
		get => CPU.Ports.TCSR2.GetBit(3);
		set => CPU.Ports.TCSR2 = CPU.Ports.TCSR2.SetBit(3, value);
	}

	/// <summary>
	/// OCF2 is set when a match has occurred between the FRC and OCR2.
	/// Writing to OCR2 after reading TCSR2 clears OCF2.
	/// Read only.
	/// </summary>
	public bool OutputCompareFlag2
	{
		get => CPU.Ports.TCSR2.GetBit(5);
		set => CPU.Ports.TCSR2 = CPU.Ports.TCSR2.SetBit(5, value);
	}

	#endregion

	public void IncrementCounter(int amount)
	{
		for (var i = 0; i < amount; i++)
		{
			FreeRunningCounter++;
			CheckComparison();
			UpdateInterrupts();
		}
	}

	public void CheckComparison()
	{
		if (FreeRunningCounter == OutputCompareRegister1)
		{
			//Console.WriteLine($"FRC matches OCR1! ({OutputCompareRegister1})");
			OutputCompareFlag1 = true;
			if (OutputEnable1)
			{
				throw new NotImplementedException();
			}
		}

		if (FreeRunningCounter == OutputCompareRegister2)
		{
			//Console.WriteLine($"FRC matches OCR2! ({OutputCompareRegister2})");
			OutputCompareFlag2 = true;
			if (OutputEnable2)
			{
				throw new NotImplementedException();
			}
		}
	}

	public void UpdateInterrupts()
	{
		CPU.Interrupts.TOI = TimerOverflowFlag && EnableTimerOverflowInterrupt;
		CPU.Interrupts.ICI = EnableInputCaptureInterrupt && InputCaptureFlag;
		CPU.Interrupts.OCI = (EnableOutputCompareInterrupt1 && OutputCompareFlag1) || (EnableOutputCompareInterrupt2 && OutputCompareFlag2);
	}

	private bool TCSR1_Read;
	private bool TCSR2_Read;

	private void OnRegisterRead(RegisterAddress address)
	{
		if (InputCaptureFlag)
		{
			if (address == RegisterAddress.TCSR1)
			{
				TCSR1_Read = true;
			}

			if (address == RegisterAddress.TCSR2)
			{
				TCSR2_Read = true;
			}
		}

		if (OutputCompareFlag1)
		{
			if (address == RegisterAddress.TCSR1)
			{
				TCSR1_Read = true;
			}

			if (address == RegisterAddress.TCSR2)
			{
				TCSR2_Read = true;
			}
		}

		if (OutputCompareFlag2)
		{
			if (address == RegisterAddress.TCSR2)
			{
				TCSR2_Read = true;
			}
		}

		// TOF
	}

	private void OnRegisterWrite(RegisterAddress address)
	{
		var resetTCSR1 = false;
		var resetTCSR2 = false;

		if (InputCaptureFlag && (TCSR1_Read || TCSR2_Read))
		{
			if (address == RegisterAddress.ICRH)
			{
				InputCaptureFlag = false;
				resetTCSR1 = true;
				resetTCSR2 = true;
			}
		}

		if (OutputCompareFlag1 && (TCSR1_Read || TCSR2_Read))
		{
			if (address is RegisterAddress.OCR1H or RegisterAddress.OCR1L)
			{
				OutputCompareFlag1 = false;
				resetTCSR1 = true;
				resetTCSR2 = true;
			}
		}

		if (OutputCompareFlag2 && TCSR2_Read)
		{
			if (address is RegisterAddress.OCR2H or RegisterAddress.OCR2L)
			{
				OutputCompareFlag2 = false;
				resetTCSR2 = true;
			}
		}

		// TOF

		if (resetTCSR1)
		{
			TCSR1_Read = false;
		}

		if (resetTCSR2)
		{
			TCSR2_Read = false;
		}
	}
}