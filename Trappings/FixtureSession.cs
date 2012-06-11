﻿using System;

namespace Trappings
{
    public sealed partial class FixtureSession : IDisposable
    {
        private readonly IDatabaseProvider db;
        private static bool _hasRunInitializers;
        private static int instanceCount;

        private FixtureSession(IFixtureLoader fixtureLoader, IDatabaseProvider db)
        {
            this.db = db;
            var fixtures = fixtureLoader.GetFixtures();
            foreach(var fixture in fixtures)
                db.LoadFixtures(fixture);

            instanceCount++;
        }

        public void Dispose()
        {
            instanceCount--;
            db.Clear();
        }

        /// <summary>
        /// Add an object to the session that will be cleaned up at the end
        /// </summary>
        public void Track(object @object, string collectionName)
        {
            db.AddItemForCleanup(collectionName, @object);
        }

        /// <remarks>
        /// I downgraded this to internal because it seems like uneccessary API surface area. Lets
        /// keep it small for as long as we can
        /// </remarks>
        internal static FixtureSession Create(Action<IFixtureFinder> configure)
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

        public static bool HasActiveSessions { get { return instanceCount > 0; } }
    }
}
