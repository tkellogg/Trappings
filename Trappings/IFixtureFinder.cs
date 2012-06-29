using System;
using System.Collections.Generic;

namespace Trappings
{
    public interface IFixtureFinder
    {
        IEnumerable<Type> GetTypes();
        IFixtureFinder Add(params Type[] types);
        IEnumerable<ITestFixtureData> GetFixtures();
        IFixtureFinder Add(ITestFixtureData fixture);
    }
}