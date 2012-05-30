using System;
using Dynamo.Ioc;

namespace Trappings
{
    public sealed class Trappings : IDisposable
    {
        public Trappings(IFixtureLoader fixtureLoader, IDatabaseProvider db)
        {
            var fixtures = fixtureLoader.GetFixtures();
            foreach(var fixture in fixtures)
                db.LoadFixtures(fixture);
        }

        public static Trappings Create()
        {
            var container = new Container();
            container.Register<IConfiguration, Configuration>();
            container.Register<IFixtureLoader, JsonFixtureLoader>();
            container.Register<IDatabaseProvider, MongoDatabaseProvider>();
            container.Register<IFileSystemProvider, FileSystemProvider>();
            container.Register<Trappings, Trappings>();
            container.Compile();
            return container.Resolve<Trappings>();
        }

        public void Dispose()
        {
        }
    }
}
