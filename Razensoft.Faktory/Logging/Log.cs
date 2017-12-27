using System;

namespace Razensoft.Faktory.Logging
{
    public class Log
    {
        private readonly Type type;

        public Log(Type type)
        {
            this.type = type;
        }

        public static LogLevel Level { get; set; } = LogLevel.Info;

        public void Trace(string message)
        {
            Print(LogLevel.Trace, message, ConsoleColor.Cyan);
        }

        public void Info(string message)
        {
            Print(LogLevel.Info, message, ConsoleColor.Gray);
        }

        public void Warning(string message)
        {
            Print(LogLevel.Warning, message, ConsoleColor.Yellow);
        }

        public void Error(string message)
        {
            Print(LogLevel.Error, message, ConsoleColor.Red);
        }

        private void Print(LogLevel level, string message, ConsoleColor color)
        {
            if (Level > level)
                return;
            var logLine = $"{DateTime.Now:s} [{type.FullName}] {level.ToString().ToUpper()} {message}";
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(logLine);
            Console.ForegroundColor = currentColor;
        }
    }

    public enum LogLevel
    {
        Trace,
        Info,
        Warning,
        Error
    }
}
