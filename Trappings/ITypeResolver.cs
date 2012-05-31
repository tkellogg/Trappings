using System;
using System.Collections.Generic;

namespace Trappings
{
    public interface ITypeResolver
    {
        IEnumerable<Type> GetTypes();
    }
}