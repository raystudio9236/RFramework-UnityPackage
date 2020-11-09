using UnityEngine;

namespace RFramework.Common.Log
{
    public class RLog
    {
        public enum Level
        {
            Log = 0,
            Warning,
            Error,
            None,
        }

        private static Level _level = Level.Log;

        public static void SetLevel(Level l)
        {
            _level = l;
        }

        public static void Log(object obj, object context)
        {
            Log(obj);
        }

        public static void LogWarning(object obj, object context)
        {
            LogWarning(obj);
        }

        public static void LogError(object obj, object context)
        {
            LogError(obj);
        }

        public static void LogException(System.Exception obj, object context)
        {
            LogException(obj);
        }

        public static void Log(object obj)
        {
            if (_level <= Level.Log)
            {
                Debug.Log($"[{Time.frameCount}] {obj}");
            }
        }

        public static void LogFormat(string format, params object[] pars)
        {
            if (_level <= Level.Log)
            {
                Debug.LogFormat($"[{Time.frameCount}] {format}", pars);
            }
        }

        public static void LogWarning(object obj)
        {
            if (_level <= Level.Warning)
            {
                Debug.LogWarning($"[{Time.frameCount}] {obj}");
            }
        }

        public static void LogWarningFormat(string format, params object[] pars)
        {
            if (_level <= Level.Warning)
            {
                Debug.LogWarningFormat($"[{Time.frameCount}] {format}", pars);
            }
        }

        public static void LogError(object obj)
        {
            if (_level <= Level.Error)
            {
                Debug.LogError($"[{Time.frameCount}] {obj}");
            }
        }

        public static void LogErrorFormat(string format, params object[] pars)
        {
            if (_level <= Level.Error)
            {
                Debug.LogErrorFormat($"[{Time.frameCount}] {format}", pars);
            }
        }

        public static void LogException(System.Exception obj = null)
        {
            if (_level <= Level.Error)
            {
                Debug.LogException(obj);
            }
        }
    }
}