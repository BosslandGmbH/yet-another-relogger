using System;
using System.Diagnostics;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers
{
    public static class CrashChecker
    {
        public static bool IsResponding(Process proc)
        {
            if (proc == null)
                return false;
            return (TestResponse(proc.MainWindowHandle));
        }

        public static bool IsResponding(IntPtr handle)
        {
            return (TestResponse(handle));
        }

        private static bool TestResponse(IntPtr handle)
        {
            UIntPtr dummy;

            var result = WinApi.SendMessageTimeout(handle, 0, UIntPtr.Zero, IntPtr.Zero,
                WinApi.SendMessageTimeoutFlags.SmtoAbortifhung, 1000, out dummy);

            return (result != IntPtr.Zero);
        }
    }
}
