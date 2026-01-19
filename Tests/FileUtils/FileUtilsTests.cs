using Xunit;



public class UtilsTests {
    private static string GetDataPath(string fileName) {
        return Path.Combine(AppContext.BaseDirectory, "data", fileName);
    }

    struct TestTableForFileExistsAndNotEmptyAsync {
        public string Name;
        public string FilePath;
        public bool Expected;
    };

    [Fact]
    public async Task FileExistsAndNotEmptyAsyncTest() {
        TestTableForFileExistsAndNotEmptyAsync[] table = {
            new TestTableForFileExistsAndNotEmptyAsync{Name="happy path",FilePath=GetDataPath("metrics.txt"), Expected=true},
            new TestTableForFileExistsAndNotEmptyAsync{Name="file not exists",FilePath=GetDataPath("metrics.txt.abc"), Expected=false},
            new TestTableForFileExistsAndNotEmptyAsync{Name="empty file", FilePath=GetDataPath("empty.txt"), Expected=false},
            new TestTableForFileExistsAndNotEmptyAsync{Name="dir not exists",FilePath=Path.Combine(AppContext.BaseDirectory, "data1", "metrics.txt"), Expected=false},
        };
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
                inputFile=GetDataPath("metrics.txt"),
                Expected = {
                    Item1=new Common.RentedBuffer{},
                    Item2=new Common.Error{Code=0}
                }
            },
            new testTableFForReadAll{
                Name="empty file",
                inputFile=GetDataPath("empty.txt"),
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
