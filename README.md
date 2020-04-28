# DtmfDetection & DtmfDetection.NAudio

[![build](https://img.shields.io/appveyor/build/bert2/dtmf-detection/master?logo=appveyor)](https://ci.appveyor.com/project/bert2/dtmf-detection/branch/master) [![tests](https://img.shields.io/appveyor/tests/bert2/dtmf-detection/master?compact_message&logo=appveyor)](https://ci.appveyor.com/project/bert2/dtmf-detection/branch/master) [![coverage](https://img.shields.io/codecov/c/github/bert2/DtmfDetection/master?logo=codecov)](https://codecov.io/gh/bert2/DtmfDetection) [![nuget package](https://img.shields.io/nuget/v/DtmfDetection.NAudio.svg?logo=nuget)](https://www.nuget.org/packages/DtmfDetection.NAudio) [![nuget downloads](https://img.shields.io/nuget/dt/DtmfDetection.NAudio?color=blue&logo=nuget)](https://www.nuget.org/packages/DtmfDetection.NAudio) ![last commit](https://img.shields.io/github/last-commit/bert2/DtmfDetection/master?logo=github)

Implementation of the [Goertzel algorithm](https://en.wikipedia.org/wiki/Goertzel_algorithm) for the detection of [DTMF tones](https://en.wikipedia.org/wiki/Dual-tone_multi-frequency_signaling) (aka touch tones) in audio data.

| package | use case |
|---|---|
| [DtmfDetection](https://www.nuget.org/packages/DtmfDetection) | Use this package if you are only working with raw [PCM](https://en.wikipedia.org/wiki/Pulse-code_modulation) data (i.e. arrays of `float`s). |
| [DtmfDetection.NAudio](https://www.nuget.org/packages/DtmfDetection.NAudio) | Integrates with [NAudio](https://github.com/naudio/NAudio) to detect DTMF tones in audio files and audio streams (e.g. mic-in or the current audio output). |

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

```csharp
using System;
using System.Linq;
using DtmfDetection;

using static DtmfDetection.DtmfGenerator;

class Program {
    static void Main() {
        var samples = GenerateStereoSamples();
        foreach (var dtmf in samples.DtmfChanges(channels: 2))
            Console.WriteLine(dtmf);
    }

    // `DtmfDetection.DtmfGenerator` has helpers for generating DTMF tones.
    static float[] GenerateStereoSamples() =>
        Stereo(
            left: Generate(PhoneKey.Star),
            right: Concat(Mark(PhoneKey.One, ms: 40), Space(ms: 20), Mark(PhoneKey.Two, ms: 40)))
        .Take(NumSamples(milliSeconds: 40 + 20 + 40, channels: 2))
        .ToArray();
}
```

<details><summary>Output</summary>

> \* started @ 00:00:00 (ch: 0)\
1 started @ 00:00:00 (ch: 1)\
1 stopped @ 00:00:00.0000026 (ch: 1)\
2 started @ 00:00:00.0000051 (ch: 1)\
\* stopped @ 00:00:00.0000100 (ch: 0)\
2 stopped @ 00:00:00.0000100 (ch: 1)

</details>

### Pre-built example tool

## DTMF tone localization accuracy

Be aware that this library cannot locate DTMF tones with 100% accuracy, because the detector analyzes the data in blocks of length ~26 ms with the default configuration. This block size determines the resolution of the localization and every DTMF tone starting position will be "rounded off" to the start of the nearest block.

For instance, if a DTMF tone starts at 35 ms into the audio, its calculated starting position will be around 26 ms, i.e. at the beginning the second block.

A resolution of 26 ms might seem rather inaccurate relative to the typical duration of a DTMF tone (40 ms). However, keep in mind that DTMF analysis typically is about correctly _detecting_ DTMF tones and not about accurately _locating_ them.

## Configuring the detector

The library is designed to be very configurable. Of course, each setting of the detector configuration can be changed. Additionally it is possible to replace any part of its logic with a custom implementation.

### Adjusting the detection threshold

The detector's threshold value is probably the setting that needs to be tweaked most often. Depending on the audio source and quality, the threshold might have to be increased to reduce false positives or decreased to reduce false negatives.

Typical values are between `30` and `35` with enabled [Goertzel response normalization](#disabling-goertzel-response-normalization) and `100` to `115` without it. Its default value is `30`.

Changing the threshold value is easy, because each of the three main entry points take an optional [`Config`](./src/DtmfDetection/Config.cs) argument (defaulting to `Config.Default`):

- `List<DtmfChange> float[].DtmfChanges(int, int, Config?)`
- `List<DtmfChange> WaveStream.DtmfChanges(bool, Config?)`
- `BackgroundAnalyzer(IWaveIn, bool, Action<DtmfChange>?, Config?, IAnalyzer?)`

Now, simply create your own `Config` instance and pass it to the entry point you want to use:

```csharp
var mycfg = new Config(threshold: 20, sampleBlockSize: ..., ...);
var dmtfs = waveStream.DtmfChanges(config: mycfg);
```

Or you start of with the default config and adjust it with one of its builder methods:

```csharp
var mycfg = Config.Default.WithThreshold(20);
```

### Disabling Goertzel response normalization

As of version 1.0.0 the frequency response calculated with Goertzel algorithm will be normalized with the total energy of the input signal. This effectively makes the detector invariant against changes in the loudness of the signal with very little additional computational costs.

You can test this yourself with a simple example program that detects DTMF tones in your system's current audio output, but with disabled response normalization:

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

        using var analyzer = new BackgroundAnalyzer(
            audioSource,
            config: Config.Default.WithNormalizeResponse(false));

        analyzer.OnDtmfDetected += dtmf => Console.WriteLine(dtmf);

        _ = Console.ReadKey(intercept: true);
    }
}
```

Now play any of the [test files](./test/integration/testdata) and observe the program's output. If your playback volume is high enough, DTMF tones should be detected. Try to gradually decrease the volume and see that no more DTMF tones will be detected as soon as the Goertzel responses fall below the detection threshold. With enabled response normalization all DTMF tones should be detected regardless of the volume level.

I generally recommend to leave response normalization enabled, because loudness invariance ensures that DTMF tones are detected correctly in a wider range of scenarios. However, if you are analyzing audio signals that feature strong background noises, you might accomplish better detection results by disabling response normalization.

Just note that without response normalization the detection threshold has to be significantly increased and depends on the loudness of your signal. A good starting point is a value of `100`.

### Providing a custom source of sample data

Different kinds of sample data are fed to the analysis in a unified way using the [`ISamples`](./src/DtmfDetection/Interfaces/ISamples.cs) interface. Currently there are three implementations of `ISamples`:

| implementation | package | usage |
|---|---|---|
| [`AudioData`](./src/DtmfDetection/AudioData.cs) | `DtmfDetection` | created from a `float[]` |
| [`AudioFile`](./src/DtmfDetection.NAudio/AudioFile.cs) | `DtmfDetection.NAudio` | created from an NAudio `WaveStream` |
| [`AudioStream`](./src/DtmfDetection.NAudio/AudioStream.cs) | `DtmfDetection.NAudio` | created from an NAudio `IWaveIn` |

In case none of the above implementations suits your needs, you can implement the interface yourself and pass it directly to the [`Analyzer`](./src/DtmfDetection/Analyzer.cs):

```csharp
// Untested `ISamples` implementation for `System.IO.Stream`s.
public class MySamples : ISamples, IDisposable {
    private readonly BinaryReader reader;
    private long position;
    public int Channels => 1;
    public int SampleRate => 8000;
    public TimeSpan Position => new TimeSpan((long)Math.Round(position * 1000.0 / SampleRate));
    public MySamples(Stream samples) => this.reader = new BinaryReader(samples);
    public int Read(float[] buffer, int count) {
        var safeCount = Math.Min(count, reader.BaseStream.Length / sizeof(float) - position);
        for (var i = 0; i < safeCount; i++, position++)
            buffer[i] = reader.ReadSingle();
        return (int)safeCount;
    }
    public void Dispose() => reader.Dispose();
}

// ...

var mySamples = new MySamples(myStream);
var analyzer = Analyzer.Create(mySamples, Config.Default);
var dtmfs = new List<DtmfChange>();

while (analyzer.MoreSamplesAvailable)
    dtmfs.AddRange(analyzer.AnalyzeNextBlock());
```

Refer to the [API reference](src/DtmfDetection/README.md#isamples-class) of the `ISamples` interface for more details on how to implement it correctly.


### Injecting a custom detector implementation

### Other configuration options

## API reference

DtmfDetection: [./src/DtmfDetection/README.md](./src/DtmfDetection/README.md)

DtmfDetection.NAudio: [./src/DtmfDetection.NAudio/README.md](./src/DtmfDetection.NAudio/README.md)

## Changelog

### 1.2.2

- refactor wait time estimation of the `BackgroundAnalyzer` to a less handcrafted solution

### 1.2.1

- `BackgroundAnalyzer` ctor now also takes a handler for the `OnDtmfDetected` event

### 1.2.0

- add XML documentation
- generate API reference

### 1.1.0

DtmfDetection:

- add extension method for analyzing `float` arrays
- add helpers for generating DTMF tones

### 1.0.1

DtmfDetection:

- remove unwanted dependencies from nuget package

### 1.0.0

- upgrade to netstandard2.1
- make implementation much more configurable
- improve runtime performance by ~25%

DtmfDetection:

- normalize Goertzel response with total signal energy for loudness invariance

DtmfDetection.NAudio:

- update NAudio reference to 1.10.0
- correctly calculate wait time until enough samples can be read when analyzing audio provided by a `NAudio.Wave.BufferedWaveProvider` stream

### 0.9.2

DtmfDetection.NAudio:

- update NAudio reference to 1.8.4.0

### 0.9.1

DtmfDetection:

- update to .NET framework 4.7
- reduce memory footprint a little bit

DtmfDetection.NAudio:

- update to .NET framework 4.7

### 0.9.0

DtmfDetection:

- implement multi-channel support
- fix short DTMF tones not being detected
- adjust Goertzel algorithm implementation a bit

DtmfDetection.NAudio:

- fix mono-conversion (average all channels)

## TODO

- finish README
- continuous deployment of CLI tool to choco
- add config options to CLI tool
