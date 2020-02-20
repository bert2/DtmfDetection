# DtmfDetection & DtmfDetection.NAudio

[![Build status](https://ci.appveyor.com/api/projects/status/yxidl32tv632lagv/branch/master?svg=true)](https://ci.appveyor.com/project/bert2/dtmfdetection/branch/master) [![NuGet](https://img.shields.io/nuget/v/DtmfDetection.NAudio.svg)](https://www.nuget.org/packages/DtmfDetection.NAudio)

Implementation of the [Goertzel algorithm](https://en.wikipedia.org/wiki/Goertzel_algorithm) for the detection of [DTMF tones](https://en.wikipedia.org/wiki/Dual-tone_multi-frequency_signaling) (aka touch tones) in audio data. Install the package [DtmfDetection.NAudio](https://www.nuget.org/packages/DtmfDetection.NAudio) for integration with [NAudio](https://github.com/naudio/NAudio).

## Quick start

### With NAudio

How to detect and print DTMF tones in an mp3 file:

How to detect and print multi-channel DTMF tones in a wav file:

How to detect and print DTMF tones in audio output:

How to detect and print multi-channel DTMF tones in microphone input:

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

### 1.0.0

- complete rewrite of previous implementation
- upgrade to netstandard2.1
- make implementation much more configurable
- normalize Goertzel response with total signal energy for loudness invariance
- improve performance by ~25%

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

- finish readme
- test with .net framework
- implement continuous deployment of CLI tool to choco via AppVeyor
- add XML documentation
- add config options to CLI tool
