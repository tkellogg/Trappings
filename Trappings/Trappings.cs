using System;
using Dynamo.Ioc;

namespace Trappings
{
    public sealed class Trappings : IDisposable
    {
        private readonly IFixtureFinder fixtureFinder;

        public Trappings(IFixtureLoader fixtureLoader, IDatabaseProvider db, IFixtureFinder fixtureFinder)
        {
            this.fixtureFinder = fixtureFinder;
            var fixtures = fixtureLoader.GetFixtures();
            foreach(var fixture in fixtures)
                db.LoadFixtures(fixture);
        }

        private static Container _container;
        private static Container Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new Container();
                    _container.Register<IConfiguration, Configuration>();
                    _container.Register<IFixtureLoader, ClrFixtureLoader>();
                    _container.Register<IFixtureFinder, FixtureFinder>();
                    _container.Register<IDatabaseProvider, MongoDatabaseProvider>();
                    _container.Register<IFileSystemProvider, FileSystemProvider>();
                    _container.Register<Trappings, Trappings>();
                    _container.Compile();
                }
                return _container;
            }
        }

        public void Dispose()
        {
        }

        public static Trappings Create(Action<IFixtureFinder> configure)
        {
            var fixtureFinder = new FixtureFinder();
            configure(fixtureFinder);
            var fixtureLoader = new ClrFixtureLoader(fixtureFinder);
            var dbProvider = new MongoDatabaseProvider();
            return new Trappings(fixtureLoader, dbProvider, fixtureFinder);
        }

        public static Trappings Create()
        {
            return Create(conf => { });
        }
    }
}
