Trappings is a convenient way to setup integration tests with MongoDB. You
define reusable sets of data (fixtures) that will also be cleaned out at
the end of a test.

Define Fixtures
---------------

Fixtures are nothing more than static fields on a class:

```csharp
class TheRaceTrack
{
  public static Dictionary<string, Car> cars
  {
    { "Chevy Cruze", new Car { Make = "Chevy", Model = "Cruze" } },
    { "Audi A4", new Car { Make = "Audi", Model = "A4" } },
  };

  public static Dictionary<string, Driver> drivers
  {
    { "Jack", new Driver { Name = "Jack Handy" } }
  }
}
```

Each field name is the name of the collection that the objects get inserted
into. The key of the dictionary is a name you can use later to refer to 
objects. In the above example, `TheRaceTrack.cars["Chevy Cruze"].Id` will be
populated during testing to be the correct BsonObjectId.

Using fixtures from a test
--------------------------

```csharp
[Test]
public void ILoveCars()
{
  using(Trappings.Create(config => config.Add(typeof(TheRaceTrack))))
  {
    // Database is now setup. You can use code that assumes that documents
    // exist in db.cars and db.drivers
  }
  // objects from TheRaceTrack are no longer accessible in Mongo
}
```

You can use this for inter-process tests -- like functional or regression
tests. 
