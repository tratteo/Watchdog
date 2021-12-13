//TODO args: name, wake up interval
using HandierCli;
using Watchdog;

ArgumentsHandler handler = ArgumentsHandler.Factory()
    .Positional("process to watch over")
    .Keyed("-arg", "arguments for the observed process")
    .Keyed("-w", "working directory")
    .Keyed("-mi", "max number of reboots")
    .Flag("/e", "embed the watchdog in a single terminal")
    .Flag("/k", "kill the observed process when the watchdog is terminated")
    .Build();
handler.LoadArgs(args);
if (handler.Valid())
{
    handler.GetKeyed("-w", out string workingDir);
    string executable = handler.GetPositional(0);
    handler.GetKeyed("-arg", out string arguments);
    arguments ??= string.Empty;
    if (!File.Exists(executable))
    {
        Logger.ConsoleInstance.LogError("Executable " + executable + " not found");
        Console.ReadKey();
        Environment.Exit(1);
    }
    if (workingDir != null && !Directory.Exists(workingDir))
    {
        Logger.ConsoleInstance.LogError("Working directory " + workingDir + " not found");
        Console.ReadKey();
        Environment.Exit(1);
    }
    ProcessWatchdog watchdog = new ProcessWatchdog(executable, arguments, workingDir)
    {
        Embed = handler.HasFlag("/e"),
        KillOnExit = handler.HasFlag("/k")
    };
    if (handler.GetKeyed("-mi", out string iters))
    {
        try
        {
            int val = int.Parse(iters);
            watchdog.MaxIterations = val;
        }
        catch (Exception)
        {
            Logger.ConsoleInstance.LogError("-mi requires an int");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
    await watchdog.Loop();
}
else
{
    Logger.ConsoleInstance.LogError("Wrong synthax, args: \n" + handler.ToString());
    Console.ReadKey();
    Environment.Exit(1);
}