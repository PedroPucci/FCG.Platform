using Serilog.Events;
using Serilog;
using ILogger = Serilog.ILogger;

namespace FCG.Platform.Extensions.ExtensionsLogs
{
    public class LogExtension
    {
        private static readonly string LogDirectory = "C://Users//User//Downloads//FCG-Plataform-logs";

        static LogExtension()
        {
            InitializeLogger();
        }

        public static void InitializeLogger()
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(LogEventLevel.Debug)
                    .WriteTo.File(Path.Combine(LogDirectory, "log-.txt"), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1000000)
                    .CreateLogger();

                DeletePreviousLogFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start the logger: {ex.Message}");
            }
        }

        private static void DeletePreviousLogFile()
        {
            var yesterdayDate = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            var logFilePath = Path.Combine(LogDirectory, $"log-{yesterdayDate}.txt");
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);
        }

        public static ILogger GetLogger()
        {
            return Log.Logger;
        }
    }
}