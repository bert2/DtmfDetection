# DtmfDetection 1.2.0 API documentation

Created by [mddox](https://github.com/loxsmoke/mddox) on 24/02/2020

# All types

|   |   |   |
|---|---|---|
| [Analyzer Class](#analyzer-class) | [DtmfGenerator Class](#dtmfgenerator-class) | [ToDtmfTonesExt Class](#todtmftonesext-class) |
| [AudioData Class](#audiodata-class) | [DtmfTone Class](#dtmftone-class) | [Utils Class](#utils-class) |
| [Config Class](#config-class) | [FloatArrayExt Class](#floatarrayext-class) | [IAnalyzer Class](#ianalyzer-class) |
| [Detector Class](#detector-class) | [Goertzel Class](#goertzel-class) | [IDetector Class](#idetector-class) |
| [DtmfChange Class](#dtmfchange-class) | [PhoneKey Enum](#phonekey-enum) | [ISamples Class](#isamples-class) |

# Analyzer Class

Namespace: DtmfDetection

The `Analyzer` reads sample blocks of size `Config.SampleBlockSize * ISamples.Channels` from the input sample data and feeds each sample block to its `IDetector` every time `AnalyzeNextBlock()` is called.

An internal state machine is used to skip redundant reports of the same DTMF tone detected in consecutive sample blocks. Instead only the starting and stop position of each DMTF tone will be reported.

When no more samples are available the property `MoreSamplesAvailable` will be set to `false` and the analysis is considered finished (i.e. subsequent calls to `AnalyzeNextBlock()` should be avoided as they might fail or result in undefined behavior, depending on the `ISamples` implementation.

## Properties

| Name | Type | Summary |
|---|---|---|
| **MoreSamplesAvailable** | bool | Indicates whether there is more data to analyze. `AnalyzeNextBlock()` should not be called when this is `false`. Is `true` initially and turns `false` as soon as `ISamples.Read()` returned a number less than `Config.SampleBlockSize`. |
## Constructors

| Name | Summary |
|---|---|
| **Analyzer([ISamples](#isamples-class) samples, [IDetector](#idetector-class) detector)** | Creates a new `Analyzer` that will feed the given sample data to the given `IDetector`. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **AnalyzeNextBlock()** | IList\<[DtmfChange](#dtmfchange-class)\> | Tries to read `Config.SampleBlockSize * ISamples.Channels` samples from the input data and runs DTMF detection on that sample block. Should only be called when `MoreSamplesAvailable` is true. |
| **Create([ISamples](#isamples-class) samples, [IDetector](#idetector-class) detector)** | [Analyzer](#analyzer-class) | Creates a new `Analyzer` that will feed the given sample data to the given `IDetector`. |
| **Create([ISamples](#isamples-class) samples, [Config](#config-class) config)** | [Analyzer](#analyzer-class) | Creates a new `Analyzer` using a self-created instance of `Detector` to feed the given sample data to it. |
# AudioData Class

Namespace: DtmfDetection

Convenience implementation of `ISamples` for PCM audio data.
            PCM data is usually represented as an array of `float`s.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Channels** | int | Returns the number of channels this `AudioData` has been created with. |
| **SampleRate** | int | Returns the sample rate this `AudioData` has been created with. |
| **Position** | TimeSpan | Calculates and returns the current position in the PCM data. |
## Constructors

| Name | Summary |
|---|---|
| **AudioData(float[] samples, int channels, int sampleRate)** | Creates a new `AudioData` from the given array of `float` values which were sampled with the given sample rate and for the given number of channels. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Read(float[] buffer, int count)** | int | Reads `count` samples from the input and writes them into `buffer`. Because the input PCM data already has the expected format, this boils down to a simple call to `Array.Copy()`. |
# Config Class

Namespace: DtmfDetection

The detetor configuration.

## Constructors

| Name | Summary |
|---|---|
| **Config(double threshold, int sampleBlockSize, int sampleRate, bool normalizeResponse)** | Creates a new `Config` instance. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Equals([Config](#config-class) other)** | bool | Indicates whether the current `Config` is equal to another `Config`. |
| **Equals(object obj)** | bool | Indicates whether this `Config` and a specified object are equal. |
| **GetHashCode()** | int | Returns the hash code for this `Config`. |
| **WithNormalizeResponse(bool normalizeResponse)** | [Config](#config-class) | Creates a cloned `Config` instance from this instance, but with a new `NormalizeResponse` setting. |
| **WithSampleBlockSize(int sampleBlockSize)** | [Config](#config-class) | Creates a cloned `Config` instance from this instance, but with a new `SampleBlockSize` setting. |
| **WithSampleRate(int sampleRate)** | [Config](#config-class) | Creates a cloned `Config` instance from this instance, but with a new `SampleRate` setting. |
| **WithThreshold(double threshold)** | [Config](#config-class) | Creates a cloned `Config` instance from this instance, but with a new `Threshold` setting. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **Threshold** | double | The detection threshold. Typical values are `30`-`35` (when `NormalizeResponse` is `true`) and `100`-`115` (when `normalizeResponse` is `false`). |
| **SampleBlockSize** | int | The number of samples to analyze before the Goertzel response should be calulated. It is recommened to leave it at the default value `205` (tuned to minimize error of the target frequency bin). |
| **SampleRate** | int | The sample rate (in Hz) the Goertzel algorithm expects. Sources with higher samples rates must resampled to this sample rate. It is recommended to leave it at the default value `8000`. |
| **NormalizeResponse** | bool | Toggles normalization of the Goertzel response with the total signal energy of the sample block. Recommended |
| **Default** | [Config](#config-class) | A default configuration instance. |
| **DefaultThreshold** | double | The default detection threshold (tuned to normalized responses). |
| **DefaultSampleBlockSize** | int | The default number of samples to analyze before the Goertzel response should be calulated (tuned to minimize error of the target frequency bin). |
| **DefaultSampleRate** | int | Default rate (in Hz) at which the analyzed samples are expected to have been measured. |

# Detector Class

Namespace: DtmfDetection

Creates a `Goertzel` accumulator for each of the DTMF tone low (697, 770, 852, and 941 Hz) and high frequencies (1209, 1336, 1477, and 1633 Hz) and repeats that for each audio channel in the input data.

When `Detect()` is called, each sample of the input sample block is added to each `Goertzel` accumulator and afterwards the Goertzel response of each frequency is retrieved.

Reports a detected DTMF tone when:
- exactly one of the four low frequency responses crosses the detection threshold, and
- exactly one of the four high frequency responses crosses the detection threshold.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Channels** | int | The number of channels this detector has been created for. Used by the `Analyzer` to validate that this detector supports the number of channels present int the source data (`ISamples.Channels`). |
| **Config** | [Config](#config-class) | The `Config` this detector has been created with. |
## Constructors

| Name | Summary |
|---|---|
| **Detector(int channels, [Config](#config-class) config)** | Creates a new `Detector` for the given number of audio channels and with the given dector config. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Detect(ReadOnlySpan\<float\> sampleBlock)** | IReadOnlyList\<[PhoneKey](#phonekey-enum)\> | Runs the Goertzel algorithm on all samples in `sampleBlock` and returns the DTMF key detected in each channel. |
# DtmfChange Class

Namespace: DtmfDetection

Represents the start or a stop of a DTMF tone in audio data.

## Properties

| Name | Type | Summary |
|---|---|---|
| **IsStop** | bool | Indicates whether a DMTF tone started or stopped at the current position. |
## Constructors

| Name | Summary |
|---|---|
| **DtmfChange([PhoneKey](#phonekey-enum) key, TimeSpan position, int channel, bool isStart)** | Creates a new `DtmfChange` with the given identification and location. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Equals([DtmfChange](#dtmfchange-class) other)** | bool | Indicates whether the current `DtmfChange` is equal to another `DtmfChange`. |
| **Equals(Object obj)** | bool | Indicates whether this `DtmfChange` and a specified object are equal. |
| **GetHashCode()** | int | Returns the hash code for this `DtmfChange`. |
| **Start([PhoneKey](#phonekey-enum) key, TimeSpan position, int channel)** | [DtmfChange](#dtmfchange-class) | Creates a new `DtmfChange` that marks the start of a DTMF tone at the specified location. |
| **Stop([PhoneKey](#phonekey-enum) key, TimeSpan position, int channel)** | [DtmfChange](#dtmfchange-class) | Creates a new `DtmfChange` that marks the end of a DTMF tone at the specified location. |
| **ToString()** | string | Prints the identification and location of this `DtmfChange` to a `string` and returns it. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **Key** | [PhoneKey](#phonekey-enum) | The key of the DTMF tone that changed. |
| **Position** | TimeSpan | The position inside the audio data where the change was detected. |
| **Channel** | int | The audio channel where the change was detected. |
| **IsStart** | bool | Indicates whether a DMTF tone started or stopped at the current position. |

# DtmfGenerator Class

Namespace: DtmfDetection

Provides helpers to generate DTMF tones.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **Add(IEnumerable\<float\> xs, IEnumerable\<float\> ys)** | IEnumerable\<float\> | Adds two sequences of PCM data together. Used to generate dual tones. The amplitude might exceed the range `[-1..1]` after adding. |
| **AsSamples(IEnumerable\<float\> source, int channels, int sampleRate)** | [ISamples](#isamples-class) | Creates an `AudioData` instance from a sequence of PCM samples. |
| **Concat(IEnumerable\<float\>[] xss)** | IEnumerable\<float\> | Concatenates multiple finite sequences of PCM data. Typically used with `Mark()` and `Space()`.  |
| **Constant(float amplitude)** | IEnumerable\<float\> | Generates a constant PCM signal of infinite length. |
| **Generate([PhoneKey](#phonekey-enum) key, int sampleRate)** | IEnumerable\<float\> | Generates single-channel PCM data playing the DTMF tone `key` infinitely. |
| **Generate((int highFreq, int lowFreq) dual, int sampleRate)** | IEnumerable\<float\> | Generates single-channel PCM data playing the dual tone comprised of the two frequencies `highFreq` and `lowFreq` infinitely. |
| **Generate(int highFreq, int lowFreq, int sampleRate)** | IEnumerable\<float\> | Generates single-channel PCM data playing the dual tone comprised of the two frequencies `highFreq` and `lowFreq` infinitely. |
| **Mark([PhoneKey](#phonekey-enum) key, int ms, int sampleRate)** | IEnumerable\<float\> | Generates single-channel PCM data playing the DTMF tone `key` for the specified length `ms`. |
| **Noise(float amplitude)** | IEnumerable\<float\> | Generates an infinite PCM signal of pseudo-random white noise. |
| **Normalize(IEnumerable\<float\> source, float maxAmplitude)** | IEnumerable\<float\> | Normlizes a signal with the given `maxAmplitude`. |
| **NumSamples(int milliSeconds, int channels, int sampleRate)** | int | Converts a duration in milliseconds into the number of samples required to represent a signal of that duration as PCM audio data. |
| **Sine(int freq, int sampleRate, float amplitude)** | IEnumerable\<float\> | Generates a sinusoidal PCM signal of infinite length for the specified frequency. |
| **Space(int ms, int sampleRate)** | IEnumerable\<float\> | Generates single-channel PCM data playing silence for the specified length `ms`. |
| **Stereo(IEnumerable\<float\> left, IEnumerable\<float\> right)** | IEnumerable\<float\> | Takes two sequences of single-channel PCM data and interleaves them to form a single sequence of dual-channel PCM data. |
# DtmfTone Class

Namespace: DtmfDetection

Represents a DTMF tone in audio data.

## Constructors

| Name | Summary |
|---|---|
| **DtmfTone([PhoneKey](#phonekey-enum) key, TimeSpan position, TimeSpan duration, int channel)** | Creates a new `DtmfTone` with the given identification and location. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Equals([DtmfTone](#dtmftone-class) other)** | bool | Indicates whether the current `DtmfTone` is equal to another `DtmfTone`. |
| **Equals(Object obj)** | bool | Indicates whether this `DtmfTone` and a specified object are equal. |
| **From([DtmfChange](#dtmfchange-class) start, [DtmfChange](#dtmfchange-class) stop)** | [DtmfTone](#dtmftone-class) | Creates a new `DtmfTone` from two `DtmfChange`s representing the start and end of the same tone. |
| **GetHashCode()** | int | Returns the hash code for this `DtmfTone`. |
| **ToString()** | string | Prints the identification and location of this `DtmfTone` to a `string` and returns it. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **Key** | [PhoneKey](#phonekey-enum) | The key of the DTMF tone. |
| **Position** | TimeSpan | The position inside the audio data where the DTMF tone was detected. |
| **Duration** | TimeSpan | The length of the DTMF tone inside the audio data. |
| **Channel** | int | The audio channel where the DTMF tone was detected. |
# FloatArrayExt Class

Namespace: DtmfDetection

Provides an extension method to detect DTMF tones in PCM audio data.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **DtmfChanges(float[] samples, int channels, int sampleRate, [Config?](#config-class) config)** | List\<[DtmfChange](#dtmfchange-class)\> | Detects DTMF tones in an array of `float`s. |
# Goertzel Class

Namespace: DtmfDetection

The actual implementation of the Goertzel algorithm (https://en.wikipedia.org/wiki/Goertzel_algorithm) that estimates
            the strength of a frequency in a signal.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Response** | double | Calculates and returns the estimated strength of the frequency in the samples given so far. |
| **NormResponse** | double | Calculates `Response`, but normalized with the total signal energy, which achieves loudness invariance. |
## Constructors

| Name | Summary |
|---|---|
| **Goertzel(double c, double s1, double s2, double e)** | Used to create a new `Goertzel` from the values of a previous one. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **AddSample(float sample)** | [Goertzel](#goertzel-class) | Adds a new sample to this `Goertzel` and returns a new one created from the previous `Goertzel` values and the sample. |
| **Equals([Goertzel](#goertzel-class) other)** | bool | Indicates whether the current `Goertzel` is equal to another `Goertzel`. |
| **Equals(Object obj)** | bool | Indicates whether this `Goertzel` and a specified object are equal. |
| **GetHashCode()** | int | Returns the hash code for this `Goertzel`. |
| **Init(int targetFreq, int sampleRate, int numSamples)** | [Goertzel](#goertzel-class) | Initializes a `Goertzel` for a given target frequency. |
| **Reset()** | [Goertzel](#goertzel-class) | Creates a new `Goertzel` from this one's coefficient `C`, but resets the state (`S1`, `S2`) and the total signal |
| **ToString()** | string | Prints the value of `NormResponse` to a `string` and returns it. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **C** | double | Stores a pre-computed coefficient calculated from the parameters of `Init()`. |
| **S1** | double | Stores the state of the `Goertzel`. Used to determine the strength of the target frequency in the signal. |
| **S2** | double | Stores the state of the `Goertzel`. Used to determine the strength of the target frequency in the signal. |
| **E** | double | Accumulates the total signal energy of the signal. Used for normalization. |
# PhoneKey Enum

Namespace: DtmfDetection


## Values

| Name | Summary |
|---|---|
| **Zero** | Key '0' |
| **One** | Key '1' |
| **Two** | Key '2' |
| **Three** | Key '3' |
| **Four** | Key '4' |
| **Five** | Key '5' |
| **Six** | Key '6' |
| **Seven** | Key '7' |
| **Eight** | Key '8' |
| **Nine** | Key '9' |
| **Star** | Key '*' |
| **Hash** | Key '#' |
| **A** | Key 'A' |
| **B** | Key 'B' |
| **C** | Key 'C' |
| **D** | Key 'D' |
| **None** | Used to represent the absence of any DTMF tones. |
# ToDtmfTonesExt Class

Namespace: DtmfDetection

Provides helpers to generate a sequence of `DtmfTone`s from a list of `DtmfChange`s.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **FindMatchingStop(IList\<[DtmfChange](#dtmfchange-class)\> dtmfs, int offset, [DtmfChange](#dtmfchange-class) start)** | [DtmfChange](#dtmfchange-class) | Finds the stop of a DTMF tone matching the given start of a DTMF tone in a list of `DtmfChange`s. A `DtmfChange x` matches `start` when `x.IsStop == true`, `x.Channel == start.Channel`, `x.Key == start.Key`, and `x.Position >= start.Position` |
| **ToDtmfTones(IList\<[DtmfChange](#dtmfchange-class)\> dtmfs)** | IEnumerable\<[DtmfTone](#dtmftone-class)\> | Converts a list of `DtmfChange`s to a sequence of `DtmfTone`s by finding the matching stop of a DTMF tones to each start of a DTMF tone and merging both into a single `DtmfTone` struct. |
# Utils Class

Namespace: DtmfDetection

Provides helpers to convert between `PhoneKey`s and their corresponding frequency encodings.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **PhoneKeys()** | IEnumerable\<[PhoneKey](#phonekey-enum)\> | Enumerates all `PhoneKey`s except `PhoneKey.None`. |
| **ToDtmfTone([PhoneKey](#phonekey-enum) key)** | (int high, int low) | Converts a `PhoneKey` to the two frequencies it is encoded with in audio data. |
| **ToPhoneKey((int high, int low) dtmfTone)** | [PhoneKey](#phonekey-enum) | Converts a frequency tuple to a `PhoneKey`. |
| **ToSymbol([PhoneKey](#phonekey-enum) key)** | char | Converts a `PhoneKey` to its UTF-8 symbol. |
# IAnalyzer Class

Namespace: DtmfDetection.Interfaces

Interface to decouple the `BackgroundAnalyzer` from the `Analyzer` it is using by default. Use this if you want to inject your own analyzer into the `BackgroundAnalyzer`. Feel free to start by copying the original `Analyzer` and adjust it to your needs.

## Properties

| Name | Type | Summary |
|---|---|---|
| **MoreSamplesAvailable** | bool | Indicates whether there is more data to analyze. Should always be `true` initially and once it turned `false`, it should never turn back to `true` again. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **AnalyzeNextBlock()** | IList\<[DtmfChange](#dtmfchange-class)\> | Analyzes the next block of samples. The size of the analyzed block should match `Config.SampleBlockSize` multiplied by the number of channels in the sample data. This might throw when called while `MoreSamplesAvailable` is `false`. |
# IDetector Class

Namespace: DtmfDetection.Interfaces

Interface to decouple the `Analyzer` from the `Detector` it is using by default. Use this if you want to inject your own detector into the `Analyzer`. Feel free to start by copying the original `Detector` and adjust it to your needs.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Channels** | int | The number of channels this detector has been created for. Used by the `Analyzer` to validate that this detector supports the number of channels present int the source data (`ISamples.Channels`). |
| **Config** | [Config](#config-class) | The `Config` this detector has been created with. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Detect(ReadOnlySpan\<float\> sampleBlock)** | IReadOnlyList\<[PhoneKey](#phonekey-enum)\> | Runs the Goertzel algorithm on all samples in `sampleBlock` and returns the DTMF key detected in each channel. |
# ISamples Class

Namespace: DtmfDetection.Interfaces

Interface used by the `Analyzer` to access a variety of audio sources in a uniform way. Implement this interface if your data source does not fit any of the pre-built implementations:
- `AudioData` for float arrays of PCM data,
- `AudioFile` for audio files (mp3, wav, aiff and Windows Media Foundation formats),
- or `AudioStream` for infinite audio streams.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Channels** | int | The number of audio channels. This should match the value of `Config.Channels` the `Analyzer` is using. |
| **SampleRate** | int | The rate at which the values have been sampled in Hz. This should match the value of `Config.SampleRate` the `Analyzer` is using. |
| **Position** | TimeSpan | The position of the "read cursor" in the sample stream. This should increase with every call to `Read()` that returns a value greater than 0. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Read(float[] buffer, int count)** | int | Reads `count` samples from the input and writes them into `buffer`. This method should either block until `count` samples have been succesfully read, or return a number less than `count` to indicate that the end of the stream has been reached. Once the end of stream has been reached, subsequent calls to `Read()` are allowed to fail with exceptions. |
