DtmfDetection
=============

Simple C# 6.0 implementation of the Goertzel algorithm to detect DTMF tones in audio data. Includes wrappers and extensions for NAudio.

NuGet Packages
--------------

https://www.nuget.org/packages/DtmfDetection/

https://www.nuget.org/packages/DtmfDetection.NAudio/

Usage example
-------------

How to print all DTMF tones in an WAV file:

``` C#
using (var waveFile = new WaveFileReader("dtmftest.wav"))
{
    foreach (var occurence in waveFile.DtmfTones())
    {
        Console.WriteLine($"{occurence.Position.TotalSeconds:00.000} s: "
                        + $"{occurence.DtmfTone.Key} key "
                        + $"(duration: {occurence.Duration.TotalSeconds:00.000} s)");
    }
}
```
