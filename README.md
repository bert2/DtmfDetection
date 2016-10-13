DtmfDetection
=============

Simple C# implementation of the Goertzel algorithm for DTMF tone (a.k.a. Touch-Tone) detection and localization in audio data. Includes wrappers and extensions for NAudio.

NuGet Packages
--------------

https://www.nuget.org/packages/DtmfDetection/

https://www.nuget.org/packages/DtmfDetection.NAudio/

Usage Examples (with NAudio)
----------------------------

How to print all DTMF tones in a WAV file:

``` C#
using System;
using NAudio.Wave;
using DtmfDetection.NAudio;

public static class Program
{
    public static void Main()
    {
		using (var waveFile = new WaveFileReader("dtmftest.wav"))
		{
			foreach (var occurence in waveFile.DtmfTones())
			{
				Console.WriteLine(
					$"{occurence.Position.TotalSeconds:00.000} s: "
					+ $"{occurence.DtmfTone.Key} key "
					+ $"(duration: {occurence.Duration.TotalSeconds:00.000} s)");
			}
		}
	}
}
```

How to print all DTMF tones in an MP3 file, but with each audio channel analyzed separately:

``` C#
using System;
using NAudio.Wave;
using DtmfDetection.NAudio;

public static class Program
{
    public static void Main()
    {
		using (var mp3File = new Mp3FileReader("dtmftest.mp3"))
		{
			foreach (var occurence in mp3File.DtmfTones(forceMono: false))
			{
				Console.WriteLine(
					$"{occurence.Position.TotalSeconds:00.000} s "
					+ $"({occurence.Channel}): "
					+ $"{occurence.DtmfTone.Key} key "
					+ $"(duration: {occurence.Duration.TotalSeconds:00.000} s)");
			}
		}
	}
}
```

How to detect DTMF tones in the current audio output:

``` C#
using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using DtmfDetection.NAudio;

public static class Program
{
	public static void Main()
	{
		using (var loopback = new WasapiLoopbackCapture { ShareMode = AudioClientShareMode.Shared })
		{
			var analyzer = new LiveAudioDtmfAnalyzer(loopback);
			analyzer.DtmfToneStarted += start => Console.WriteLine(
				$"{start.DtmfTone.Key} key started on {start.Position.TimeOfDay}");
			analyzer.DtmfToneStopped += end => Console.WriteLine(
				$"{end.DtmfTone.Key} key stopped after {end.Duration.TotalSeconds} s");

			analyzer.StartCapturing();
			Console.ReadKey(true);
			analyzer.StopCapturing();
		}
	}
}
```

How to detect DTMF tones through mic in, but with each audio channel analyzed separately:

``` C#
using System;
using NAudio.Wave;
using DtmfDetection.NAudio;

public static class Program
{
	public static void Main()
	{
		using (var micIn = new WaveInEvent { WaveFormat = new WaveFormat(8000, 32, 1) })
		{
			var analyzer = new LiveAudioDtmfAnalyzer(micIn, forceMono: false);
			analyzer.DtmfToneStarted += start => Console.WriteLine(
				$"{start.DtmfTone.Key} key started on {start.Position.TimeOfDay} (channel {start.Channel})");
			analyzer.DtmfToneStopped += end => Console.WriteLine(
				$"{end.DtmfTone.Key} key stopped after {end.Duration.TotalSeconds} s (channel {end.Channel})");

			analyzer.StartCapturing();
			Console.ReadKey(true);
			analyzer.StopCapturing();
		}
	}
}
```
