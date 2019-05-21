using SouthernApi.Model.Request;
using SouthernApi.Model.Response;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface IItemDataService
    {
        Task<PIMImportResponse> ProcessItems(ItemRequest item);
        JobStatusResponse GetJobStatus(string jobId);
    }
}
