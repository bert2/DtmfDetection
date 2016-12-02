namespace DtmfDetector
{
    using System;

    using NAudio.CoreAudioApi;
    using NAudio.Wave;

    using DtmfDetection.NAudio;

    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                    AnalyzeWaveFile(args[0]);
                else
                    CaptureAndAnalyzeLiveAudio();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static void AnalyzeWaveFile(string path)
        {
            using (var log = new Log("waveFile.log"))
            {
                using (var audioFile = new AudioFileReader(path))
                {
                    foreach (var occurence in audioFile.DtmfTones())
                    {
                        log.Add(
                            $"{occurence.Position.TotalSeconds:00.000} s "
                            + $"({occurence.Channel}): "
                            + $"{occurence.DtmfTone.Key} key "
                            + $"(duration: {occurence.Duration.TotalSeconds:00.000} s)");
                    }
                }
            }
        }

        private static void CaptureAndAnalyzeLiveAudio()
        {
            using (var log = new Log("audioOut.log"))
            {
                LiveAudioDtmfAnalyzer analyzer = null;
                IWaveIn audioSource = null;

                while (true)
                {
                    Console.WriteLine("====================================\n"
                                      + "[O]   Capture current audio output\n"
                                      + "[M]   Capture mic in\n"
                                      + "[Esc] Stop capturing/quit");
                    var key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.O:
                            analyzer?.StopCapturing();
                            audioSource?.Dispose();

                            audioSource = new WasapiLoopbackCapture { ShareMode = AudioClientShareMode.Shared };
                            analyzer = InitLiveAudioAnalyzer(log, audioSource);
                            analyzer.StartCapturing();

                            break;

                        case ConsoleKey.M:
                            analyzer?.StopCapturing();
                            audioSource?.Dispose();

                            audioSource = new WaveInEvent { WaveFormat = new WaveFormat(8000, 32, 1) };
                            analyzer = InitLiveAudioAnalyzer(log, audioSource);
                            analyzer.StartCapturing();

                            break;

                        case ConsoleKey.Escape:
                            if (analyzer == null || !analyzer.IsCapturing)
                                return;

                            analyzer.StopCapturing();
                            audioSource.Dispose();

                            break;
                    }
                }
            }
        }

        private static LiveAudioDtmfAnalyzer InitLiveAudioAnalyzer(Log log, IWaveIn waveIn)
        {
            var analyzer = new LiveAudioDtmfAnalyzer(waveIn);
            analyzer.DtmfToneStarted += start => log.Add($"{start.DtmfTone.Key} key started on {start.Position.TimeOfDay} (channel {start.Channel})");
            analyzer.DtmfToneStopped += end => log.Add($"{end.DtmfTone.Key} key stopped after {end.Duration.TotalSeconds} s (channel {end.Channel})");
            return analyzer;
        }
    }
}
