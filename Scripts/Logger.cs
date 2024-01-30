using System.Text;
using UnityEngine;

namespace LRS.SceneManagement
{
    internal static class Logger
    {
        public static void Log(string message)
        {
            if (Settings.DebugMode)
            {
                Debug.Log(CreateLogMessage(message));
            }
        }

        public static void LogWarning(string message)
        {
            if (Settings.DebugMode)
            {
                Debug.LogWarning(CreateLogMessage(message, "yellow"));
            }
        }

        public static void LogError(string message)
        {
            if (Settings.DebugMode)
            {
                Debug.LogError(CreateLogMessage(message, "red"));
            }
        }

        private static string CreateLogMessage(string message, string color = "white")
        {
            StringBuilder builder = new();
            builder.Append("<b><color=");
            builder.Append(color);
            builder.Append(">Reliable Scene Manager:</color></b>\n");
            builder.Append("<color=");
            builder.Append(color);
            builder.Append(">");
            builder.Append(message);
            builder.Append("</color>");
            return builder.ToString();
        }
    }
}