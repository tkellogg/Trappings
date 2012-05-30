using System.Collections.Generic;
using System.IO;

namespace Trappings
{
    public class FileSystemProvider : IFileSystemProvider
    {
        private readonly string directory;

        public FileSystemProvider(IConfiguration configuration)
        {
            directory = configuration.Directory;
        }

        public IList<string> GetFiles()
        {
            return Directory.GetFiles(directory);
        }

        public string ReadFile(string name)
        {
            return File.ReadAllText(name);
        }
    }
}