using System;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class FixtureFinderTests
    {
        readonly FixtureFinder finder = new FixtureFinder();

        [Fact]
        public void You_can_use_Add_to_add_types_to_the_list()
        {
            var types = finder.Add(typeof (string), typeof (Guid)).GetTypes();
            types.ToArray().ShouldEqual(new[] {typeof (string), typeof(Guid)});
        }

        [Fact]
        public void It_ignores_null_types_passed_in()
        {
            var types = finder.Add(typeof (string), null).GetTypes();
            types.ToArray().ShouldEqual(new[] {typeof (string)});
        }

        [Fact]
        public void It_adds_Fixtures()
        {
            finder.Add(Mock.Of<ITestFixtureData>());
            finder.GetFixtures().Count().ShouldEqual(1);
        }
    }
}
