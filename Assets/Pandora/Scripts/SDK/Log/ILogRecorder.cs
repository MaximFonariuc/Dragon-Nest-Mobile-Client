using UnityEngine;

namespace com.tencent.pandora
{
    interface ILogRecorder
    {
        void Add(Log log);
        void Dispose();
    }

    struct Log
    {
        public string message;
        public string stackTrace;
        public int level;
    }
}
