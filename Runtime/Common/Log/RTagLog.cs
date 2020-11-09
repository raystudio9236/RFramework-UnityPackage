namespace RFramework.Common.Log
{
    public class RTagLog
    {
        private string _tag;
        public string Tag { get => _tag; }

        public RTagLog(string tag)
        {
            _tag = tag;
        }

        public void Log(object obj)
        {
            RLog.Log($"|{_tag}| {obj}");
        }

        public void LogFormat(string format, params object[] pars)
        {
            RLog.LogFormat($"|{_tag}| {format}", pars);
        }

        public void LogWarning(object obj)
        {
            RLog.LogWarning($"|{_tag}| {obj}");
        }

        public void LogWarningFormat(string format, params object[] pars)
        {
            RLog.LogWarningFormat($"|{_tag}| {format}", pars);
        }

        public void LogError(object obj)
        {
            RLog.LogError($"|{_tag}| {obj}");
        }

        public void LogErrorFormat(string format, params object[] pars)
        {
            RLog.LogErrorFormat($"|{_tag}| {format}", pars);
        }
    }
}