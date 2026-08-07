using System.Text;

namespace War3Frame;

public static class FourCc
{
    private static readonly Dictionary<string, int> Cache = new(StringComparer.Ordinal);

    public static int ToId(string idChar)
    {
        if (idChar is null) throw new ArgumentNullException(nameof(idChar));
        if (idChar.Length == 0) return 0;

        if (Cache.TryGetValue(idChar, out var cached))
            return cached;

        var str = idChar.Length >= 4 ? idChar[..4] : idChar.PadRight(4, '\0');
        var bytes = Encoding.ASCII.GetBytes(str);
        var id = (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        Cache[idChar] = id;
        return id;
    }
}
