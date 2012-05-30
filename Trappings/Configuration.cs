namespace Trappings
{
    public class Configuration : IConfiguration
    {
        public string Directory
        {
            get { return @"fixtures"; }
        }

        public string ConnectionString
        {
            get { return "mongodb://localhost/test"; }
        }
    }
}