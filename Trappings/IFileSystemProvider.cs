using System.Collections.Generic;

namespace Trappings
{
    internal interface IFileSystemProvider
    {
        IList<string> GetFiles();
    }
}