using System.Collections.Generic;

namespace Trappings
{
    internal class FixtureContainer
    {
        public string Name { get; set; }
        public IEnumerable<Fixture> Fixtures { get; set; }
    }
}