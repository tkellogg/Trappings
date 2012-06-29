using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
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
            using (FixtureSession.Create(new[]{ typeof(AcceptanceFixtures) }))
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
            using (FixtureSession.Create(new[]{ typeof(AcceptanceFixtures) }))
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

        private class CarsAndDrivers : ITestFixtureData
        {
            public static Car Car;

            public IEnumerable<SetupObject> Setup()
            {
                Car = new Car {Make = "Chevy", Model = "Cruze"};
                yield return new SetupObject("cars", Car);
                yield return new SetupObject("drivers", new Driver{CarId = Car.Id, Name = "Tim Kellogg"});
            }
        }

        [Fact]
        public void ITestFixtureData_can_be_used_for_complex_setups()
        {
            using (FixtureSession.Create<CarsAndDrivers>())
            {
                var collection = TestUtils.GetCollection<Car>("cars").AsQueryable();
                var cars = collection.Where(x => x.Make == "Chevy" && x.Model == "Cruze").ToArray();
                cars.Length.ShouldEqual(1);
            }
        }

        [Fact]
        public void ITestFixtureData_can_be_used_to_form_relationships_via_IDs()
        {
            using (FixtureSession.Create<CarsAndDrivers>())
            {
                var driverCollection = TestUtils.GetCollection<Driver>("drivers").AsQueryable();
                var drivers = (driverCollection.Where(x => x.Name == "Tim Kellogg")).ToArray();

                drivers.Length.ShouldEqual(1);
                var driver = drivers[0];
                driver.CarId.ShouldNotEqual(ObjectId.Empty);

                var carCollection = TestUtils.GetCollection<Car>("cars").AsQueryable();
                var cars = (carCollection.Where(x => x.Id == driver.CarId)).ToArray();
                cars.Any().ShouldBeTrue();
            }
        }

        [Fact]
        public void You_can_use_a_delegate_to_add_fixtures()
        {
            var objects = new[] { new SetupObject { CollectionName = "drivers", Value = new Driver{Name="Dale"}}  };
            using (FixtureSession.Create(objects))
            {
                var collection = TestUtils.GetCollection<Driver>("drivers").AsQueryable();
                var driver = collection.FirstOrDefault();
                driver.ShouldNotBeNull();
                driver.Name.ShouldEqual("Dale");
            }
        }

        [Fact]
        public void You_can_use_the_session_to_get_objects_out_of_the_db()
        {
            using (var session = FixtureSession.Create<CarsAndDrivers>())
            {
                var expected = CarsAndDrivers.Car;
                var fromDb = session.GetFromDb<Car>("cars", expected.Id);

                fromDb.Make.ShouldEqual(expected.Make);
                fromDb.Model.ShouldEqual(expected.Model);
                fromDb.Id.ShouldEqual(expected.Id);
            }
        }
    }
}
