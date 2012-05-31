using System;
using System.Linq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class FixtureFinderTests
    {
        [Fact]
        public void You_can_use_Add_to_add_types_to_the_list()
        {
            var finder = new FixtureFinder();
            var types = finder.Add(typeof (string), typeof (Guid)).GetTypes();
            types.ToArray().ShouldEqual(new[] {typeof (string), typeof(Guid)});
        }

        [Fact]
        public void It_ignores_null_types_passed_in()
        {
            var finder = new FixtureFinder();
            var types = finder.Add(typeof (string), null).GetTypes();
            types.ToArray().ShouldEqual(new[] {typeof (string)});
        }
    }
}
