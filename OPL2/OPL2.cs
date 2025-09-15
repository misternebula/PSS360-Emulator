using NAudio.Wave;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OPL2;

public class OPL2
{
	const int SAMPLE_RATE = 44100;
	const int BUFFER_SIZE = 1024;

	public void Start()
	{
		Console.WriteLine("OPL2 - Start");

		Console.WriteLine("OPL2 - adlib_init");
		adlib_init(SAMPLE_RATE);

		var waveFormat = new WaveFormat(SAMPLE_RATE, 16, 1);
		var bufferProvider = new BufferedWaveProvider(waveFormat)
		{
			BufferLength = SAMPLE_RATE * 2
		};

		var waveOut = new WaveOutEvent();
		waveOut.Init(bufferProvider);
		waveOut.Play();

		var thread = new Thread(() =>
		{
			var samples = new short[BUFFER_SIZE];
			var byteBuffer = new byte[BUFFER_SIZE * 2];

			var bufferDurationMs = BUFFER_SIZE * 1000.0 / SAMPLE_RATE;
			var sw = Stopwatch.StartNew();
			var nextTime = 0.0;

			while (true)
			{
				adlib_getsample(samples, BUFFER_SIZE);
				Buffer.BlockCopy(samples, 0, byteBuffer, 0, byteBuffer.Length);
				bufferProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);

				nextTime += bufferDurationMs;

				while (sw.Elapsed.TotalMilliseconds < nextTime)
				{
					Thread.SpinWait(50);
				}
			}
		});

		Console.WriteLine("OPL2 - Starting background thread...");
		thread.IsBackground = true;
		thread.Start();
	}

	public void Write(byte address, byte value)
	{
		//Console.WriteLine($"OPL2 - Write {value:x8} to {address:x8}");
		adlib_write(address, value);
	}

	[DllImport("WoodyOPL.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void adlib_init(UInt32 samplerate);

	[DllImport("WoodyOPL.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void adlib_getsample([Out] short[] sndptr, IntPtr numsamples);

	[DllImport("WoodyOPL.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void adlib_write(UIntPtr idx, byte val);
}