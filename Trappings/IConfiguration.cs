namespace Trappings
{
    internal interface IConfiguration
    {
        string Directory { get; }
        string ConnectionString { get; }
    }
}