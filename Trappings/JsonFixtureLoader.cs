using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Codeplex.Data;

namespace Trappings
{
    public class JsonFixtureLoader : IFixtureLoader
    {
        private readonly IFileSystemProvider fileSystem;
        private static readonly Regex NamePattern = new Regex(@"(.*)\.json", RegexOptions.IgnoreCase);

        public JsonFixtureLoader(IFileSystemProvider fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public IEnumerable<FixtureContainer> GetFixtures()
        {
            return fileSystem.GetFiles()
                .Where(name => NamePattern.IsMatch(name))
                .Select(CreateFixtureContainer);
        }

        private FixtureContainer CreateFixtureContainer(string name)
        {
            return new FixtureContainer
                       {
                           Name = NamePattern.Match(name).Groups[1].Value,
                           Fixtures = ReadFixtures(fileSystem.ReadFile(name))
                       };
        }

        private IEnumerable<Fixture> ReadFixtures(string fileContents)
        {
            dynamic fixtures = DynamicJson.Parse(fileContents);
            foreach (var pair in fixtures)
            {
                yield return new Fixture {Name = pair.Key, Value = pair.Value};
            }
        }
    }
}