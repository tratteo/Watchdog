using HandierCli;
using System.Diagnostics;

namespace Watchdog;

public class ProcessWatchdog
{
    private readonly string executablePath;
    private readonly string arguments;
    private readonly string? workingDir;
    private readonly string executableName;
    private int iterationCounter = 0;
    private Process? instance;

    public bool Inhibit { get; private set; } = false;

    public bool Embed { get; set; } = false;

    public bool KillOnExit { get; set; } = false;

    public int MaxIterations { get; set; } = int.MaxValue;

    public ProcessWatchdog(string executable, string arguments, string? workingDir = null)
    {
        executablePath = executable;
        this.arguments = arguments;
        this.workingDir = workingDir;
        executableName = SplitPath(executable).Item2;
        AppDomain.CurrentDomain.ProcessExit += OnExit;
    }

    public async Task Loop()
    {
        Logger.ConsoleInstance.LogInfo("Booting " + executableName);
        while (!Inhibit && iterationCounter < MaxIterations)
        {
            instance = StartProcess();
            if (instance != null)
            {
                await instance.WaitForExitAsync();
                iterationCounter++;
                Logger.ConsoleInstance.LogWarning("Iteration: " + iterationCounter + " | " + executableName + " has stopped with exit code: " + instance.ExitCode + ", rebooting");
            }
        }
    }

    private void OnExit(object? sender, EventArgs args)
    {
        if (KillOnExit && instance != null)
            instance.Kill();
    }

    private (string, string) SplitPath(string path)
    {
        var index = path.LastIndexOf("/");
        if (index == -1)
            index = path.LastIndexOf("\\");
        var folder = index < 0 ? string.Empty : path[..(index + 1)];
        var name = path.Substring(index + 1, path.Length - index - 1);
        return (folder, name);
    }

    private Process? StartProcess()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(executablePath, arguments);
        if (!Embed)
            startInfo.UseShellExecute = true;
        if (workingDir != null)
            startInfo.WorkingDirectory = workingDir;

        return Process.Start(startInfo);
    }

    public class WatchdogLogger : Logger
    {
        public override void LogError(string log, bool newLine = true) => base.LogError(log, newLine);

        public override void LogInfo(string log, ConsoleColor color, bool newLine = true) => base.LogInfo("(Watchdog) " + log, color, newLine);

        public override void LogWarning(string log, bool newLine = true) => base.LogWarning(log, newLine);
    }
}