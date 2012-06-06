using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
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

        public object GetId(object @object)
        {
            var type = @object.GetType();
            var property = type.GetProperty("Id");
            property = property ?? type.GetProperty("id");
            property = property ?? type.GetProperty("ID");
            property = property ?? type.GetProperty("_id");
            if (property == null)
                return GetIdFromDynamicObject(@object);


            return property.GetValue(@object, null);
        }

        private static object GetIdFromDynamicObject(object @object)
        {
            if (!(@object is DynamicObject || @object is ExpandoObject))
                throw new ArgumentException(string.Format("Couldn't find Id property for {0}", @object.GetType()));

            var id = TryGetMember(@object, o => o.Id);
            id = id ?? TryGetMember(@object, o => o.id);
            id = id ?? TryGetMember(@object, o => o._id);

            if (id == null)
                throw new ArgumentException(string.Format("Couldn't find Id property for {0}", @object.GetType()));
            return id;
        }

        private static object TryGetMember(dynamic @object, Func<dynamic, object> getter)
        {
            try
            {
                return getter(@object);
            }
            catch (RuntimeBinderException)
            {
                return null;
            }
        }
    }
}