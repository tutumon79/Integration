﻿using System.Collections.Generic;

namespace SouthernApi.Model
{
    public class Item
    {
        public string ItemNumber { get; set; }
        public string SupplierProductNumber { get; set; }
        public string SupplierItemDescription { get; set; }
        public string LabelBrand { get; set; }
        public string RequestedLabelBrand { get; set; }
        public string Producer { get; set; }
        public string FancifulName { get; set; }
        public string Category { get; set; }
        public string Class { get; set; }
        public string SubClass { get; set; }
        public string ABV { get; set; }
        public string Proof { get; set; }
        public string JuicePercentage { get; set; }
        public string Vintage { get; set; }
        public string Flavor { get; set; }
        public string Color { get; set; }
        public string Material { get; set; }
        public string SubMaterial { get; set; }
        public string Varietal { get; set; }
        public string AgeStatement { get; set; }
        public string AgeStatementUOM { get; set; }
        public string MarketingDescriptions { get; set; }
        public string CertificationAgency { get; set; }
        public string ClientRequest { get; set; }
        public string SpecialContainer { get; set; }
        public string OuterPackaging { get; set; }
        public string SpecialLabel { get; set; }
        public string HolidaySeasonal { get; set; }
        public string TTBCOLAID { get; set; }
        public string BaseUnitType { get; set; }
        public string BaseUnitVolume { get; set; }
        public string VolumeUnitOfMeasure { get; set; }
        public string BaseUnitClosure { get; set; }
        public string CorkType { get; set; }
        public string BaseUnitWidth { get; set; }
        public string UnitWidthUOM { get; set; }
        public string BaseUnitHeight { get; set; }
        public string UnitHeightUOM { get; set; }
        public string BaseUnitLength { get; set; }
        public string LengthDepthUOM { get; set; }
        public string BaseUnitShippingWeight { get; set; }
        public string UnitWeightUOM { get; set; }
        public string UnitsPerCase { get; set; }
        public string BoxMaterial { get; set; }
        public string InnerPacksPerCase { get; set; }
        public string SpecialPackType { get; set; }
        public string GiftComponent { get; set; }
        public string CaseWidth { get; set; }
        public string CaseWidthUOM { get; set; }
        public string CaseHeight { get; set; }
        public string CaseHeightUOM { get; set; }
        public string CaseLength { get; set; }
        public string CaseLengthUOM { get; set; }
        public string CaseShippingWeight { get; set; }
        public string CaseWeightUOM { get; set; }
        public string PalletWidth { get; set; }
        public string PalletWidthUOM { get; set; }
        public string PalletHeight { get; set; }
        public string PalletHeightUOM { get; set; }
        public string PalletLength { get; set; }
        public string PalletLengthUOM { get; set; }
        public string PalletShippingWeight { get; set; }
        public string PalletWeightUOM { get; set; }
        public string CasesPerLayer { get; set; }
        public string LayersPerPallet { get; set; }
        public string CasesPerPallet { get; set; }
        public string BaseUnitUPC { get; set; }
        public string BaseUnitUPCEffectiveDate { get; set; }
        public string BaseUnitUPCOverride { get; set; }
        public string BaseUnitEAN { get; set; }
        public string BaseUnitEANEffectiveDate { get; set; }
        public string InnerPackUPC { get; set; }
        public string InnerPackUPCEffectiveDate { get; set; }
        public string InnerPackUPCOverride { get; set; }
        public string InnerPackEAN { get; set; }
        public string InnerPackEANEffectiveDate { get; set; }
        public string SCC { get; set; }
        public string SCCEffectiveDate { get; set; }
        public string SCCOverride { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string Appellation { get; set; }
        public string Vineyard { get; set; }
        public string GeographicalDesignation { get; set; }
        public string PackagingCountry { get; set; }
        public string ShelfLife { get; set; }
        public string PremiseType { get; set; }
        public string SupplierItemStatus { get; set; }
        public string SupplierItemDiscontinuedDate { get; set; }
        public string ReplacementId { get; set; }
        public string ReplacementIdType { get; set; }
        public string ReplacementItemEffectiveDate { get; set; }
        public string ReasonCode { get; set; }
        public string CommentSubject { get; set; }
        public string CommentDescription { get; set; }
        public string CommentDate { get; set; }
        public string PrimaryTaxonomyID { get; set; }
        public string GeographyStructureID { get; set; }
        public List<object> ExtendedProperty { get; set; }
    }
}
