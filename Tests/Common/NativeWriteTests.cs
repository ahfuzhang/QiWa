using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

public class NativeWriteTests {
    [Fact]
    public void WriteStdout_WritesBytes_OnUnix() {
        if (!(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())) {
            return;
        }

        ReadOnlySpan<byte> payload = "native-write-test\n"u8;
        int readFd = -1;
        int writeFd = -1;
        int originalStdout = -1;
        bool stdoutRedirected = false;
        try {
            int[] fds = new int[2];
            Assert.Equal(0, pipe(fds));
            readFd = fds[0];
            writeFd = fds[1];

            originalStdout = dup(1);
            Assert.True(originalStdout >= 0);

            // Redirect stdout to a pipe so we can capture the native write.
            Assert.True(dup2(writeFd, 1) >= 0);
            stdoutRedirected = true;
            close(writeFd);
            writeFd = -1;

            Common.NativeWrite.WriteStdout(payload);

            Assert.True(dup2(originalStdout, 1) >= 0);
            stdoutRedirected = false;
            close(originalStdout);
            originalStdout = -1;

            using var buffer = new MemoryStream();
            byte[] chunk = new byte[256];
            unsafe {
                fixed (byte* chunkPtr = chunk) {
                    while (true) {
                        nint n = read(readFd, (IntPtr)chunkPtr, (nuint)chunk.Length);
                        if (n <= 0) {
                            break;
                        }
                        buffer.Write(chunk, 0, (int)n);
                    }
                }
            }

            Assert.True(buffer.ToArray().AsSpan().IndexOf(payload) >= 0);
        }
        finally {
            if (stdoutRedirected && originalStdout != -1) {
                dup2(originalStdout, 1);
            }
            if (originalStdout != -1) {
                close(originalStdout);
            }
            if (writeFd != -1) {
                close(writeFd);
            }
            if (readFd != -1) {
                close(readFd);
            }
        }
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int pipe(int[] fds);

    [DllImport("libc", SetLastError = true)]
    private static extern int dup(int oldfd);

    [DllImport("libc", SetLastError = true)]
    private static extern int dup2(int oldfd, int newfd);

    [DllImport("libc", SetLastError = true)]
    private static extern int close(int fd);

    [DllImport("libc", SetLastError = true)]
    private static extern nint read(int fd, IntPtr buf, nuint count);
}
