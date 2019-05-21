using System.Collections.Generic;
using System.Threading.Tasks;
using SouthernApi.Interfaces;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;

namespace SouthernApi.Validators
{
    public class DigitalAssetValidator : IValidator
    {
        public Task<List<ValidationResponse>> ValidateItemsAsync(ItemRequest itemRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
