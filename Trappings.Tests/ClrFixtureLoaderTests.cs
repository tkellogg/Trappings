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

        class ImplementingTestFixtureData : ITestFixtureData
        {
            // not picked up
            public static Car[] cars = new[] {new Car {Make = "Volkswagon", Model = "Jetta"}};
            public static int InstanceCount;

            public ImplementingTestFixtureData() { InstanceCount++;}

            public IEnumerable<SetupObject> Setup()
            {
                var car = new Car {Make = "Chevy", Model = "Cruze"};
                yield return new SetupObject("cars", car);;
                yield return new SetupObject("drivers", new Driver{Name = "Tim Kellogg", CarId = car.Id});
            }
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
        
        public class DescribeUsingTestFixtureData
        {
            private readonly ClrFixtureLoader loader;
            private readonly int oldInstanceCount;
            private readonly IFixtureFinder resolver;

            public DescribeUsingTestFixtureData()
            {
                oldInstanceCount = ImplementingTestFixtureData.InstanceCount;
                resolver = Mock.Of<IFixtureFinder>(x => 
                        x.GetTypes() == new[] {typeof (ImplementingTestFixtureData)});

                loader = new ClrFixtureLoader(resolver);
            }

            [Fact]
            public void The_type_is_instantiated_and_used()
            {
                foreach(var fixture in loader.GetFixtures()) ;
                ImplementingTestFixtureData.InstanceCount.ShouldEqual(oldInstanceCount + 1);
            }
        
            [Fact]
            public void All_fixtures_found_are_returned()
            {
                var fixtures = loader.GetFixtures().ToArray();
                fixtures.Length.ShouldEqual(2);
                fixtures[0].Name.ShouldEqual("cars");
                fixtures[1].Name.ShouldEqual("drivers");
            }

            [Fact]
            public void Static_fields_arent_used()
            {
                var fixtures = loader.GetFixtures().ToArray();
                fixtures.Any(x => x.Name == "cars" &&
                     x.Fixtures.Any(y => y.Value.Model == "Jetta")).ShouldBeFalse();
            }

            [Fact]
            public void It_uses_prebuilt_instances_after_Types()
            {
                var instance = Mock.Of<ITestFixtureData>(x => x.Setup() == 
                    new[]{ Mock.Of<SetupObject>(so => so.CollectionName == "instance") });
                Mock.Get(resolver).Setup(x => x.GetFixtures()).Returns(new[] {instance});

                var fixtures = loader.GetFixtures().ToArray();
                fixtures.Length.ShouldEqual(3);
                fixtures[2].Name.ShouldEqual("instance");
            }
        }
    }
}
