using System.Collections.Generic;

namespace Trappings
{
    public interface IFixtureLoader
    {
        IEnumerable<FixtureContainer> GetFixtures();
    }
}