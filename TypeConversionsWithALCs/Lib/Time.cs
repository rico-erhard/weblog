using System;

namespace Lib
{
    public class Time : AppLib.Time
    {
        DateTimeOffset time;

        Time(DateTimeOffset time)
        {
            this.time = time;
        }

        public static Time Of(long unixTimeMilliseconds)
        => new Time(DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds));

        public static Time Now => new Time(DateTimeOffset.Now);

        public override string ToString() => time.ToString();

        public long ToUnixTimeMilliseconds() => time.ToUnixTimeMilliseconds();

        public AppLib.UnixTimeMilliseconds UnixTimeMilliseconds
        => () => time.ToUnixTimeMilliseconds();
    }
}
