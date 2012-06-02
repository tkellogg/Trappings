using System;

namespace Trappings
{
    public sealed class FixtureManager : IDisposable
    {
        private readonly IDatabaseProvider db;
        private static bool _hasRunInitializers;

        private FixtureManager(IFixtureLoader fixtureLoader, IDatabaseProvider db)
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

        public static FixtureManager Create(Action<IFixtureFinder> configure)
        {
            RunInitializersIfNeeded();
            var fixtureFinder = new FixtureFinder();
            configure(fixtureFinder);
            var fixtureLoader = new ClrFixtureLoader(fixtureFinder);
            var configuration = new Configuration();
            var dbProvider = new MongoDatabaseProvider(configuration);
            return new FixtureManager(fixtureLoader, dbProvider);
        }

        private static void RunInitializersIfNeeded()
        {
            if (!_hasRunInitializers)
                new SetupClassScanner().ScanForSetupTypes();

            _hasRunInitializers = true;
        }

        public static FixtureManager Create()
        {
            return Create(conf => { });
        }

        public static FixtureManager UseFixture<T>()
        {
            return Create(conf => conf.Add(typeof (T)));
        }

        public static FixtureManager UseFixtures<T1, T2>()
        {
            return Create(conf => conf.Add(typeof (T1), typeof (T2)));
        }

        public static FixtureManager UseFixtures<T1, T2, T3>()
        {
            return Create(conf => conf.Add(typeof (T1), typeof (T2), typeof (T3)));
        }

        public static FixtureManager UseFixtures<T1, T2, T3, T4>()
        {
            return Create(conf => conf.Add(typeof (T1), typeof (T2), typeof (T3), typeof (T4)));
        }
    }
}
