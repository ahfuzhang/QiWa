using Xunit;



public class UtilsTests {

    struct TestTableForFileExistsAndNotEmptyAsync {
        public string Name;
        public string FilePath;
        public bool Expected;
    };

    [Fact]
    public async Task FileExistsAndNotEmptyAsyncTest() {
        TestTableForFileExistsAndNotEmptyAsync[] table = {
            new TestTableForFileExistsAndNotEmptyAsync{Name="happy path",FilePath="../../../test/data/metrics.txt", Expected=true},
            new TestTableForFileExistsAndNotEmptyAsync{Name="file not exists",FilePath="../../../test/data/metrics.txt.abc", Expected=false},
            new TestTableForFileExistsAndNotEmptyAsync{Name="empty file", FilePath="../../../test/data/empty.txt", Expected=false},
            new TestTableForFileExistsAndNotEmptyAsync{Name="dir not exists",FilePath="../../../test/data1/metrics.txt", Expected=false},
        };
        Console.WriteLine("----------------------------------" + Directory.GetCurrentDirectory());
        Task<bool>[] tasks = new Task<bool>[table.Length];
        for (int i = 0; i < table.Length; i++) {
            tasks[i] = FileUtils.Utils.FileExistsAndNotEmptyAsync(table[i].FilePath);
        }
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var results = await Task.WhenAll(tasks).WaitAsync(cts.Token);
        for (int i = 0; i < results.Length; i++) {
            Assert.True(results[i] == table[i].Expected,
                $"Test={table[i].Name}, expected={table[i].Expected}, actual={results[i]}");
        }
        // const string path = "test/data/metrics.txt";
        // bool exists = await FileUtils.Utils.FileExistsAndNotEmptyAsync(path);
        // Assert.True(exists);
        // bool willNotExists = await FileUtils.Utils.FileExistsAndNotEmptyAsync(path+".abc");
        // Assert.False(willNotExists);
        // const string emptyFile = "test/data/empty.txt";
        // bool empty = await FileUtils.Utils.FileExistsAndNotEmptyAsync(emptyFile);
        // Assert.False(empty);
        // //
        // const string dirNotExists = "test/data1/metrics.txt";
        // exists = await FileUtils.Utils.FileExistsAndNotEmptyAsync(dirNotExists);
        // Assert.False(exists);
    }

    struct testTableFForReadAll {
        public string Name;
        public string inputFile;
        public System.ValueTuple<Common.RentedBuffer, Common.Error> Expected;
    }

    [Fact]
    public async Task ReadAllFileContent() {
        var table = new testTableFForReadAll[] {
            new testTableFForReadAll{
                Name="happy path",
                inputFile="../../../test/data/metrics.txt",
                Expected = {
                    Item1=new Common.RentedBuffer{},
                    Item2=new Common.Error{Code=0}
                }
            },
            new testTableFForReadAll{
                Name="empty file",
                inputFile="../../../test/data/empty.txt",
                Expected = {
                    Item1=new Common.RentedBuffer{},
                    Item2=new Common.Error{Code=2}
                }
            },
        };
        Task<System.ValueTuple<Common.RentedBuffer, Common.Error>>[] tasks = new Task<System.ValueTuple<Common.RentedBuffer, Common.Error>>[table.Length];
        for (int i = 0; i < table.Length; i++) {
            tasks[i] = FileUtils.Utils.ReadAllAndRentAync(table[i].inputFile);
        }
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var results = await Task.WhenAll(tasks).WaitAsync(cts.Token);
        for (int i = 0; i < results.Length; i++) {
            Assert.True(results[i].Item2.Code == table[i].Expected.Item2.Code,
                $"Test={table[i].Name}, expected={table[i].Expected.Item2.Code}, actual={results[i].Item2.Code}");
            results[i].Item1.Dispose();
        }
    }
}
