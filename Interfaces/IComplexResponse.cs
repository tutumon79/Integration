namespace SouthernApi.Interfaces
{
    public interface IComplexResponse
    {
        IComplexBase CreateObject(string type);
    }
}
