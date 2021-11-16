//TODO args: name, wake up interval
using GibNet;
using GibNet.Logging;
using System.Diagnostics;

ArgumentsHandler handler = ArgumentsHandler.Factory()
    .Positional("process")
    .Positional("arguments")
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
    string arguments = handler.GetPositional(1);
    Watchdog watchdog = new Watchdog(executable, arguments, workingDir)
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