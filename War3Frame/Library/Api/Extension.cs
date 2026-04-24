using System;
using System.Collections.Concurrent;
using System.Text;

namespace War3Frame
{
    public static partial class JassApi
    {
        // 添加静态字段 c 和 i
        private static readonly ConcurrentDictionary<string, int> c = new();
        private static readonly ConcurrentDictionary<int, string> i = new();

        public static int C2I(string idChar)
        {
            if (idChar is null) throw new ArgumentNullException(nameof(idChar));
            if (idChar.Length == 0) return 0;

            return c.GetOrAdd(idChar, s =>
            {
                // use first 4 chars (pad with '\0' if shorter), ASCII encoding
                var str = s.Length >= 4 ? s.Substring(0, 4) : s.PadRight(4, '\0');
                var b = Encoding.ASCII.GetBytes(str);
                // big-endian: first char is most-significant byte
                var val = (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];
                i.TryAdd(val, s);
                return val;
            });
        }
    }
}
