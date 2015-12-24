using NAudio.CoreAudioApi;

namespace DtmfDetector
{
    using System;

    using NAudio.Wave;

    using DtmfDetection.NAudio;

    public static class Program
    {
        public static void Main()
        {
            AnalyzeWaveFile();
            CaptureAndAnalyzeAudioOut();
        }

        private static void AnalyzeWaveFile()
        {
            using (var log = new Log("waveFile.log"))
            {
                using (var waveFile = new WaveFileReader("dtmftest.wav"))
                {
                    foreach (var occurence in waveFile.FindDtmfTones())
                        log.Add($"{occurence.Position.TotalSeconds:00.000} s: {occurence.DtmfTone.Key} key (duration: {occurence.Duration.TotalSeconds:00.000} s)");
                }
            }
        }

        private static void CaptureAndAnalyzeAudioOut()
        {
            using (var log = new Log("audioOut.log"))
            {
                LiveAudioAnalyzer audioSource = null;

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
                            audioSource?.StopCapturing();
                            audioSource = InitLiveAudioAnalyzer(log, new WasapiLoopbackCapture { ShareMode = AudioClientShareMode.Shared });
                            audioSource.StartCapturing();
                            break;

                        case ConsoleKey.M:
                            audioSource?.StopCapturing();
                            audioSource = InitLiveAudioAnalyzer(log, new WaveInEvent { WaveFormat = new WaveFormat(8000, 32, 1) });
                            audioSource.StartCapturing();
                            break;

                        case ConsoleKey.Escape:
                            if (audioSource == null || !audioSource.IsCapturing)
                                return;

                            audioSource.StopCapturing();
                            break;
                    }
                }
            }
        }

        private static LiveAudioAnalyzer InitLiveAudioAnalyzer(Log log, IWaveIn waveIn)
        {
            var audioSource = new LiveAudioAnalyzer(waveIn);
            audioSource.DtmfToneStarting += start => log.Add($"{start.DtmfTone.Key} key started on {start.Position.TimeOfDay}");
            audioSource.DtmfToneStopped += end => log.Add($"{end.DtmfTone.Key} key stopped after {end.Duration.TotalSeconds:00.000} s");
            return audioSource;
        }
    }
}
