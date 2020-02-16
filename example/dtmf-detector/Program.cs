namespace dtmf_detector {
    using System;
    using NAudio.Wave;
    using DtmfDetection;
    using DtmfDetection.NAudio;
    using System.Reflection;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using NAudio.CoreAudioApi;

    public static class Program {
        public static int Main(string[] args) {
            try {
                PrintVersion();

                if (args.Length == 0)
                    MenuLoop();
                else if (string.Equals(args[0], "--version", StringComparison.OrdinalIgnoreCase))
                    return 0;
                else if (string.Equals(args[0], "--help", StringComparison.OrdinalIgnoreCase))
                    PrintHelp();
                else
                    AnalyzeFile(path: args[0]);

                return 0;
            } catch (COMException e) when (e.ErrorCode == unchecked((int)0xC00D36C4)) {
                Console.WriteLine(new InvalidOperationException("Unsupported media type", e));
                return 1;
            } catch (Exception e) {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static void PrintVersion() {
            var assembly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo
                .GetVersionInfo(assembly.Location)
                .FileVersion;
            Console.WriteLine($"dtmf-detector v{version}\n");
        }

        private static void PrintHelp() {
            Console.WriteLine("usage:");
        }

        private static void AnalyzeFile(string path) {
            // supports .mp3, .wav, aiff, and Windows Media Foundation formats
            using var audioFile = new AudioFileReader(path);
            Console.WriteLine($"DTMF tones found in file '{audioFile.FileName}':\n");
            foreach (var dtmf in audioFile.DtmfChanges().ToDtmfTones())
                Console.WriteLine(dtmf);
        }

        private static void MenuLoop() {
            var state = MenuState.NotAnalyzing;
            BackgroundAnalyzer? analyzer = null;
            IWaveIn? audioSource = null;

            while (true) {
                PrintMenu(state);
                var key = Console.ReadKey(intercept: true);
                switch (key.Key, state) {
                    case (ConsoleKey.Escape, MenuState.NotAnalyzing):
                        return;
                    case (ConsoleKey.Escape, MenuState.AnalyzingMicIn):
                    case (ConsoleKey.Escape, MenuState.AnalyzingOutput):
                        state = MenuState.NotAnalyzing;
                        analyzer?.StopCapturing();
                        audioSource?.Dispose();
                        analyzer = null;
                        audioSource = null;
                        break;
                    case (ConsoleKey.M, MenuState.NotAnalyzing):
                        state = MenuState.AnalyzingMicIn;
                        audioSource = new WaveInEvent { WaveFormat = new WaveFormat(Config.Default.SampleRate, bits: 32, channels: 1) };
                        analyzer = new BackgroundAnalyzer(audioSource, config: Config.Default.WithThreshold(10));
                        analyzer.OnDtmfDetected += dtmf => Console.WriteLine(dtmf);
                        analyzer.StartCapturing();
                        break;
                    case (ConsoleKey.O, MenuState.NotAnalyzing):
                        state = MenuState.AnalyzingOutput;
                        audioSource = new WasapiLoopbackCapture { ShareMode = AudioClientShareMode.Shared };
                        analyzer = new BackgroundAnalyzer(audioSource);
                        analyzer.OnDtmfDetected += dtmf => Console.WriteLine(dtmf);
                        analyzer.StartCapturing();
                        break;
                }
            }
        }

        private static void PrintMenu(MenuState state) {
            Console.WriteLine("----------------------------------");

            if (state != MenuState.NotAnalyzing) Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" ■ ");
            Console.ResetColor();
            Console.WriteLine($"{state.ToPrettyString()}");

            Console.WriteLine("----------------------------------");

            Console.WriteLine(state switch {
                MenuState.NotAnalyzing => "[M]   analyze microphone input\n"
                                        + "[O]   analyze current audio output\n"
                                        + "[Esc] quit",
                _                      => "[Esc] stop analyzing"
            });

            Console.WriteLine();
        }

        private static string ToPrettyString(this MenuState state) => state switch {
            MenuState.NotAnalyzing => "not analyzing",
            MenuState.AnalyzingMicIn => "analyzing microphone input",
            MenuState.AnalyzingOutput => "analyzing current audio output",
            _ => throw new InvalidEnumArgumentException(nameof(state), (int)state, typeof(MenuState))
        };

        private enum MenuState {
            NotAnalyzing,
            AnalyzingMicIn,
            AnalyzingOutput
        }
    }
}
