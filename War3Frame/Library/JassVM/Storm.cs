using System.Text;

namespace War3Frame
{
    public class Storm
    {
        private static void HashMix(ref uint a, ref uint b, ref uint c)
        {
            a = (c >> 13) ^ (a - b - c);
            b = (a << 8) ^ (b - c - a);
            c = (b >> 13) ^ (c - a - b);
            a = (c >> 12) ^ (a - b - c);
            b = (a << 16) ^ (b - c - a);
            c = (b >> 5) ^ (c - a - b);
            a = (c >> 3) ^ (a - b - c);
            b = (a << 10) ^ (b - c - a);
            c = (b >> 15) ^ (c - a - b);
        }

        public static uint StringHash(string? str)
        {
            if (string.IsNullOrEmpty(str))
                return StringHashEx((ReadOnlySpan<byte>)null);

            byte[] s = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= (byte)'a' && s[i] <= (byte)'z')
                    s[i] = (byte)(s[i] - ((byte)'a' - (byte)'A'));
                else if (s[i] == (byte)'/')
                    s[i] = (byte)'\\';
            }

            return StringHashEx(s);
        }

        private static uint StringHashEx(ReadOnlySpan<byte> str)
        {
            uint a = 0x9E3779B9u, b = 0x9E3779B9u, c = 0u;
            int pos = 0;
            int len = str.Length;
            int remaining = len;

            while (remaining >= 12)
            {
                a += (uint)(str[pos] + (str[pos + 1] << 8) + (str[pos + 2] << 16) + (str[pos + 3] << 24));
                b += (uint)(str[pos + 4] + (str[pos + 5] << 8) + (str[pos + 6] << 16) + (str[pos + 7] << 24));
                c += (uint)(str[pos + 8] + (str[pos + 9] << 8) + (str[pos + 10] << 16) + (str[pos + 11] << 24));

                HashMix(ref a, ref b, ref c);

                pos += 12;
                remaining -= 12;
            }

            c += (uint)len;

            switch (remaining)
            {
                case 11: c += (uint)str[pos + 10] << 24; goto case 10;
                case 10: c += (uint)str[pos + 9] << 16; goto case 9;
                case 9: c += (uint)str[pos + 8] << 8; goto case 8;
                case 8: b += (uint)str[pos + 7] << 24; goto case 7;
                case 7: b += (uint)str[pos + 6] << 16; goto case 6;
                case 6: b += (uint)str[pos + 5] << 8; goto case 5;
                case 5: b += (uint)str[pos + 4]; goto case 4;
                case 4: a += (uint)str[pos + 3] << 24; goto case 3;
                case 3: a += (uint)str[pos + 2] << 16; goto case 2;
                case 2: a += (uint)str[pos + 1] << 8; goto case 1;
                case 1: a += (uint)str[pos]; break;
                default: break;
            }

            HashMix(ref a, ref b, ref c);

            return c;
        }
    }
}
