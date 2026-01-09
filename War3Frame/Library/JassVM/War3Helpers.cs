using System.Text;

namespace War3Frame
{
    public partial class War3
    {
        private enum TypeVariable
        {
            Nothing = 0,
            Offset,
            Null,
            Code,
            Integer,
            Real,
            String,
            Handle,
            Boolean,
            IntegerArray,
            RealArray,
            StringArray,
            HandleArray,
            BooleanArray,
        }

        private static uint GetHashKey(string str)
        {
            return Storm.StringHash(str);
        }

        private static uint GetHashKey(int Int)
        {
            uint[] hashKeys = [
                0x98AA3D0C, 0xF67BCA9E, 0xC46CA84C, 0x2AC9D845,
                0x9A1CF1DD, 0x6450148E, 0x8516213D, 0xC0882BBF,
                0xF10C2A9C, 0x9D7CF013, 0xCD845F5E, 0x1D4BD837,
                0x1055F69A, 0xA6A87DCD, 0x312D8D9E, 0x645A1CEC
            ];

            uint hash = 0x7FED7FED;
            uint acc = 0xEEEEEEEE;
            int value = Int;

            for (int i = 0; i < 4 && value != 0; i++)
            {
                byte b = (byte)(value & 0xFF);
                uint high = (uint)((b >> 4) & 0x0F);
                uint low = (uint)(b & 0x0F);

                hash += acc;
                hash ^= (hashKeys[high] - hashKeys[low]);
                acc = hash + 3 + ((acc << 5) + b + acc);
                value >>= 8;
            }

            return hash;
        }

        private static byte[] GetUtf8BytesWithNull(string text)
        {
            int byteCount = Encoding.UTF8.GetByteCount(text);
            byte[] buffer = new byte[byteCount + 1];
            Encoding.UTF8.GetBytes(text, 0, text.Length, buffer, 0);
            buffer[byteCount] = 0;
            return buffer;
        }

        private static unsafe bool CStringCompare(byte* str1, byte* str2)
        {
            if (str1 == null || str2 == null)
                return str1 == null && str2 == null;

            if (str1 == str2) return true;

            while (*str1 != 0 && *str2 != 0)
            {
                if (*str1 != *str2)
                    return false;
                str1++;
                str2++;
            }

            return *str1 == 0 && *str2 == 0;
        }
    }
}