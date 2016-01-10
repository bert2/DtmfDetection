namespace DtmfDetector
{
    using System;
    using System.IO;

    public class Log : IDisposable
    {
        private readonly StreamWriter file;

        public Log(string fileName)
        {
            file = new StreamWriter(fileName, false);
        }

        public void Add(string message)
        {
            Console.WriteLine(message);
            file.WriteLine(message);
        }

        public void Dispose()
        {
            file.Flush();
            file.Close();
            GC.SuppressFinalize(this);
        }
    }
}
