namespace SouthernApi.Interfaces
{
    public interface IObjectFactory
    {
        ISGWSBase GetObject(string type);
    }
}
