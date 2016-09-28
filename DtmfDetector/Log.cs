namespace DtmfDetector
{
    using System;
    using System.IO;

    public sealed class Log : IDisposable
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            file.Flush();
            file.Close();
        }
    }
}
