# DtmfDetection.NAudio 1.2.0 API documentation

Created by [mddox](https://github.com/loxsmoke/mddox) on 05/03/2020

# All types

|   |   |   |
|---|---|---|
| [AudioFile Class](#audiofile-class) | [BufferedWaveProviderExt Class](#bufferedwaveproviderext-class) | [SampleProviderExt Class](#sampleproviderext-class) |
| [AudioStream Class](#audiostream-class) | [IWaveInExt Class](#iwaveinext-class) | [WaveStreamExt Class](#wavestreamext-class) |
| [BackgroundAnalyzer Class](#backgroundanalyzer-class) | [MonoSampleProvider Class](#monosampleprovider-class) |   |
# AudioFile Class

Namespace: DtmfDetection.NAudio

Convenience implementation of `ISamples` for `WaveStream` audio. `WaveStream`s are commonly used for finite data like audio files. The `Analyzer` uses `ISamples` to read the input data in blocks and feed them to the `Detector`.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Channels** | int | Returns the number of channels in the `WaveStream` input or `1` when mono-conversion has been enabled. |
| **SampleRate** | int | Returns the target sample rate this `AudioFile` has been created with. |
| **Position** | TimeSpan | Returns the current position of the input `WaveStream`. |
## Constructors

| Name | Summary |
|---|---|
| **AudioFile(WaveStream source, int targetSampleRate, bool forceMono)** | Creates a new `AudioFile` from a `WaveStream` input. Also resamples the input and optionally converts it to single-channel audio. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Read(float[] buffer, int count)** | int | Reads `count` samples from the input and writes them into `buffer`. |
# AudioStream Class

Namespace: DtmfDetection.NAudio

Convenience implementation of `ISamples` for an infinite `IWaveIn` audio stream. The `Analyzer` uses `ISamples` to read the input data in blocks and feed them to the `Detector`.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Channels** | int | Returns the number of channels in the `IWaveIn` input or `1` when mono-conversion has been enabled. |
| **SampleRate** | int | Returns the target sample rate this `AudioStream` has been created with. |
| **Position** | TimeSpan | Simply calls `DateTime.Now.TimeOfDay` and returns the result. |
## Constructors

| Name | Summary |
|---|---|
| **AudioStream(IWaveIn source, int targetSampleRate, bool forceMono)** | Creates a new `AudioStream` from an `IWaveIn` input by buffering it with a `BufferedWaveProvider`. Also resamples the input and optionally converts it to single-channel audio. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Read(float[] buffer, int count)** | int | Reads `count` samples from the input and writes them into `buffer`. Will block as long as it takes for the input to buffer the requested number of samples. |
| **StopWaiting()** | void | Stops waiting for the input to buffer data. Some `IWaveIn`s don't have data available continuously. For instance a `WasapiLoopbackCapture` will only have data as long as the OS is playing some audio. Calling `StopWaiting()` will break the infinite wait loop and the `Analyzer` processing this `AudioStream` will consider it being "finished". This in turn helps to gracefully exit the thread running the analysis. |
# BackgroundAnalyzer Class

Namespace: DtmfDetection.NAudio

Helper that does audio analysis in a background thread. Useful when analyzing infinite inputs like mic-in our the current audio output.

## Constructors

| Name | Summary |
|---|---|
| **BackgroundAnalyzer(IWaveIn source, bool forceMono, in Config? config, IAnalyzer analyzer)** | Creates a new `BackgroundAnalyzer` and immediately starts listening to the `IWaveIn` input. `Dispose()` this instance to stop the background thread doing the analysis. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Dispose()** | void | Calls `IWaveIn.StopRecording()` on the input stream and halts the background thread doing the analysis. |
# BufferedWaveProviderExt Class

Namespace: DtmfDetection.NAudio

Provides an extension method that waits until a `BufferedWaveProvider` has read enough data.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **WaitForSamples(BufferedWaveProvider source, int count)** | bool | Blocks the thread for as long as the `BufferedWaveProvider` minimally should need to buffer at least `count` sample frames. The wait time is estimated from the difference of the number of already buffered bytes to the number of requested bytes. |
# IWaveInExt Class

Namespace: DtmfDetection.NAudio

Provides an extensions method to buffer a `IWaveIn` stream using a `BufferedWaveProvider`.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **ToBufferedWaveProvider(IWaveIn source)** | BufferedWaveProvider | Creates a `BufferedWaveProvider` for a `IWaveIn` and returns it. |
# MonoSampleProvider Class

Namespace: DtmfDetection.NAudio

Decorates an `ISampleProvider` with a mono-conversion step.

## Properties

| Name | Type | Summary |
|---|---|---|
| **WaveFormat** | WaveFormat | The `WaveFormat` of the decorated `ISampleProvider`. Will match match the `WaveFormat` of the input `ISampleProvider` except that it will be mono (`WaveFormat.Channels` = 1). |
## Constructors

| Name | Summary |
|---|---|
| **MonoSampleProvider(ISampleProvider sourceProvider)** | Creates a new `MonoSampleProvider` from a multi-channel `ISampleProvider`. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **Read(float[] buffer, int offset, int count)** | int | Tries to read `count` sample frames from the input `ISampleProvider`, averages the sample values across all channels and writes one mixed sample value for each sample frame into `buffer`. |
# SampleProviderExt Class

Namespace: DtmfDetection.NAudio

Provides extensions methods for `ISampleProvider`s.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **AsMono(ISampleProvider source)** | ISampleProvider | Converts multi-channel input data to mono by avering all channels. Does nothing in case the input data already is mono. |
| **Resample(ISampleProvider source, int targetSampleRate)** | ISampleProvider | Resamples the input data to the specified target sample rate using the `WdlResamplingSampleProvider`. Does nothing in case the sample rate already matches. |
# WaveStreamExt Class

Namespace: DtmfDetection.NAudio

Provides an extension method to detect DTMF tones in a `WaveStream`.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **DtmfChanges(WaveStream waveStream, bool forceMono, in Config? config)** | List\<DtmfChange\> | Detects DTMF tones in a `WaveStream`. |
