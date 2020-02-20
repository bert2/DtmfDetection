using _Task = System.Threading.Tasks.Task;

string Prompt(string message, TimeSpan? timeout = null) {
    Warning(message);
    Console.Write("> ");

    string response = null;

    _Task.WhenAny(
        _Task.Run(() => response = Console.ReadLine()),
        _Task.Delay(timeout ?? TimeSpan.FromSeconds(30))
    ).Wait();

    if (response == null)
        throw new Exception($"User prompt timed out.");

    return response;
}
