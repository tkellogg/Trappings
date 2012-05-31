using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class MongoDatabaseProviderTests : IDisposable
    {
        readonly FixtureContainer container = new FixtureContainer
         {
             Name = "cars",
             Fixtures = new[]
                {
                    new Fixture{ Name = "Cruze", Value = new{ Make = "Chevy", Model = "Cruze", Id = BsonObjectId.GenerateNewId() }},
                    new Fixture{ Name = "Malibu", Value = new{ Make = "Chevy", Model = "Malibu", Id = BsonObjectId.GenerateNewId() }},
                }
         };

        [Fact]
        public void It_loads_fixtures_into_the_collection_named_by_the_container()
        {
            var db = new MongoDatabaseProvider(new Configuration());
            db.LoadFixtures(container);

            var collection = TestUtils.GetCollection<Car>("cars");
            var objects = collection.AsQueryable().ToList();
            objects.Count.ShouldEqual(2);
            objects.Any(x => x.Make == "Chevy" && x.Model == "Cruze").ShouldBeTrue();
        }

        [Fact]
        public void It_clears_everything_that_was_inserted()
        {
            Dispose(); // just in case
            var db = new MongoDatabaseProvider(new Configuration());
            db.LoadFixtures(container);
            db.Clear();

            var collection = TestUtils.GetCollection<Car>("cars");
            var objects = collection.AsQueryable().ToList();
            objects.Count.ShouldEqual(0);
        }

        public void Dispose()
        {
            TestUtils.GetCollection<Car>("cars").Drop();
        }
    }
}
