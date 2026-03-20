using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Common;

/// <summary>
/// 这个类是为了提供类似 golang 中的 defer 语句的效果
/// 也可以在代码中连续写多个 `using () using () {}` 来代替
/// </summary>
/// <example>
/// ```csharp
/// // 退出作用域时，自动调用 Cleanup() 函数。本质上是 try...finally 的另一个写法
/// using (var _ = new ScopeGuard(() => Cleanup(obj))){
///     Step1();
///     Step2();
/// }
/// ```
/// </example>
public ref struct ScopeGuard : IDisposable
{
    private Action? _onDispose;

    public ScopeGuard(Action onDispose)
    {
        _onDispose = onDispose;
    }

    public void Dispose()
    {
        _onDispose?.Invoke();
        _onDispose = null;
    }
}

/// <summary>
/// 封装一个 error 对象
/// </summary>
public struct Error
{
    public System.UInt32 Code;
    public string Message;

    /// <summary>
    /// 判断是否有错误发生
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Err()
    {
        return Code != 0;
    }

    /// <summary>
    /// 产生错误对象的时候，同时带上源码行的信息
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="message">错误信息</param>
    /// <param name="file">编译器提供</param>
    /// <param name="member">编译器提供</param>
    /// <param name="line">编译器提供</param>
    /// <returns>Error 对象</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error WithLoc(System.UInt32 code, string message,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0
    )
    {
        for (int i = file.Length - 1; i >= 0; i--)
        {
            if (file[i] == '/' || file[i] == '\\')
            {
                file = file.Substring(i + 1);
                break;
            }
        }
        return new Error { Code = code, Message = $"{file}:{line} ({member})\n\t Code={code},Message={message}" };
    }
}

public class Utils
{
    public static string GetExceptionLocation(Exception err)
    {
        if (err == null)
        {
            return string.Empty;
        }
        var trace = new StackTrace(err, true);
        var frames = trace.GetFrames();
        if (frames == null || frames.Length == 0)
        {
            return string.Empty;
        }
        foreach (var frame in frames)
        {
            var file = frame.GetFileName();
            var line = frame.GetFileLineNumber();
            if (!string.IsNullOrEmpty(file) && line > 0)
            {
                return $"file={file}, line={line}";
            }
        }
        return string.Empty;
    }
}
