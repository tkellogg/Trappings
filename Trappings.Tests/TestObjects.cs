using MongoDB.Bson;

namespace Trappings.Tests
{
    public class Car
    {
        public ObjectId Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }
}