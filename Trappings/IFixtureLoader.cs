using System.Collections.Generic;

namespace Trappings
{
    internal interface IFixtureLoader
    {
        IEnumerable<FixtureContainer> GetFixtures();
    }
}