using Newtonsoft.Json;
using SouthernApi.Interfaces;
using SouthernApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SouthernApi.Helpers
{
    public class JsonHelper : IJsonHelper
    {
        public List<ItemAttributes> LoadSettings(string file)
        {
            try
            {
                List<ItemAttributes> settings = JsonConvert.DeserializeObject<List<ItemAttributes>>(File.ReadAllText(file));
                return settings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WriteToRequestStream(dynamic data, HttpWebRequest request)
        {
            try
            {
                if (data != null)
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        var serializer = JsonConvert.SerializeObject(data);
                        streamWriter.Write(serializer);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
