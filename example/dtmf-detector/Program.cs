namespace dtmf_detector {
    using System;
    using NAudio.Wave;
    using DtmfDetection;
    using DtmfDetection.NAudio;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using NAudio.CoreAudioApi;

    public static class Program {
        public static int Main(string[] args) {
            try {
                PrintHeader();

                if (args.Length == 0)
                    MenuLoop();
                else if (string.Equals(args[0], "--version", StringComparison.OrdinalIgnoreCase))
                    return 0;
                else if (string.Equals(args[0], "--help", StringComparison.OrdinalIgnoreCase))
                    PrintHelp();
                else
                    AnalyzeFile(path: args[0]);

                return 0;
            } catch (Exception e) {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static void PrintHeader() {
            var v = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
            Console.WriteLine($"dtmf-detector {v.Major}.{v.Minor}.{v.Build} (https://github.com/bert2/DtmfDetection)\n");
        }

        private static void PrintHelp() {
            Console.WriteLine("USAGE:");
            Console.WriteLine();
            Console.WriteLine("    dtmf-detector.exe <filepath>    Run DTMF detection on the specified file and");
            Console.WriteLine("                                    print all detected DTMF tones.");
            Console.WriteLine();
            Console.WriteLine("    dtmf-detector.exe               Run interactive mode to detect DMTF tones in");
            Console.WriteLine("                                    audio ouput or microphone input.");
            Console.WriteLine();
            Console.WriteLine("    dtmf-detector.exe --version     Print version.");
            Console.WriteLine();
            Console.WriteLine("    dtmf-detector.exe --help        Show this usage information.");
        }

        private static void AnalyzeFile(string path) {
            AudioFileReader audioFile;
            try {
                // supports .mp3, .wav, aiff, and Windows Media Foundation formats
                audioFile = new AudioFileReader(path);
            } catch (COMException e) when (e.ErrorCode == unchecked((int)0xC00D36C4)) {
                throw new InvalidOperationException("Unsupported media type", e);
            }

            Console.WriteLine($"DTMF tones found in file '{audioFile.FileName}':\n");
            foreach (var dtmf in audioFile.DtmfChanges().ToDtmfTones())
                Console.WriteLine(dtmf);
        }

        private static void MenuLoop() {
            var state = State.NotAnalyzing;
            IWaveIn? audioSource = null;
            BackgroundAnalyzer? analyzer = null;

            while (true) {
                PrintMenu(state);
                var key = Console.ReadKey(intercept: true);
                switch (key.Key, state) {
                    case (ConsoleKey.Escape, State.NotAnalyzing):
                        return;
                    case (ConsoleKey.Escape, State.AnalyzingMicIn):
                    case (ConsoleKey.Escape, State.AnalyzingOutput):
                        state = State.NotAnalyzing;
                        analyzer?.Dispose();
                        audioSource?.Dispose();
                        break;
                    case (ConsoleKey.M, State.NotAnalyzing):
                        state = State.AnalyzingMicIn;
                        audioSource = new WaveInEvent { WaveFormat = new WaveFormat(Config.Default.SampleRate, bits: 32, channels: 1) };
                        analyzer = new BackgroundAnalyzer(audioSource, config: Config.Default.WithThreshold(10));
                        analyzer.OnDtmfDetected += dtmf => Console.WriteLine(dtmf);
                        break;
                    case (ConsoleKey.O, State.NotAnalyzing):
                        state = State.AnalyzingOutput;
                        audioSource = new WasapiLoopbackCapture { ShareMode = AudioClientShareMode.Shared };
                        analyzer = new BackgroundAnalyzer(audioSource);
                        analyzer.OnDtmfDetected += dtmf => Console.WriteLine(dtmf);
                        break;
                }
            }
        }

        private static void PrintMenu(State state) {
            Console.WriteLine("----------------------------------");

            if (state != State.NotAnalyzing) Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" ■ ");
            Console.ResetColor();
            Console.WriteLine($"{state.ToPrettyString()}");

            Console.WriteLine("----------------------------------");

            Console.WriteLine(state switch {
                State.NotAnalyzing => "[M]   analyze microphone input\n"
                                    + "[O]   analyze current audio output\n"
                                    + "[Esc] quit",
                _                  => "[Esc] stop analyzing"
            });

            Console.WriteLine();
        }

        private static string ToPrettyString(this State state) => state switch {
            State.NotAnalyzing => "not analyzing",
            State.AnalyzingMicIn => "analyzing microphone input",
            State.AnalyzingOutput => "analyzing current audio output",
            _ => throw new InvalidEnumArgumentException(nameof(state), (int)state, typeof(State))
        };

        private enum State {
            NotAnalyzing,
            AnalyzingMicIn,
            AnalyzingOutput
        }
    }
}
