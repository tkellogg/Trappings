using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class JsonFixtureLoaderTests
    {
        [Fact]
        public void Each_JSON_file_is_a_fixture()
        {
            var fileSystem = Mock.Of<IFileSystemProvider>(x => x.GetFiles() == new[]{"cars.json"});
            var loader = new JsonFixtureLoader(fileSystem);

            var containers = loader.GetFixtures().ToArray();
            containers.Length.ShouldEqual(1);
            containers[0].Name.ShouldEqual("cars");
        }

        [Fact]
        public void It_filters_out_files_that_arent_JSON()
        {
            var fileSystem = Mock.Of<IFileSystemProvider>(x => x.GetFiles() == new[]{"bats.yml"});
            var loader = new JsonFixtureLoader(fileSystem);

            var containers = loader.GetFixtures().ToArray();
            containers.ShouldBeEmpty();
        }

        [Fact]
        public void It_can_mix_n_match_file_types()
        {
            var fileSystem = Mock.Of<IFileSystemProvider>(x => x.GetFiles() == new[]{"cars.json", "bats.yml"});
            var loader = new JsonFixtureLoader(fileSystem);

            var containers = loader.GetFixtures().ToArray();
            containers.Length.ShouldEqual(1);
            containers[0].Name.ShouldEqual("cars");
        }

        [Fact]
        public void It_reads_each_item_in_the_file_into_a_fixture()
        {
            var document = @"
            {
                ""item1"": {
                    ""Make"": ""Chevy"",
                    ""Model"": ""Cruze""
                }
            }
            ";
            var fileSystem = Mock.Of<IFileSystemProvider>(x => x.GetFiles() == new[]{"cars.json"}
                && x.ReadFile("cars.json") == document);
            var loader = new JsonFixtureLoader(fileSystem);

            var containers = loader.GetFixtures().ToArray();
            containers[0].Fixtures.Count().ShouldEqual(1);
            containers[0].Fixtures.First().Name.ShouldEqual("item1");
            ((string)containers[0].Fixtures.First().Value.Make).ShouldEqual("Chevy");
            ((string)containers[0].Fixtures.First().Value.Model).ShouldEqual("Cruze");
        }

    }
}
