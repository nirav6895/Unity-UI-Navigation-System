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

        /// <summary>
        /// Logs a message to the Unity console if the LogLevel is set to Full.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            if (LogLevel == LogLevel.Full)
                Debug.Log(message);
        }

        /// <summary>
        /// Logs a warning message to the Unity console if the LogLevel is set to Full or WarningsAndErrors.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string message)
        {
            if (LogLevel >= LogLevel.WarningsAndErrors)
                Debug.LogWarning(message);
        }

        /// <summary>
        /// Logs an error message to the Unity console if the LogLevel is set to ErrorsOnly or Disable.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(string message)
        {
            if (LogLevel >= LogLevel.ErrorsOnly)
                Debug.LogError(message);
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