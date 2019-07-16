namespace Cleaner.Core
{
    public class AppSettings
    {
        public AppSettings_Logging Logging { get; set; }
    }

    public class AppSettings_Logging
    {
        public bool IncludeScopes { get; set; }
        public AppSettings_Logging_LogLevel LogLevel { get; set; }
        public AppSettings_Logging_Console Console { get; set; }
    }

    public class AppSettings_Logging_LogLevel
    {
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class AppSettings_Logging_Console
    {
        public bool IncludeScopes { get; set; }
    }
}
