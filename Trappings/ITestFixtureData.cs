using System.Collections.Generic;

namespace Trappings
{
    /// <summary>
    /// A class that implements this interface can be instantiated to load fixtures in a 
    /// more complex manner. If a class implements this interface, it is instantated and used,
    /// but no static fields are scanned.
    /// </summary>
    public interface ITestFixtureData
    {
        /// <summary>
        /// Use `yield return new SetupObject()` to setup several objects in the database.
        /// </summary>
        /// <returns></returns>
        IEnumerable<SetupObject> Setup();
    }
}
