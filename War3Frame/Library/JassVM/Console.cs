using System.Runtime.InteropServices;

namespace War3Frame
{
    public partial class War3
    {
        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AllocConsole();

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool FreeConsole();

        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetConsoleTitleW(string lpConsoleTitle);

        public static bool EnableConsole(string? title = null)
        {
            FreeConsole();
            if (!AllocConsole())
                return false;
            if (!string.IsNullOrEmpty(title))
                SetConsoleTitleW(title);
            return true;
        }
    }
}
