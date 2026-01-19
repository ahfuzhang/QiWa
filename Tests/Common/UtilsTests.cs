
using System.Runtime.CompilerServices;
using Xunit;

public class ScopeGuardTests {
    [Fact]
    public void UseScope()
    {
        var notCleanup = true;
        {
            using (var _ = new Common.ScopeGuard(() => {notCleanup = false;})) {
                Console.WriteLine("biz logic");
            }
        }
        Assert.False(notCleanup);
    }
}

public class ErrorTests {
    [Fact]
    public void HasError() {
        Common.Error err = default;
        Assert.False(err.Err());
        Common.Error err1 = new Common.Error{Code=1, Message="err happend"};
        Assert.True(err1.Err());
    }
}

public class RentedBuffer {
    [Fact]
    public void Rent() {
        Common.RentedBuffer buffer = default;
        var span1 = buffer.Bytes();
        Assert.Equal(0, span1.Length);
        int bytes = System.Random.Shared.Next(100, 63336);
        buffer.Rent(bytes);
        Assert.NotNull(buffer.Data);
        Assert.True(buffer.Data.Length>=bytes);
        Assert.True(buffer.Length==bytes);
        ReadOnlySpan<byte> src = "hello\n"u8;
        src.CopyTo(buffer.Data);
        var span2 = buffer.Bytes();
        Assert.Equal(buffer.Length, span2.Length);
        Assert.Equal((byte)'h', span2[0]);
        buffer.Dispose();
        Assert.Null(buffer.Data);
        Assert.Equal<int>(0, buffer.Length);
    }
}
