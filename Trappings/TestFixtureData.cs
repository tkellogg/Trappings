using System.Collections.Generic;

namespace Trappings
{
    public class TestFixtureData : ITestFixtureData
    {
        private readonly IEnumerable<SetupObject> objects;

        public TestFixtureData(IEnumerable<SetupObject> objects)
        {
            this.objects = objects;
        }

        public IEnumerable<SetupObject> Setup()
        {
            return objects;
        }
    }
}