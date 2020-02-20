# DtmfDetection & DtmfDetection.NAudio

[![Build status](https://ci.appveyor.com/api/projects/status/yxidl32tv632lagv/branch/master?svg=true)](https://ci.appveyor.com/project/bert2/dtmfdetection/branch/master) [![NuGet](https://img.shields.io/nuget/v/DtmfDetection.NAudio.svg)](https://www.nuget.org/packages/DtmfDetection.NAudio)

Implementation of the [Goertzel algorithm](https://en.wikipedia.org/wiki/Goertzel_algorithm) for the detection of [DTMF tones](https://en.wikipedia.org/wiki/Dual-tone_multi-frequency_signaling) (aka touch tones) in audio data. Install the package [DtmfDetection.NAudio](https://www.nuget.org/packages/DtmfDetection.NAudio) for integration with [NAudio](https://github.com/naudio/NAudio).

## Quick start

### With NAudio

How to detect and print DTMF changes (DTMF tone starting or stopping) in an [mp3 file](https://raw.githubusercontent.com/bert2/DtmfDetection/master/test/integration/testdata/long_dtmf_tones.mp3):

```csharp
using System;
using DtmfDetection.NAudio;
using NAudio.Wave;

class Program {
    static void Main() {
        using var audioFile = new AudioFileReader("long_dtmf_tones.mp3");
        var dtmfs = audioFile.DtmfChanges();
        foreach (var dtmf in dtmfs) Console.WriteLine(dtmf);
    }
}
```

<details><summary>Output</summary>

> 1 started @ 00:00:02.7675736 (ch: 0)\
1 stopped @ 00:00:05.5607029 (ch: 0)\
2 started @ 00:00:06.7138321 (ch: 0)\
2 stopped @ 00:00:06.8675736 (ch: 0)\
3 started @ 00:00:07.3031972 (ch: 0)\
3 stopped @ 00:00:07.4313378 (ch: 0)\
4 started @ 00:00:08.2000680 (ch: 0)\
4 stopped @ 00:00:10.5319501 (ch: 0)\
5 started @ 00:00:12.0950793 (ch: 0)\
5 stopped @ 00:00:12.2744444 (ch: 0)\
6 started @ 00:00:12.7357142 (ch: 0)\
6 stopped @ 00:00:12.8125850 (ch: 0)\
7 started @ 00:00:14.5038321 (ch: 0)\
7 stopped @ 00:00:14.5294557 (ch: 0)\
7 started @ 00:00:14.5550793 (ch: 0)\
7 stopped @ 00:00:16.8357142 (ch: 0)\
8 started @ 00:00:17.6813378 (ch: 0)\
8 stopped @ 00:00:17.7582086 (ch: 0)\
9 started @ 00:00:18.4500680 (ch: 0)\
9 stopped @ 00:00:18.5269614 (ch: 0)\
\# started @ 00:00:19.1163265 (ch: 0)\
\# stopped @ 00:00:19.1419501 (ch: 0)\
\# started @ 00:00:19.1675736 (ch: 0)\
\# stopped @ 00:00:19.3469614 (ch: 0)\
0 started @ 00:00:19.8338321 (ch: 0)\
0 stopped @ 00:00:19.8850793 (ch: 0)\
\* started @ 00:00:20.4744444 (ch: 0)\
\* stopped @ 00:00:20.6025850 (ch: 0)\
1 started @ 00:00:22.0119501 (ch: 0)\
1 stopped @ 00:00:23.7544444 (ch: 0)

</details>

How to detect and print multi-channel DTMF changes in a [wav file](https://raw.githubusercontent.com/bert2/DtmfDetection/master/test/integration/testdata/stereo_dtmf_tones.wav) while also merging the start and stop of each DTMF tone into a single data structure:

```csharp
using System;
using DtmfDetection;
using DtmfDetection.NAudio;
using NAudio.Wave;

class Program {
    static void Main() {
        using var audioFile = new AudioFileReader("stereo_dtmf_tones.wav");
        var dtmfs = audioFile.DtmfChanges(forceMono: false).ToDtmfTones();
        foreach (var dtmf in dtmfs) Console.WriteLine(dtmf);
    }
}
```

<details><summary>Output</summary>

> 1 @ 00:00:00 (len: 00:00:00.9994557, ch: 0)\
2 @ 00:00:01.9988208 (len: 00:00:00.9993878, ch: 1)\
3 @ 00:00:03.9975736 (len: 00:00:01.9987529, ch: 0)\
4 @ 00:00:04.9969614 (len: 00:00:01.9987528, ch: 1)\
5 @ 00:00:07.9950793 (len: 00:00:00.9993651, ch: 0)\
6 @ 00:00:07.9950793 (len: 00:00:00.9993651, ch: 1)\
7 @ 00:00:09.9938321 (len: 00:00:02.9981180, ch: 0)\
8 @ 00:00:11.0188208 (len: 00:00:00.9737642, ch: 1)\
9 @ 00:00:14.0169614 (len: 00:00:00.9737415, ch: 0)\
0 @ 00:00:15.0163265 (len: 00:00:00.9737415, ch: 0)

</details>

How to detect and print DTMF changes in audio output:

```csharp
using System;
using DtmfDetection.NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;

class Program {
    static void Main() {
        using var audioSource = new WasapiLoopbackCapture {
            ShareMode = AudioClientShareMode.Shared
        };

        using var analyzer = new BackgroundAnalyzer(audioSource);

        analyzer.OnDtmfDetected += dtmf => Console.WriteLine(dtmf);

        _ = Console.ReadKey(intercept: true);
    }
}
```

How to detect and print DTMF changes in microphone input while also lowering the detection threshold:

```csharp
using System;
using DtmfDetection;
using DtmfDetection.NAudio;
using NAudio.Wave;

class Program {
    static void Main() {
        using var audioSource = new WaveInEvent {
            WaveFormat = new WaveFormat(Config.Default.SampleRate, bits: 32, channels: 1)
        };

        using var analyzer = new BackgroundAnalyzer(
            audioSource,
            Config.Default.WithThreshold(10));

        analyzer.OnDtmfDetected += dtmf => Console.WriteLine(dtmf);

        _ = Console.ReadKey(intercept: true);
    }
}
```

### Without NAudio

How to detect and print DTMF tones in an array of [PCM](https://en.wikipedia.org/wiki/Pulse-code_modulation) samples:

### Pre-built example tool

## Configure the detector

### Adjust detection threshold

### Disable Goertzel response normalization

### Provide custom source of sample data

### Inject custom detector implementation

### Other configuration options

## Changelog

### 1.0.1

- remove unwanted dependencies from package DtmfDetection

### 1.0.0

- complete rewrite of previous implementation
- upgrade to netstandard2.1
- make implementation much more configurable
- normalize Goertzel response with total signal energy for loudness invariance
- improve performance by ~25%
- correctly calculate wait time until enough samples have been read when analyzing audio provided by a `NAudio.Wave.BufferedWaveProvider` stream

### 0.9.2

DtmfDetection:

- sync version with DtmfDetection.NAudio

DtmfDetection.NAudio:

- update NAudio reference to v1.8.4.0

### 0.9.1

DtmfDetection:

- update to .NET framework version 4.7
- reduce memory footprint a little bit

DtmfDetection.NAudio:

- update to .NET framework version 4.7

### 0.9.0

DtmfDetection:

- implement multi-channel support
- fix short DTMF tones not being detected
- adjust Goertzel algorithm implementation a bit

DtmfDetection.NAudio:

- fix mono-conversion (average all channels)

## TODO

- test with .net framework
- add XML documentation
- add XML documentation
- implement continuous deployment of CLI tool to choco via AppVeyor
- add config options to CLI tool
