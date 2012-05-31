namespace Trappings
{
    internal interface IDatabaseProvider
    {
        void LoadFixtures(FixtureContainer container);
    }
}