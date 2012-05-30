using MongoDB.Driver;

namespace Trappings.Tests
{
    public static class TestUtils
    {
        public static MongoCollection<T> GetCollection<T>(string name)
        {
            var db = MongoDatabase.Create("mongodb://localhost/test");
            return db.GetCollection<T>(name);
        }
    }
}