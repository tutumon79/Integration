namespace SouthernApi.Interfaces
{
    public interface IValidatorFactory
    {
        IValidator GetObject(string type);
    }
}
