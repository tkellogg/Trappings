using System.Collections.Generic;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Trappings.Tests
{
    public class ClrFixtureLoaderTests
    {
        class WithOneContainer
        {
            static Dictionary<string, Car> cars = new Dictionary<string, Car>
                   {
                       {"cruze", new Car{ Make = "Chevy", Model = "Cruze"} }
                   };
        }

        [Fact]
        public void It_gets_fixtures_from_a_type()
        {
            var resolver = Mock.Of<ITypeResolver>(x => x.GetTypes() == new[] {typeof (WithOneContainer)});
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
    }
}
