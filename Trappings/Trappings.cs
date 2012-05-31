using System;

namespace Trappings
{
    public sealed class Trappings : IDisposable
    {
        private readonly IFixtureFinder fixtureFinder;

        private Trappings(IFixtureLoader fixtureLoader, IDatabaseProvider db, IFixtureFinder fixtureFinder)
        {
            this.fixtureFinder = fixtureFinder;
            var fixtures = fixtureLoader.GetFixtures();
            foreach(var fixture in fixtures)
                db.LoadFixtures(fixture);
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
