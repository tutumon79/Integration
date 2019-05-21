using SouthernApi.Model;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface IValidationHelper
    {
        Task<ValidationResponse> ValidateRequiredFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> requiredFields);
        Task<ValidationResponse> ValidateIntegerFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> nonLOVFields);
        Task<ValidationResponse> ValidateFieldLengthAsync(ItemRequest itemRequest, List<ItemAttributes> nonLOVFields);
        ItemRequest FilterItemRequest(ItemRequest itemRequest, List<ValidationResponse> validationResponse);
        bool ProcessValidationResults(ItemRequest itemRequest, ValidationResponse validationResult, List<ValidationResponse> validationList);
        void UpdateErrorList(ItemRequest itemRequest, ValidationResponse validationResult, List<ValidationResponse> validationList);
        bool ValidateRequiredField(Item item, ItemAttributes itemAttributes);
        bool ValidateIntegerField(Item item, ItemAttributes itemAttributes);
        bool ValidateFieldLength(Item item, ItemAttributes itemAttributes);
        Task<bool> ValidateLOVField(Item item, ItemAttributes itemAttributes);
        Task<TaxonomyValidationResponse> ValidateTaxonomyField(Item item, string itemAttributes, string parentId);
        Task<LOVItem> GetMatchingLOVItemFromCache(string key, string value);
        ExtendedProp GetExtendedProperty(Item item, ItemAttributes itemAttributes);
    }   
}
