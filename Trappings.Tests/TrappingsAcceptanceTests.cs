using System.Linq;
using MongoDB.Driver.Linq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class TrappingsAcceptanceTests
    {
        [Fact]
        public void A_context_is_a_Trappings_object_in_a_using_statement()
        {
            using (FixtureSession.Create())
            {
                // The context
            }
        }

        [Fact]
        public void Objects_are_accessible_within_the_context()
        {
            using (FixtureSession.Create(conf => conf.Add(typeof(AcceptanceFixtures))))
            {
                var collection = TestUtils.GetCollection<Car>("cars");
                var cruze = (from car in collection.AsQueryable()
                             where car.Make == "Chevy" && car.Model == "Cruze"
                             select car).FirstOrDefault();
                cruze.ShouldNotBeNull();
            }
        }

        [Fact]
        public void Dispose_removes_inserted_objects()
        {
            var collection = TestUtils.GetCollection<Car>("cars");
            var originalNumberOfCars = collection.FindAll().Count();
            using (FixtureSession.Create(conf => conf.Add(typeof(AcceptanceFixtures))))
            {
                collection.FindAll().Count().ShouldEqual(originalNumberOfCars + 1);
            }
            collection.FindAll().Count().ShouldEqual(originalNumberOfCars);
        }

        [Fact]
        public void Objects_added_explicitly_will_get_cleaned_up_at_the_end_with_the_rest()
        {
            var collection = TestUtils.GetCollection<Car>("cars");
            var originalNumberOfCars = collection.FindAll().Count();
            using (var session = FixtureSession.Create())
            {
                var fusion = new Car {Make = "Ford", Model = "Fusion"};
                collection.Save(fusion);
                session.Track(fusion, "cars");
                collection.FindAll().Count().ShouldEqual(originalNumberOfCars + 1);
            }
            collection.FindAll().Count().ShouldEqual(originalNumberOfCars);
        }

        [Fact]
        public void Trappings_can_have_connectionString_configured()
        {
            try
            {
                const string connectionString = "mongodb://example.com/prod";
                FixtureSession.ConnectionString = connectionString;
                FixtureSession.ConnectionString.ShouldEqual(connectionString);
            }
            finally
            {
                FixtureSession.ConnectionString = "mongodb://localhost/test";
            }
        }
    }
}
