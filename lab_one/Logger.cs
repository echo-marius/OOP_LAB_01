using System.Text;

namespace lab_one;

public enum LogLevel
{
    Trace,
    Info,
    Warning,
    Error
}

public class Logger
{
    private static readonly Random _rng = new Random();
    public string Name { get; set; }
    public static string Format { get; set; } = "[%t] [%l] %n: %m %r";
    public static LogLevel Level { get; set; } = LogLevel.Trace;

    public Logger(string name)
    {
        Name = name;
    }

    private string GetRandomString(LogLevel level)
    {
        int index = _rng.Next(0, 3); 

        switch (level)
        {
            case LogLevel.Trace:
                return index switch
                {
                    0 => "Entering execution block",
                    1 => "Trace point reached",
                    _ => "Checking local state"
                };
            
            case LogLevel.Info:
                return index switch
                {
                    0 => "Operation completed",
                    1 => "Everything looks good",
                    _ => "Here is your result"
                };

            case LogLevel.Warning:
                return index switch
                {
                    0 => "Resource utilization high",
                    1 => "Deprecated API method invoked",
                    _ => "Response time threshold exceeded"
                };

            case LogLevel.Error:
                return index switch
                {
                    0 => "Failed to establish connection",
                    1 => "Unhandled exception caught",
                    _ => "Database query failed"
                };
            default:
                return "Status update recorded";
        }
    }
    public void Log(LogLevel level, string message)
    {
        if (level < Level || String.IsNullOrWhiteSpace(message) ||  String.IsNullOrWhiteSpace(Format))
        {
            return;
        }
        StringBuilder builder = new StringBuilder();
        builder.Append(Format);
        
        builder.Replace("%t", TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm:ss"));
        builder.Replace("%n", this.Name);
        builder.Replace("%l", level.ToString());
        builder.Replace("%r",GetRandomString(Level));
        builder.Replace("%m", message);
        
        Console.WriteLine(builder.ToString());
    }
}

