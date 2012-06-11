namespace Trappings
{
    internal class Configuration : IConfiguration
    {
        public string Directory
        {
            get { return @"fixtures"; }
        }

        string IConfiguration.ConnectionString
        {
            get { return ConnectionString; }
        }

        public static string ConnectionString { get; set; }
    }
}