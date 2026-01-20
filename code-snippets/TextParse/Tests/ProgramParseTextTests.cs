using System.Collections.Generic;
using System.Text;
using Xunit;

public class ProgramParseTextTests {
    private struct TestCase {
        public string Name;
        public string Input;
        public Dictionary<string, string> Tags;
        public string Expected;
    }

    [Fact]
    public void ParseText_TableDriven() {
        var cases = new[] {
            new TestCase {
                Name = "basic",
                Input = "# HELP foo\nfoo 1\nbar{code=\"200\"} 2",
                Tags = new Dictionary<string, string> {
                    ["env"] = "prod",
                    ["node"] = "n1",
                },
                Expected = "foo{env=\"prod\",node=\"n1\"} 1\nbar{env=\"prod\",node=\"n1\", code=\"200\"} 2",
            },
            new TestCase {
                Name = "leading whitespace and blank line",
                Input = "  baz 3\n\n# comment\nqux 4",
                Tags = new Dictionary<string, string> {
                    ["env"] = "prod",
                    ["node"] = "n1",
                },
                Expected = "  baz{env=\"prod\",node=\"n1\"} 3\n\nqux{env=\"prod\",node=\"n1\"} 4",
            },
            new TestCase {
                Name = "escape tag values",
                Input = "foo 1",
                Tags = new Dictionary<string, string> {
                    ["path"] = "a\\b",
                    ["note"] = "x\"y",
                },
                Expected = "foo{note=\"x\\\"y\",path=\"a\\\\b\"} 1",
            },
        };
        //Console.WriteLine("ParseText_TableDriven:");
        foreach (var testCase in cases) {
            byte[] input = Encoding.UTF8.GetBytes(testCase.Input);
            byte[] output = Program.parseText(input, testCase.Tags);
            string actual = Encoding.UTF8.GetString(output);
            Assert.True(actual == testCase.Expected, $"case={testCase.Name}, expected={testCase.Expected}, actual={actual}");
        }
    }
}
