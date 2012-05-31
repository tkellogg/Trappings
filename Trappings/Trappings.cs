using System;

namespace Trappings
{
    public sealed class Trappings : IDisposable
    {
        private readonly IDatabaseProvider db;

        private Trappings(IFixtureLoader fixtureLoader, IDatabaseProvider db)
        {
            this.db = db;
            var fixtures = fixtureLoader.GetFixtures();
            foreach(var fixture in fixtures)
                db.LoadFixtures(fixture);
        }

        public void Dispose()
        {
            db.Clear();
        }

        public static Trappings Create(Action<IFixtureFinder> configure)
        {
            var fixtureFinder = new FixtureFinder();
            configure(fixtureFinder);
            var fixtureLoader = new ClrFixtureLoader(fixtureFinder);
            var configuration = new Configuration();
            var dbProvider = new MongoDatabaseProvider(configuration);
            return new Trappings(fixtureLoader, dbProvider);
        }

        public static Trappings Create()
        {
            return Create(conf => { });
        }
    }
}
