using System;

namespace Trappings
{
    public sealed class FixtureSession : IDisposable
    {
        private readonly IDatabaseProvider db;
        private static bool _hasRunInitializers;

        private FixtureSession(IFixtureLoader fixtureLoader, IDatabaseProvider db)
        {
            this.db = db;
            var fixtures = fixtureLoader.GetFixtures();
            foreach(var fixture in fixtures)
                db.LoadFixtures(fixture);
        }

        public static string ConnectionString
        {
            get { return Configuration.ConnectionString; }
            set { Configuration.ConnectionString = value; }
        }

        public void Dispose()
        {
            db.Clear();
        }

        /// <summary>
        /// Add an object to the session that will be cleaned up at the end
        /// </summary>
        public void Track(object @object, string collectionName)
        {
            db.AddItemForCleanup(collectionName, @object);
        }

        public static FixtureSession Create(Action<IFixtureFinder> configure)
        {
            RunInitializersIfNeeded();
            var fixtureFinder = new FixtureFinder();
            configure(fixtureFinder);
            var fixtureLoader = new ClrFixtureLoader(fixtureFinder);
            var configuration = new Configuration();
            var dbProvider = new MongoDatabaseProvider(configuration);
            return new FixtureSession(fixtureLoader, dbProvider);
        }

        private static void RunInitializersIfNeeded()
        {
            if (!_hasRunInitializers)
                new SetupClassScanner().ScanForSetupTypes();

            _hasRunInitializers = true;
        }

        public static FixtureSession Create()
        {
            return Create(conf => { });
        }

        public static FixtureSession UseFixture<T>()
        {
            return Create(conf => conf.Add(typeof (T)));
        }

        public static FixtureSession UseFixtures<T1, T2>()
        {
            return Create(conf => conf.Add(typeof (T1), typeof (T2)));
        }

        public static FixtureSession UseFixtures<T1, T2, T3>()
        {
            return Create(conf => conf.Add(typeof (T1), typeof (T2), typeof (T3)));
        }

        public static FixtureSession UseFixtures<T1, T2, T3, T4>()
        {
            return Create(conf => conf.Add(typeof (T1), typeof (T2), typeof (T3), typeof (T4)));
        }
    }
}
