using System;

namespace Trappings
{
    public sealed partial class FixtureSession : IDisposable
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

        /// <summary>
        /// Creates a session with the given fixtures loaded into the database
        /// </summary>
        public static FixtureSession Create(params Type[] types)
        {
            return Create(conf => conf.Add(types));
        }
    }
}
