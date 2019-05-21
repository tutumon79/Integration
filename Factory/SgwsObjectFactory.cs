using SouthernApi.Interfaces;
using SouthernApi.Model;
using SouthernApi.Validators;

namespace SouthernApi.Factory
{
    public class SgwsObjectFactory : IObjectFactory
    {
        public ISGWSBase GetObject(string type)
        {
            switch (type)
            {
                case "ItemMaster":
                    return new ItemMaster();
                case "DigitalAsset":
                    return new DigitalAsset();
                case "KitComponentMaster":
                    return new KitComponentMaster();
                default:
                    return new ItemMaster();
            }
        }
    }
}
