using System.Collections.Generic;

namespace SouthernApi.Model.Request
{
    public class ItemRequest : SGWSRequestBase
    {
        public string Supplier { get; set; }
        public List<Item> Items { get; set; }
        public List<KitComponent> Components { get; set; }
    }
}
