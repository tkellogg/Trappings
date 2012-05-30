using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Trappings
{
    internal class JsonFixtureLoader : IFixtureLoader
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
                           Name = NamePattern.Match(name).Groups[1].Value
                       };
        }
    }
}