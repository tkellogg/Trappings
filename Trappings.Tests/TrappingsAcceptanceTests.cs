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
            using (Trappings.Create())
            {
                // The context
            }
        }

        [Fact]
        public void Objects_are_accessible_within_the_context()
        {
            using (Trappings.Create())
            {
                var collection = TestUtils.GetCollection<Car>("cars");
                var cruze = (from car in collection.AsQueryable()
                             where car.Make == "Chevy" && car.Model == "Cruze"
                             select car).FirstOrDefault();
                cruze.ShouldNotBeNull();
            }
        }
    }
}
