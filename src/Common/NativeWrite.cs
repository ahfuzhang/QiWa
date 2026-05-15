using System.Runtime.InteropServices;

namespace Common;

public static class NativeWrite
{
    /// <summary>
    /// 使用 write 系统调用
    /// </summary>
    public static void WriteStdout(ReadOnlySpan<byte> data)
    {
#if WINDOWS
        WindowsWrite(data);
#elif UNIX  // /UNIX 宏同时对应 MacOS 和 Linux
        UnixWrite(data);
#else
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
