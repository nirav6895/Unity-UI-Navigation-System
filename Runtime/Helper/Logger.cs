#region Using
using UnityEngine;
#endregion

namespace NG.UINavigationSystem.Utilities
{
    /// <summary>
    /// A simple logging utility class. 
    /// This class provides methods for logging messages, warnings, and errors to the Unity console based on the current log level setting.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The current log level for the Logger. This can be set to control the amount of logging output in the Unity console.
        /// </summary>
        internal static LogLevel LogLevel = LogLevel.Full;

        private const string LOG_FORMAT = "[UINavigationSystem] {0}";

        /// <summary>
        /// Logs a message to the Unity console if the LogLevel is set to Full.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            if (LogLevel == LogLevel.Full)
                Debug.Log(string.Format(LOG_FORMAT, message));
        }

        /// <summary>
        /// Logs a formatted message to the Unity console if the LogLevel is set to Full.
        /// Message is built only if it's going to be logged.
        /// </summary>
        /// <param name="format">The message format, e.g. "Show UI: {0} with Sorting Order: {1}".</param>
        /// <param name="args">The arguments to format.</param>
        public static void Log(string format, params object[] args)
        {
            if (LogLevel == LogLevel.Full)
                Log(string.Format(format, args));
        }

        /// <summary>
        /// Logs a warning message to the Unity console if the LogLevel is set to Full or WarningsAndErrors.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string message)
        {
            if (LogLevel <= LogLevel.WarningsAndErrors)
                Debug.LogWarning(string.Format(LOG_FORMAT, message));
        }

        /// <summary>
        /// Logs an error message to the Unity console if the LogLevel is set to Full, WarningsAndErrors or ErrorsOnly.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(string message)
        {
            if (LogLevel <= LogLevel.ErrorsOnly)
                Debug.LogError(string.Format(LOG_FORMAT, message));
        }
    }

    /// <summary>
    /// Defines the log levels for the Logger class.
    /// </summary>
    public enum LogLevel
    {
        Full,

        WarningsAndErrors,

        ErrorsOnly,

        Disable
    }
}