using System.Collections.Generic;
using System.IO;
using System.Linq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class FileSystemProviderTests
    {
        [Fact]
        public void It_takes_a_directory_and_returns_all_files_in_that_directory()
        {
            var directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "test.txt"), "this is a test");
            File.WriteAllText(Path.Combine(directory, "test.yml"), "this: is a test");
            File.WriteAllText(Path.Combine(directory, "test.json"), @"{""this"": ""is a test""}");

            var provider = new FileSystemProvider(directory);
            IList<string> names = provider.GetFiles().OrderBy(x => x).ToList();
            names.Count.ShouldEqual(3);
            var expected = new[] {"test.json", "test.txt", "test.yml"}.Select(x => Path.Combine(directory, x)).ToArray();
            names.ShouldEqual(expected);
        }
    }
}
