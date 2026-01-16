using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

internal static class Program {
    public static async Task Main(string[] args) {
        var shutdownSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        Console.CancelKeyPress += (_, eventArgs) => {
            eventArgs.Cancel = true;
            shutdownSignal.TrySetResult(true);
        };
        using var sigtermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, context => {
            context.Cancel = true;
            shutdownSignal.TrySetResult(true);
        });
        Console.WriteLine("programe started!");
        await shutdownSignal.Task;
        OnShutdown();
        return;
    }

    private static void OnShutdown() {
        Console.WriteLine("SIGTERM received, shutting down...");
        Console.Out.Flush();
    }
}
