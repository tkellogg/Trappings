namespace Trappings
{
    /// <summary>
    /// Provides a place to initialize the AppDomain - setup MongoDB class mappings or whever you need. Make
    /// sure you provide a default constructor.
    /// </summary>
    public interface IGlobalSetup
    {
        /// <summary>
        /// Run once, before any other FixtureSession code. This is a great place for Mongo class mappings
        /// to be setup.
        /// </summary>
        void SetupOnce();
    }
}