using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class ClrFixtureLoaderTests
    {
        #region Types used for testing
        class WithOneContainer
        {
            static Dictionary<string, Car> cars = new Dictionary<string, Car>
                   {
                       {"cruze", new Car{ Make = "Chevy", Model = "Cruze"} }
                   };
        }

        class WithSeveralContainers
        {
            private static Dictionary<string, Car> cars = new Dictionary<string, Car> {};
            private static Dictionary<string, Car> oldCars = new Dictionary<string, Car> {};
        }

        class WithoutStaticFields
        {
            private Dictionary<string, Car> cars = new Dictionary<string, Car> {};
            private Dictionary<string, Car> oldCars = new Dictionary<string, Car> {};
        }

        class WithEnumerable
        {
            private static Car[] cars = new[] {new Car {Make = "Chevy", Model = "Cruze"}};
        }
        #endregion

        [Fact]
        public void It_gets_fixtures_from_a_type()
        {
            var resolver = Mock.Of<IFixtureFinder>(x => x.GetTypes() == new[] {typeof (WithOneContainer)});
            var loader = new ClrFixtureLoader(resolver);
            var containers = loader.GetFixtures().ToArray();

            containers.Length.ShouldEqual(1);
            containers[0].Fixtures.Count().ShouldEqual(1);
            containers[0].Name.ShouldEqual("cars");
            var cars = containers[0].Fixtures.First();
            cars.Name.ShouldEqual("cruze");
            var car = (Car) cars.Value;
            car.Make.ShouldEqual("Chevy");
            car.Model.ShouldEqual("Cruze");
        }

        [Fact]
        public void It_uses_field_names_as_the_collection_name()
        {
            var resolver = Mock.Of<IFixtureFinder>(x => x.GetTypes() == new[] {typeof (WithSeveralContainers)});
            var loader = new ClrFixtureLoader(resolver);
            var containers = loader.GetFixtures().ToArray();

            containers.Length.ShouldEqual(2);
            containers.Any(x => x.Name == "cars").ShouldBeTrue();
            containers.Any(x => x.Name == "oldCars").ShouldBeTrue();
        }

        [Fact]
        public void It_only_uses_static_fields()
        {
            var resolver = Mock.Of<IFixtureFinder>(x => x.GetTypes() == new[] {typeof (WithoutStaticFields)});
            var loader = new ClrFixtureLoader(resolver);
            loader.GetFixtures().Any().ShouldBeFalse();
        }

        [Fact]
        public void It_will_also_get_IEnumerble_fields()
        {
            var resolver = Mock.Of<IFixtureFinder>(x => x.GetTypes() == new[] {typeof (WithEnumerable)});
            var loader = new ClrFixtureLoader(resolver);
            var containers = loader.GetFixtures().ToArray();

            containers.Length.ShouldEqual(1);
            containers.Any(x => x.Name == "cars").ShouldBeTrue();
            containers[0].Fixtures.First().Name.ShouldBeNull();
            Car car = containers[0].Fixtures.First().Value;
            car.Make.ShouldEqual("Chevy");
            car.Model.ShouldEqual("Cruze");
        }
    }
}
