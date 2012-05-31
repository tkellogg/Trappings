using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Trappings
{
    public class MongoDatabaseProvider : IDatabaseProvider
    {
        public void LoadFixtures(FixtureContainer container)
        {
            var db = MongoDatabase.Create("mongodb://localhost/test");
            var collection = db.GetCollection<Dictionary<string,object>>(container.Name);
            foreach (var fixture in container.Fixtures)
            {
                Dictionary<string, object> converted = fixture.Value;
                converted["Id"] = BsonObjectId.GenerateNewId();
                collection.Save(converted);
            }
        }
    }
}