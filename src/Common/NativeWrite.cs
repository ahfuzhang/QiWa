using System;
using System.Runtime.InteropServices;

namespace Common {
    public static class NativeWrite {
        public static void WriteStdout(ReadOnlySpan<byte> data) {
            //Console.WriteLine($"ready write len: {data.Length}");
#if WINDOWS
            WindowsWrite(data);
#elif UNIX
            UnixWrite(data);
#else
            //Console.WriteLine("PlatformNotSupportedException");
            throw new PlatformNotSupportedException();
#endif
        }

#if UNIX
        [DllImport("libc", SetLastError = true)]
        private static extern long write(int fd, IntPtr buf, ulong count);

        private static void UnixWrite(ReadOnlySpan<byte> data)
        {
            unsafe {
                fixed (byte* ptr = data) {
                    write(1, (IntPtr)ptr, (ulong)data.Length);
                    //Console.WriteLine($"write len: {data.Length}");
                }
            }
        }
#endif

#if WINDOWS
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(
            IntPtr hFile,
            IntPtr buffer,
            uint nBytes,
            out uint written,
            IntPtr overlapped);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        private static void WindowsWrite(ReadOnlySpan<byte> data)
        {
            unsafe {
                fixed (byte* ptr = data) {
                    WriteFile(GetStdHandle(-11), (IntPtr)ptr, (uint)data.Length, out _, IntPtr.Zero);
                }
            }
        }
#endif
    }
}


// static class Syscall {
//     [DllImport("libc", SetLastError = true)]
//     public static extern long write(int fd, byte[] buf, ulong count);
// }

