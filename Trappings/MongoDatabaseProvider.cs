using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Trappings
{
    internal class MongoDatabaseProvider : IDatabaseProvider
    {
        private readonly IConfiguration configuration;
        private readonly MongoDatabase db;
        private readonly Dictionary<string, List<IMongoQuery>> savedObjectIds;

        public MongoDatabaseProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
            db = MongoDatabase.Create(configuration.ConnectionString);
            savedObjectIds = new Dictionary<string, List<IMongoQuery>>();
        }

        public void LoadFixtures(FixtureContainer container)
        {
            var collection = GetCollection(container.Name);
            foreach (var fixture in container.Fixtures)
            {
                collection.Save(fixture.Value);
                AddItemForCleanup(container.Name, fixture.Value);
            }
        }

        public void AddItemForCleanup(string collectionName, object item)
        {
            var id = GetId(item);
            var query = Query.EQ("_id", BsonValue.Create(id));
            AddCleanupQuery(collectionName, query);
        }

        private MongoCollection<Dictionary<string, object>> GetCollection(string name)
        {
            return db.GetCollection<Dictionary<string,object>>(name);
        }

        private void AddCleanupQuery(string name, IMongoQuery query)
        {
            if (!savedObjectIds.ContainsKey(name))
                savedObjectIds[name] = new List<IMongoQuery>();

            savedObjectIds[name].Add(query);
        }

        public void Clear()
        {
            foreach (var objectPair in savedObjectIds)
            {
                var collection = GetCollection(objectPair.Key);
                var queryies = objectPair.Value;
                foreach(var query in queryies)
                    collection.Remove(query, RemoveFlags.None);
            }
        }

        private object GetId(object @object)
        {
            var property = @object.GetType().GetProperty("Id");
            if (property == null)
                return null;

            return property.GetValue(@object, null);
        }
    }
}