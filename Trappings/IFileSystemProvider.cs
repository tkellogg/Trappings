using System.Collections.Generic;
using System.Linq;

namespace Trappings
{
    internal interface IFileSystemProvider
    {
        IList<string> GetFiles();
        string ReadFile(string name);
    }
}