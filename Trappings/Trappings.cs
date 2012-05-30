using System;

namespace Trappings
{
    public sealed class Trappings : IDisposable
    {
        private readonly IFixtureLoader fixtureLoader;

        internal Trappings(IFixtureLoader fixtureLoader, IDatabaseProvider db)
        {
            this.fixtureLoader = fixtureLoader;
        }

        public Trappings()
        {
        }

        public void Dispose()
        {
        }
    }
}
