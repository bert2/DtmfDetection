namespace DtmfDetector
{
    using System;

    using DtmfDetection.NAudio;

    public static class Program
    {
        public static void Main()
        {
            //using (var log = new Log("waveFile.log"))
            //{
            //    foreach (var occurence in new WaveFile("dtmftest.wav").DtmfTones)
            //    {
            //        log.Add($"{occurence.Position.TotalSeconds:00.000} s: {occurence.DtmfTone.Key} key (duration: {occurence.Duration.TotalSeconds:00.000} s)");
            //    }
            //}

            using (var log = new Log("audioOut.log"))
            {
                var audioOut = new CurrentAudioOutput();
                audioOut.DtmfToneStarting += start => log.Add($"{start.DtmfTone.Key} key started on {start.Position.TimeOfDay}");
                audioOut.DtmfToneStopped += end => log.Add($"{end.DtmfTone.Key} key stopped after {end.Duration.TotalSeconds:00.000} s");

                while (true)
                {
                    Console.WriteLine("St[a]rt, St[o]p, [Q]uit");
                    var key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.A:
                            audioOut.StartCapturing();
                            break;

                        case ConsoleKey.O:
                            audioOut.StopCapturing();
                            break;

                        case ConsoleKey.Q:
                            audioOut.StopCapturing();
                            return;
                    }
                }
            }
        }
    }
}
