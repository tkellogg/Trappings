Trappings is a convenient way to setup integration tests with MongoDB. You
define reusable sets of data (fixtures) that will also be cleaned out at
the end of a test.

Define Fixtures
---------------

The cleanest way to setup a fixture is to create a class that implements 
`ITestFixtureData`:

```csharp
class TheRaceTrack : ITestFixtureData
{
  public static Car Cruze;

  public IEnumerable<SetupObject> Setup() 
  {
    // Assign to static field for easy access later
    Cruze = new Car { Make = "Chevy", Model = "Cruze" };

    // cruze will be inserted into the database after this line
    yield return new SetupObject { CollectionName = "cars", Value = Cruze };

    // Since `cruze` has already been inserted, it's ID is already auto-assigned
    var tim = new Driver { Name = "Tim", CarId = Cruze.Id };
    yield return new SetupObject("drivers", tim);
  }
}
```

Each item that is yielded into the Enumerable is immediately inserted into the 
database. This is useful (as above) for setting up relationships between 
collections. It's also sometimes useful to hold a reference to objects created
in a statick property or field - so you can access them during tests for verifying
behavior.

Using fixtures from a test
--------------------------

```csharp
[Test]
public void ILoveCars()
{
  using(FixtureSession.UseFixture<TheRaceTrack>())
  {
    // Database is now setup. You can use code that assumes that documents
    // exist in db.cars and db.drivers

    var driver = from driver in drivers.AsQueryable()
                 where driver.CarId == TheRaceTrack.Cruze.Id
                 select driver;

    driver.Count().ShouldEqual(1);
  }
  // objects from TheRaceTrack are no longer accessible in Mongo
}
```

You can use this for inter-process tests -- like functional or regression
tests that hit a website or web service. 
