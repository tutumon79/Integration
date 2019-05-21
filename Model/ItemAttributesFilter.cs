using System.Collections.Generic;

namespace SouthernApi.Model
{
    public class ItemAttributesFilter
    {
        public List<ItemAttributes> RequiredList { get; set; }
        public List<ItemAttributes> LOVList { get; set; }
        public List<ItemAttributes> NonLOVList { get; set; }
        public List<ItemAttributes> DecimalList { get; set; }
        public List<ItemAttributes> IntegerList { get; set; }
        public List<ItemAttributes> TaxonomyList { get; set; }
        public List<ItemAttributes> GeographyList { get; set; }
    }
}
