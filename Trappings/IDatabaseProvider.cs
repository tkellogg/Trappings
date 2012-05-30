namespace Trappings
{
    public interface IDatabaseProvider
    {
        void LoadFixtures(FixtureContainer container);
    }
}