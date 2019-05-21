using SouthernApi.Model.Request;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface ISGWSBase
    {
        void CreateExcel(SGWSRequestBase request);
    }
}
