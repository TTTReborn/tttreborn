using System;

namespace TTTReborn.Extensions
{
    public partial class Log
    {
        public static void Debug(object text = null)
        {
            if (!Gamemode.Game.Instance.Debug)
            {
                return;
            }

            Log.Info($"[DEBUG] {text}");
        }

        public static void Error(object message) => Sandbox.Log.Error(message);
        public static void Error(string message) => Sandbox.Log.Error(message);
        public static void Error(Exception exception, string message) => Sandbox.Log.Error(exception, message);
        public static void Error(Exception exception) => Sandbox.Log.Error(exception);
        public static void Info(object obj) => Sandbox.Log.Info(obj);
        public static void Info(string message) => Sandbox.Log.Info(message);
        public static void Trace(object message) => Sandbox.Log.Trace(message);
        public static void Trace(string message) => Sandbox.Log.Trace(message);
        public static void Warning(object message) => Sandbox.Log.Warning(message);
        public static void Warning(string message) => Sandbox.Log.Warning(message);
        public static void Warning(Exception exception, string message) => Sandbox.Log.Warning(exception, message);
    }
}
