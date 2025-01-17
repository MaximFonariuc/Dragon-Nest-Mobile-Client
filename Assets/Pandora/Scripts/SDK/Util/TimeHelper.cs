using System;

namespace com.tencent.pandora
{
    internal class TimeHelper
    {
        /// <summary>
        /// 当前时间秒数（自1970-01-01 00:00:00至今）
        /// </summary>
        /// <returns></returns>
        public static int NowSeconds()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        /// <summary>
        /// 当前时间毫秒数（自1970-01-01 00:00:00至今）
        /// </summary>
        /// <returns></returns>
        public static UInt64 NowMilliseconds()
        {
            return (UInt64)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }
    }
}
