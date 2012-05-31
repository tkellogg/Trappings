using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trappings
{
    public class ClrFixtureLoader : IFixtureLoader
    {
        private readonly IFixtureFinder fixtureFinder;

        public ClrFixtureLoader(IFixtureFinder fixtureFinder)
        {
            this.fixtureFinder = fixtureFinder;
        }

        public IEnumerable<FixtureContainer> GetFixtures()
        {
            foreach (var type in fixtureFinder.GetTypes())
            {
                foreach (var fieldInfo in type.GetFields(BindingFlags.Static| BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        var dictionary = fieldInfo.GetValue(null) as IDictionary;
                        if (dictionary == null)
                            continue;

                        yield return CreateFixtureContainerFromDictionary(fieldInfo.Name, dictionary);
                    }
                }
            }
        }

        private FixtureContainer CreateFixtureContainerFromDictionary(string name, IDictionary dictionary)
        {
            var container = new FixtureContainer
                                {
                                    Name = name,
                                    Fixtures = dictionary.Keys.Cast<string>()
                                        .Select(x => CreateFixture(x, dictionary[x]))
                                };

            return container;
        }

        private Fixture CreateFixture(string key, object value)
        {
            return new Fixture
                       {
                           Name = key,
                           Value = value
                       };
        }
    }
}
