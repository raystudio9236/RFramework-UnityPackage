using System;
using UnityEngine;

namespace RFramework.Common.Log
{
    /// <summary>
    /// 日志静态类
    /// </summary>
    public static class RLog
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
                Debug.Log($"[{DateTime.Now:hh:mm:ss}] [{Time.frameCount}] {obj}");
            }
        }

        public static void LogFormat(string format, params object[] pars)
        {
            if (_level <= Level.Log)
            {
                Debug.LogFormat($"[{DateTime.Now:hh:mm:ss}] [{Time.frameCount}] {format}", pars);
            }
        }

        public static void LogWarning(object obj)
        {
            if (_level <= Level.Warning)
            {
                Debug.LogWarning($"[{DateTime.Now:hh:mm:ss}] [{Time.frameCount}] {obj}");
            }
        }

        public static void LogWarningFormat(string format, params object[] pars)
        {
            if (_level <= Level.Warning)
            {
                Debug.LogWarningFormat($"[{DateTime.Now:hh:mm:ss}] [{Time.frameCount}] {format}", pars);
            }
        }

        public static void LogError(object obj)
        {
            if (_level <= Level.Error)
            {
                Debug.LogError($"[{DateTime.Now:hh:mm:ss}] [{Time.frameCount}] {obj}");
            }
        }

        public static void LogErrorFormat(string format, params object[] pars)
        {
            if (_level <= Level.Error)
            {
                Debug.LogErrorFormat($"[{DateTime.Now:hh:mm:ss}] [{Time.frameCount}] {format}", pars);
            }
        }

        public static void LogException(Exception obj = null)
        {
            if (_level <= Level.Error)
            {
                Debug.LogException(obj);
            }
        }
    }
}