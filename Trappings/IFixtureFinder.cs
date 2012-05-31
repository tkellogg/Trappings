using System;
using System.Collections.Generic;

namespace Trappings
{
    public interface IFixtureFinder
    {
        IEnumerable<Type> GetTypes();
        IFixtureFinder Add(params Type[] types);
    }
}