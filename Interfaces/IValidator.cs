using SouthernApi.Model.Request;
using SouthernApi.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface IValidator
    {
        Task<List<ValidationResponse>> ValidateItemsAsync(ItemRequest itemRequest);
    }
}
