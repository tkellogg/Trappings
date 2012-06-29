using System;
using System.Collections.Generic;
using System.Linq;

namespace Trappings
{
    internal class FixtureFinder : IFixtureFinder
    {
        private readonly List<Type> types = new List<Type>();
        private readonly List<ITestFixtureData> fixtures = new List<ITestFixtureData>();

        public IEnumerable<Type> GetTypes()
        {
            return types;
        }

        public IFixtureFinder Add(params Type[] types)
        {
            this.types.AddRange(types.Where(x => x != null));
            return this;
        }

        public IFixtureFinder Add(ITestFixtureData fixture)
        {
            fixtures.Add(fixture);
            return this;
        }

        public IEnumerable<ITestFixtureData> GetFixtures()
        {
            return fixtures;
        }
    }
}