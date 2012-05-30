using MongoDB.Driver;

namespace Trappings
{
    public class MongoDatabaseProvider : IDatabaseProvider
    {
        public void LoadFixtures(FixtureContainer container)
        {
            var db = MongoDatabase.Create("mongodb://localhost/test");
            var collection = db.GetCollection<dynamic>(container.Name);
            foreach (var fixture in container.Fixtures)
            {
                collection.Save(fixture.Value);
            }
        }
    }
}