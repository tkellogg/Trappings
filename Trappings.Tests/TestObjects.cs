using System.Collections.Generic;
using MongoDB.Bson;

namespace Trappings.Tests
{
    public class Car
    {
        public ObjectId Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }

    public class Driver
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public ObjectId CarId { get; set; }
    }

    class AcceptanceFixtures
    {
        static Dictionary<string, Car> cars = new Dictionary<string, Car>
          {
              {"cruze", new Car { Make = "Chevy", Model = "Cruze"}}
          };
    }
}