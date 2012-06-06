using System;
using System.Dynamic;
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

        public class DescribeLocatingTheIdProperty
        {
            [Fact]
            public void It_finds_the_id_named_Id()
            {
                var db = new MongoDatabaseProvider(new Configuration());
                db.GetId(new {Id = 42}).ShouldEqual(42);
            }

            [Fact]
            public void It_finds_the_id_named_id()
            {
                var db = new MongoDatabaseProvider(new Configuration());
                db.GetId(new {id = 42}).ShouldEqual(42);
            }

            [Fact]
            public void It_finds_the_id_named_ID()
            {
                var db = new MongoDatabaseProvider(new Configuration());
                db.GetId(new {ID = 42}).ShouldEqual(42);
            }

            [Fact]
            public void It_finds_the_id_named__id()
            {
                var db = new MongoDatabaseProvider(new Configuration());
                db.GetId(new {_id = 42}).ShouldEqual(42);
            }

            [Fact]
            public void It_throws_when_it_cant_find_a_suitable_ID()
            {
                var db = new MongoDatabaseProvider(new Configuration());
                Assert.Throws<ArgumentException>(() =>
                         db.GetId(new {ObjectId = 42}));
            }

            [Fact]
            public void It_can_find_the_id_when_object_is_an_ExpandoObject()
            {
                var db = new MongoDatabaseProvider(new Configuration());
                dynamic obj = new ExpandoObject();
                obj.id = 42;
                ((object)db.GetId(obj)).ShouldEqual(42);
            }
        }

        public void Dispose()
        {
            TestUtils.GetCollection<Car>("cars").Drop();
        }
    }
}
