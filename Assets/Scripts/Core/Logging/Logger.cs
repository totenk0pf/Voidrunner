using UnityEngine;

namespace Core.Logging {
    public static class Logger {
        public enum LogLevel {
            INFO,
            WARNING,
            ERROR
        }

        public static void Log(string message, LogLevel level) {
            Debug.Log(message);
        }
    }
}