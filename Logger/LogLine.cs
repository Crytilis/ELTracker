namespace ELTracker.Logger;

public class LogLine
{
    public LogLine(string message, LogStyle style)
    {
        Message = message;
        Style = style;
    }

    public string Message { get; }
    public LogStyle Style { get; }
}