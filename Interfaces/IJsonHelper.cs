using SouthernApi.Model;
using System.Collections.Generic;
using System.Net;

namespace SouthernApi.Interfaces
{
    public interface IJsonHelper
    {
        List<ItemAttributes> LoadSettings(string fileName);
        void WriteToRequestStream(dynamic data, HttpWebRequest request);
    }
}
