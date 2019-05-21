using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SouthernApi.Interfaces;
using SouthernApi.Model;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;

namespace SouthernApi.Helpers
{
    public class ValidationHelper : IValidationHelper
    {
        private readonly ICacheHelper cacheHelper;
        private readonly IDistributedCache cache;
        public ValidationHelper() { }

        public ValidationHelper(ICacheHelper cacheHelper, IDistributedCache cache)
        {
            this.cache = cache;
            this.cacheHelper = cacheHelper;
        }
        /// <summary>
        /// Filter the Items which failed Required field check
        /// </summary>
        /// <param name="itemRequest"></param>
        /// <param name="validationResponse"></param>
        /// <returns></returns>
        public ItemRequest FilterItemRequest(ItemRequest itemRequest, List<ValidationResponse> validationResponse)
        {
            try
            {
                foreach (var response in validationResponse)
                {
                    if (response.Errors.Count > 0)
                    {
                        var items = itemRequest.Items.FindAll(x => response.Errors.Any(y => y.ItemNumber != x.ItemNumber));
                        itemRequest.Items = items;
                    }
                }
                return itemRequest;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Validate the filed length for the non-LOV fields
        /// </summary>
        /// <param name="itemRequest"></param>
        /// <param name="nonLOVFields"></param>
        /// <returns></returns>
        public async Task<ValidationResponse> ValidateFieldLengthAsync(ItemRequest itemRequest, List<ItemAttributes> nonLOVFields)
        {
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier };
            Debug.WriteLine("Start Field Length Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var errors = new List<ErrorResponse>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        foreach (var field in nonLOVFields)
                        {
                            if (!ValidateFieldLength(item, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "Field Length Check Failed : " + field.FieldName, IsValid = false });
                                break;
                            }
                        }
                    }
                });
                Debug.WriteLine("End Field Length Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ValidationResponse> ValidateIntegerFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> decimalFields)
        {
            Debug.WriteLine("Start Integer Validation: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var validationResponse = new ValidationResponse();
            var errors = new List<ErrorResponse>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        foreach (var field in decimalFields)
                        {
                            if (!ValidateIntegerField(item, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "Integer Field Validation Failed : " + field.FieldName, IsValid = false });
                                break;
                            }
                        }
                    }
                });
                Debug.WriteLine("End Integer Validation: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// Validate Required Fields in each item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="requiredFields"></param>
        /// <returns></returns>
        public async Task<ValidationResponse> ValidateRequiredFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> requiredFields)
        {
            Debug.WriteLine("Start: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier };
            var errors = new List<ErrorResponse>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        foreach (var field in requiredFields)
                        {
                            if (!ValidateRequiredField(item, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "Required Field Missing : " + field.FieldName, IsValid = false });
                                break;
                            }
                        }
                    }
                });
                Debug.WriteLine("End: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// If the provided result contains errors, and total errors validation failed.
        /// </summary>
        /// <param name="itemRequest"></param>
        /// <param name="validationResult"></param>
        /// <param name="ValidationList"></param>
        /// <returns></returns>
        public bool ProcessValidationResults(ItemRequest itemRequest, ValidationResponse validationResult, List<ValidationResponse> validationList)
        {
            try
            {
                UpdateErrorList(itemRequest, validationResult, validationList);
                if (validationResult.Errors.Count > 0 && itemRequest.Items.Count == validationResult.Errors.Count)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update the errorList collection with errors from the validation functions
        /// </summary>
        /// <param name="itemRequest"></param>
        /// <param name="validationResult"></param>
        /// <param name="validationList"></param>
        public void UpdateErrorList(ItemRequest itemRequest, ValidationResponse validationResult, List<ValidationResponse> validationList)
        {
            try
            {
                if (validationList.Count == 0)
                    validationList.Add(new ValidationResponse { SupplierId = itemRequest.Supplier, Errors = validationResult.Errors });
                else
                {
                    foreach (var response in validationList)
                    {
                        if (validationResult.Errors.FirstOrDefault(x => response.Errors.Any(y => y.ItemNumber == x.ItemNumber)) == null)
                            response.Errors.AddRange(validationResult.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check whether the field is integer or not
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemAttributes"></param>
        /// <returns></returns>
        public bool ValidateIntegerField(Item item, ItemAttributes itemAttributes)
        {
            try
            {
                int result = 0;
                var type = item.GetType();
                //Find the property from item based on the attribute fieldName an get the value
                var field = item.GetType().GetProperty(itemAttributes.FieldName).GetValue(item, null);
                var fieldValue = Convert.ToString(field);
                //Non required field, if not supplied dont have to be validated
                if (string.IsNullOrEmpty(fieldValue)) return true;
                //check whether field is integer or not
                return int.TryParse(fieldValue, out result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check whether the field length is valid or not
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemAttributes"></param>
        /// <returns></returns>
        public bool ValidateFieldLength(Item item, ItemAttributes itemAttributes)
        {
            try
            {
                //var type = item.GetType();
                if (itemAttributes.IsExtendedProperty)
                {
                    var extendedProperty = GetExtendedProperty(item, itemAttributes);
                    //if the property dont have value, this validation is a success, as it is not a required field.
                    if (string.IsNullOrEmpty(extendedProperty.Value))
                        return true;
                    return extendedProperty.Value.Length <= itemAttributes.Length ? true : false;
                }
                else
                {
                    //Find the property from item based on the attribute fieldName an get the value
                    var field = item.GetType().GetProperty(itemAttributes.FieldName).GetValue(item, null);
                    var fieldValue = Convert.ToString(field);
                    //Non required field, if not supplied dont have to be validated
                    if (string.IsNullOrEmpty(fieldValue)) return true;
                    return fieldValue.Length <= itemAttributes.Length ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExtendedProp GetExtendedProperty(Item item, ItemAttributes itemAttributes)
        {
            try
            {
                var property = new ExtendedProp();
                var propertyList = (List<Object>)item.ExtendedProperty;
                foreach (var extProperty in propertyList)
                {
                    var prop = (Newtonsoft.Json.Linq.JObject)extProperty;
                    if (prop.First == null) return property;
                    var propertyName = ((Newtonsoft.Json.Linq.JProperty)prop.First).Name;
                    if (propertyName == itemAttributes.FieldName)
                    {
                        var propertyValue = Convert.ToString(prop.GetValue(propertyName));
                        property.Name = propertyName;
                        property.Value = propertyValue;
                    }
                }
                return property;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check whether the required field has any value
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemAttributes"></param>
        /// <returns></returns>
        public bool ValidateRequiredField(Item item, ItemAttributes itemAttributes)
        {
            try
            {
                var type = item.GetType();
                //Find the property from item based on the attribute fieldName 
                var property = type.GetProperty(itemAttributes.FieldName);
                //If the property is null, it is not send in the input
                if (itemAttributes.IsExtendedProperty)
                {
                    var extendedProperty = GetExtendedProperty(item, itemAttributes);
                    //if the property dont have value, this validation failed.
                    return string.IsNullOrEmpty(extendedProperty.Value) ? false : true;
                }
                else
                {
                    if (property == null) return false;
                    var field = property.GetValue(item, null);
                    //check whether property has any value 
                    return (string.IsNullOrEmpty(Convert.ToString(field))) ? false : true;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Validate the user supplied data is matching with corresponding LOV field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemAttributes"></param>
        /// <returns></returns>
        public async Task<bool> ValidateLOVField(Item item, ItemAttributes itemAttributes)
        {
            try
            {
                var type = item.GetType();
                //Find the property from item based on the attribute fieldName and get the value
                var field = item.GetType().GetProperty(itemAttributes.FieldName).GetValue(item, null);
                //check whether property has any value 
                var fieldValue = Convert.ToString(field);
                if (string.IsNullOrEmpty(fieldValue))
                    return true;
                return await CheckDataInCache(itemAttributes.FieldName, fieldValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TaxonomyValidationResponse> ValidateTaxonomyField(Item item, string fieldName, string parentId)
        {
            try
            {
                var type = item.GetType();
                //Find the property from item based on the attribute fieldName and get the value
                var field = item.GetType().GetProperty(fieldName).GetValue(item, null);
                //check whether property has any value 
                var fieldValue = Convert.ToString(field);
                //if the item is Category 
                if (fieldName == "Category")
                {
                    var lovItem = await GetMatchingLOVItemFromCache(fieldName, fieldValue);
                    return new TaxonomyValidationResponse { IsValid = true, ParentId = lovItem.Id };
                }
                else
                {
                    if(string.IsNullOrEmpty(fieldValue ))
                        return new TaxonomyValidationResponse { IsValid = true, ParentId = string.Empty };
                    var lovItem = await GetMatchingLOVItemFromCache(fieldName, fieldValue);
                    if(lovItem == null)
                        return new TaxonomyValidationResponse { IsValid = false, ParentId = parentId };
                    else if(lovItem.ParentId == parentId)
                    {
                        return new TaxonomyValidationResponse { IsValid = true, ParentId = lovItem.Id };
                    }
                    return new TaxonomyValidationResponse { IsValid = false, ParentId = string.Empty };
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Data from Cache and check whether the value is matching for the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> CheckDataInCache(string key, string value)
        {
            try
            {
                switch (key)
                {
                    case "LabelBrand":
                    case "CountryOfOrigin":
                    case "Producer":
                    case "Category":
                    case "Class":
                    case "SubClass":
                    case "Region":
                    case "SubRegion":
                    case "Appelation":
                    case "Vineyard":
                        return await CheckBasicItemsInCache(key, value);
                    case "Varietal":
                        //TODO : Waiting for Shailaja's inputs
                        return true;
                    case "default":
                        return true;
                }
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> CheckBasicItemsInCache(string key, string value)
        {
            try
            {
                var result = await cacheHelper.GetCache<List<LOVItem>>(key);
                var match = result.Find(x => x.Item == value);
                return match == null ? false : true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LOVItem> GetMatchingLOVItemFromCache(string key, string value)
        {
            var result = await cacheHelper.GetCache<List<LOVItem>>(key);
            return result.Find(x => x.Item == value);
        }
    }
}
