using System.Collections.Generic;
using System.IO;

namespace Trappings
{
    internal class FileSystemProvider : IFileSystemProvider
    {
        private readonly string directory;

        public FileSystemProvider(string directory)
        {
            this.directory = directory;
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