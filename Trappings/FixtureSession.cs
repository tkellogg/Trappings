using System;
using System.Collections.Generic;
using MongoDB.Bson;

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

        public static string ConnectionString
        {
            get { return Configuration.ConnectionString; }
            set { Configuration.ConnectionString = value; }
        }

        public static FixtureSession Create(params SetupObject[] objects)
        {
            return Create(conf => conf.Add(new TestFixtureData(objects)));
        }

        /// <summary>
        /// Creates a session with the given fixtures loaded into the database
        /// </summary>
        public static FixtureSession Create(Type[] types)
        {
            return Create(conf => conf.Add(types));
        }

        #region Deprecated APIs

        [Obsolete("Use Create<T1>() instead")]
        public static FixtureSession UseFixture<T>()
        {
            return Create<T>();
        }

        [Obsolete("Use Create<T1, T2>() instead")]
        public static FixtureSession UseFixtures<T1, T2>()
        {
            return Create<T1, T2>();
        }

        [Obsolete("Use Create<T1, T2, T3>() instead")]
        public static FixtureSession UseFixtures<T1, T2, T3>()
        {
            return Create<T1, T2, T3>();
        }

        #endregion

        public static bool HasActiveSessions { get { return instanceCount > 0; } }

        /// <summary>
        /// Gets an object directly out of the database
        /// </summary>
        /// <typeparam name="TModel">the Type of the collection</typeparam>
        /// <param name="collectionName">The name of the collection</param>
        /// <param name="id">The ID value. The document in the collection MUST have an _id field</param>
        public TModel GetFromDb<TModel>(string collectionName, object id)
        {
            return db.GetById<TModel>(collectionName, id);
        }
    }
}
