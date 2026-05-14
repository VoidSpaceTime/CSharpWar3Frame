using System.Collections.Concurrent;
using System.Text;

namespace War3Frame;

public static class FourCc
{
    private static readonly ConcurrentDictionary<string, int> Cache = new();

    public static int ToId(string idChar)
    {
        if (idChar is null) throw new ArgumentNullException(nameof(idChar));
        if (idChar.Length == 0) return 0;

        return Cache.GetOrAdd(idChar, static value =>
        {
            var str = value.Length >= 4 ? value[..4] : value.PadRight(4, '\0');
            var bytes = Encoding.ASCII.GetBytes(str);
            return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        });
    }
}
