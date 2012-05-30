using System.Collections.Generic;
using System.Linq;

namespace Trappings
{
    public interface IFileSystemProvider
    {
        IList<string> GetFiles();
        string ReadFile(string name);
    }
}