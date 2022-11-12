using System.Diagnostics;

namespace ELTracker.Logger;

public static class Log
{
    public delegate void LogLineReceived(LogLine logLine);

    public static event LogLineReceived LineReceived;

    public static void WriteLine(string message, LogStyle style = LogStyle.Info)
    {
        Console.ResetColor();

        switch (style)
        {
            case LogStyle.Info:
                break;
            case LogStyle.SuperInfo:
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case LogStyle.Muted:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case LogStyle.SuperMuted:
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case LogStyle.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogStyle.SuperWarning:
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case LogStyle.Bad:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogStyle.SuperBad:
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case LogStyle.Good:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case LogStyle.SuperGood:
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
        }

        var output = $"[{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {message}";
        Console.WriteLine(output);
        Trace.WriteLine(output);

        Console.ResetColor();

        LineReceived?.Invoke(new LogLine(output, style));
    }

    public static void WriteException(Exception exception)
    {
        WriteLine($"{exception.GetType().Name}: {exception.Message}\r\n{exception.StackTrace}", LogStyle.Bad);
    }
}