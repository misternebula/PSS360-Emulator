using NAudio.Wave;
using System.Runtime.InteropServices;

namespace OPL2;

public class OPL2
{
	const int SAMPLE_RATE = 44100;
	const int BUFFER_SIZE = 1024;

	public void Start()
	{
		adlib_init(SAMPLE_RATE);

		var waveFormat = new WaveFormat(SAMPLE_RATE, 16, 1);
		var bufferProvider = new BufferedWaveProvider(waveFormat)
		{
			BufferLength = SAMPLE_RATE * 2,
			DiscardOnBufferOverflow = true
		};

		var waveOut = new WaveOutEvent();
		waveOut.Init(bufferProvider);
		waveOut.Play();

		var thread = new Thread(() =>
		{
			var samples = new short[BUFFER_SIZE];
			var byteBuffer = new byte[BUFFER_SIZE * 2];

			while (true)
			{
				adlib_getsample(samples, BUFFER_SIZE);
				Buffer.BlockCopy(samples, 0, byteBuffer, 0, byteBuffer.Length);
				bufferProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);
				Thread.Sleep(BUFFER_SIZE * 1000 / SAMPLE_RATE / 2);
			}
		});

		thread.IsBackground = true;
		thread.Start();
	}

	public void Write(byte address, byte value)
	{
		adlib_write(address, value);
	}

	[DllImport("WoodyOPL.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void adlib_init(UInt32 samplerate);

	[DllImport("WoodyOPL.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void adlib_getsample([Out] short[] sndptr, IntPtr numsamples);

	[DllImport("WoodyOPL.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void adlib_write(UIntPtr idx, byte val);
}