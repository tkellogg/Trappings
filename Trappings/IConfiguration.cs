namespace Trappings
{
    public interface IConfiguration
    {
        string Directory { get; }
        string ConnectionString { get; }
    }
}