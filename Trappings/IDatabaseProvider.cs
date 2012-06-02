namespace Trappings
{
    internal interface IDatabaseProvider
    {
        void LoadFixtures(FixtureContainer container);
        void Clear();
        void AddItemForCleanup(string collectionName, object item);
    }
}