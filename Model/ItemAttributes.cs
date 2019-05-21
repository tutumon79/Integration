
namespace SouthernApi.Model
{
    public class ItemAttributes
    {
        public string FieldName { get; set; }
        public string HeaderText { get; set; }
        public bool IsExtendedProperty { get; set; }
        public string FieldType { get; set; }
        public bool IsRequired { get; set; }
        public int Length { get; set; }
        public int? DecimalLength { get; set; }
        public int? NonDecimalLength { get; set; }
        public string RegularExpression { get; set; }
        public bool IsValid { get; set; }
        public bool IsLOV { get; set; }
        public string LOVCacheKey { get; set; }
        public bool IsTaxonomy { get; set; }
        public bool IsGeography { get; set; }
    }
}
