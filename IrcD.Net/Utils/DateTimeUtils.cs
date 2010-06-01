using System;

namespace IrcD.Utils
{
    static class DateTimeUtils
    {
        public static long ToUnixTime(this DateTime dateTime)
        {
            var span = dateTime - new DateTime(1970, 1, 1);
            return (long)span.TotalSeconds;
        }
    }
}
