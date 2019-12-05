namespace Benchmark {
    using BenchmarkDotNet.Running;

    // `dtmf-detection\test\benchmark> dotnet run -c Release`
    // `dtmf-detection\test\benchmark> dotnet run -c Release -- --filter *LastRelease*`
    // `dtmf-detection\test\benchmark> dotnet run -c Release -- --filter *Current`
    // `dtmf-detection\test\benchmark> dotnet run -c Release -- --list flat`
    public static class Program {
        public static void Main(string[] args) => BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args);
    }
}
