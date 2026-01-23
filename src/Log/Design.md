
* 目标：一个 C# 语言的高性能日志库
* 特性：
  - 高性能
  - 带缓冲的日志：日志内容先追加到一个 buffer 中
    - buffer 达到一定大小时，一次性写入目标位置
    - 达到一定时间后，一次性写入目标位置
  - 只支持 JSON 格式
    - 采用字符串拼接的方式来构造 JSON，不依赖第三方 JSON 库
  - 目标位置:
    - 默认写到 stdout
    - 当命令行配置 VictoriaLogs 的 jsonline 地址时，把日志数据进行 zstd 压缩，然后通过 http post 发送到 VictoriaLogs 的 jsonline 地址
  - thread local buffer
    - 每个线程有一个 buffer，避免加锁
    - 当使用 VictoriaLogs 的 jsonline 地址时，每个 buffer 独自进行 zstd 压缩，然后通过 http post 发送到 VictoriaLogs 的 jsonline 地址
    - 降级：当 http post 发送失败时，把日志数据写到 stdout
    - 当日志写到 stdout 时，检查一个全局的 atomic int，如果这个 int 等于 0， 则使用 cas 指令交换为 1, 然后写入到 stdout，写完后 cas 交换为 0
      - 当 全局的 atomic int 为 1， 说明 stdout 已经被占用。 这时候把日志 buffer  通过 channel 发送出去。然后申请一块新的内存，继续写日志。
  - 全局的日志消费者：
    - 负责从 channel 中读取日志 buffer，然后写入到 stdout
    - 写完日志后，对应的 buffer 被 return 回内存池。
* 设计细节
  - 对象分为三类:
    - 全局对象:
      - 一个全局的 channel，每个 thread 都可以作为生产者，把日志的 buffer 发送到 channel
        - channel 的 Item 是 RentedBuffer<byte>
      - 一个全局的 atomic int, 用于记录当前的 stdout 是否被占用
      - 全局的数组：用于把 thread local 的引用收集起来，然后进行遍历(比如检查 buffer 中的日志是否超过了 flush 时间)
      - 全局 timer: 定时扫描所有的 thread local buffer，如果 buffer 中的日志超过了 flush 时间，则把 buffer 发送到 channel
      - 以上的全局对象封装到一个对象中组合起来，避免使用时要分别初始化
    - thread local 对象
      - 主要是一个 Common.RentedBuffer 对象
    - task 函数内的对象
      - 保存一个当前上下文的 tag 的前缀。例如，我希望从当前上下文开始，每行日志始终有 tag : "func":"myfunc", "trace_id":"xxxx"
  -  内存池:
    - 使用已经实现的 ./src/Common/RentedBuffer 来实现内存的分配和释放。
  - 常量配置:
    - thread local buffer 的大小: 128KB
    - flush 时间: 1000 ms
    - channel 的大小: 1024
    - 全局数组的大小: 与 thread 的数量相同。thread local 的对象使用懒加载，等到有打日志的 api 被调用时，才会把 thread local 的对象加入到全局数组中。当 thread 退出时，需要把 thread local 的对象从全局数组中移除。
  - 全局初始化参数:
    - jsonline addr: 一个网址，例如 "http://[IP_ADDRESS]/jsonline"
    - 全局的日志 tag: 字符串，例如 "service":"my-service"。这个 tag 会被加到每一行日志的开头。
  - 日志级别：
    - fatal
    - error
    - warn
    - info
    - debug
    - trace
* 参考代码:
  - 定时器: 

        ```csharp
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        _ = Task.Run(async () =>
        {
            while (await timer.WaitForNextTickAsync())
            {
                DoWork();
            }
        });
        ```
    
* 对象设计
  - Logger: 全局日志对象，全局只创建一个实例。内部包含了全局的 channel，全局的 atomic int，全局的数组，全局的 timer
  - ThreadLocalLogger: thread local 对象，每个 thread 都有一个实例。内部包含了 thread local 的 RentedBuffer<byte>
  - TaskLogger: task 函数内的对象，每个 task 都有一个实例。内部包含了 task 的 tag 前缀. TaskLogger 通过 ThreadLocalLogger 来创建
    - TaskLogger 可以再创建 TaskLogger，新的 TaskLogger 在之前的 tag 前缀的基础上，增加新的 tag 前缀。
    - 可以写到日志的数据类型一般是：
      - string: 日志函数内部要立即转换为 utf-8. 要考虑转义的问题，以免无法拼装为合法的 json
      - Span<byte>  要考虑转义的问题，以免无法拼装为合法的 json
      - bool: 格式化为字符串 true/false
      - Int64 
      - Uint64 
      - Float64 
      - DateTime: 转换为 utc 时间，格式化为 "yyyy-MM-ddTHH:mm:ss.fffffffZ"
      - 代表为 RawJson 的 string: 把 string 转换为 utf-8 后，直接加到 buffer 中， 不转义，不加引号
      - 代表为 RawJson 的 Span<byte>: 直接加到 buffer 中， 不转义，不加引号
  - LogField 对象：
    - 用于描述日志的 tag
    - 内部有如下字段：
      - name: 字段的名字
      - data type: 数据类型 
      - value: 把 struct 构造成一个 union, 可以支持的数据类型有: string, Span<byte>, bool, Int64, Uint64, Float64, DateTime (RawJson string 其实是 string, RawJson Span<byte> 其实是 Span<byte>)
* 风格
  - 模仿 golang 的 zaplog 库
  - 尽量使用值类型，避免在堆上创建对象
  - 始终使用字符串追加的模式，而不是形成一个对象 DOM 后再序列化
  - 使用 Span<byte> 来进行字符串的追加，使用 "const"u8 这样的字符串。避免使用原生的 unicode 的 string 类型    

* LogField 详细设计

```csharp
enum FieldDataType{
    String,
    SpanByte,
    Bool,
    Int64,
    Uint64,
    Float64,
    DateTime,
    RawJsonString,
    RawJsonSpanByte,
};


public struct LogField{
    public string Name;
    public FieldDataType DataType;

    unoin Value;  // todo: 使用结构体布局来构造一个 union

    // 提供类似的很多个静态函数，来构造各种不同的 field
    public static LogField StringField(string name, string value){
        return new LogField{Name=name, DataType=String, Value={StringOffset=value}};
    }
};
```

* TaskLogger 对象的构造:

```csharp
public class ThreadLocalLogger{
    public TaskLogger New(){
        // 返回一个 TaskLogger 对象
    }
}

using System.Runtime.CompilerServices;

public class TaskLogger{
    public TaskLogger WithField(LogField field1){
        // todo: 内部根据 field1 构造好一个 json 格式的前缀，保存在对象内部，然后返回一个新的 TaskLogger
    }

    public TaskLogger WithField(LogField field1, LogField field2){
        // todo: 内部根据 field1 构造好一个 json 格式的前缀，保存在对象内部，然后返回一个新的 TaskLogger
        // todo: 重载 20 次，从 1 个 LogField 到 20 个 LogField 都支持
    }

    public void Info(LogField field1,
                    // 函数的最后三个参数都是由编译器生成的行号信息。
                    [CallerFilePath] string file = "",
                    [CallerMemberName] string member = "",
                    [CallerLineNumber] int line = 0
                    ){
        // todo: 把 field1 格式化为 json 中的 "name":"value"，然后追加到 thread local 的 buffer 中
        // CallerFilePath, CallerMemberName, CallerLineNumber 也需要加入日志中
    }

    public void Info(
                    LogField field1,
                    LogField field2,  // todo: 重载 20 次，从 1 个 LogField 到 20 个 LogField 都支持
                    // 函数的最后三个参数都是由编译器生成的行号信息。
                    [CallerFilePath] string file = "",
                    [CallerMemberName] string member = "",
                    [CallerLineNumber] int line = 0
                    ){
        // todo: 把 field1 格式化为 json 中的 "name":"value"，然后追加到 thread local 的 buffer 中
        // CallerFilePath, CallerMemberName, CallerLineNumber 也需要加入日志中
    }    

* 内部的 json 字段名约定:
  - _time: 日志的时间戳，格式为 "yyyy-MM-ddTHH:mm:ss.fffffffZ"
  - _level: 日志的级别，例如 "info"
  - _file: 日志的文件名
  - _member: 日志的函数名
  - _line: 日志的行号


