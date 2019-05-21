using System;
namespace SouthernApi.Model
{
    [Serializable]
    public class LOVItem
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Item { get; set; }
    }
}
