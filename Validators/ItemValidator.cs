using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SouthernApi.Interfaces;
using SouthernApi.Model;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;

namespace SouthernApi.Validators
{
    public class ItemValidator : IValidator
    {
        private readonly ICacheHelper cacheHelper;
        private readonly IConfiguration configuration;
        private readonly IJsonHelper jsonHelper;
        private readonly IValidationHelper validationHelper;

        public string TaxonomyId { get; set; }
        public string GeographyId { get; set; }

        public ItemValidator(ICacheHelper cacheHelper, IConfiguration configuration, IJsonHelper jsonHelper, IValidationHelper validationHelper)
        {
            this.cacheHelper = cacheHelper;
            this.configuration = configuration;
            this.jsonHelper = jsonHelper;
            this.validationHelper = validationHelper;
        }

        public async Task<List<ValidationResponse>> ValidateItemsAsync(ItemRequest itemRequest)
        {
            try
            {
                var validationResponse = await ValidateItems(itemRequest);
                return validationResponse;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ValidationResponse>> ValidateItems(ItemRequest itemRequest)
        {
            var validationResponses = new List<ValidationResponse>();
            Task<ValidationResponse>[] validationTasks = new Task<ValidationResponse>[6];
            //TODO : Enable this back from office
            //var itemSettings = cacheHelper.GetCache<List<ItemAttributes>>("ItemSettings");
            var itemSettings = jsonHelper.LoadSettings(configuration.GetValue<string>("files:itemsSettings"));
            try
            {
                //Get Filtered Results from Settings JSON
                var filteredSettings = GetAttributesFilter(itemSettings);
                //Step1 : Process Required Fields
                var requiredFiledResult = await validationHelper.ValidateRequiredFieldsAsync(itemRequest, filteredSettings.RequiredList);
                if (!validationHelper.ProcessValidationResults(itemRequest, requiredFiledResult, validationResponses))
                    return validationResponses;
                //Step2 : Filter the rows which failed on Required Field Validation check from ItemRequst
                itemRequest = validationHelper.FilterItemRequest(itemRequest, validationResponses);
                //Step3 : Perform Type Check on Int Fields
                var intCheckResult = await validationHelper.ValidateIntegerFieldsAsync(itemRequest, filteredSettings.IntegerList);
                if (!validationHelper.ProcessValidationResults(itemRequest, intCheckResult, validationResponses))
                    return validationResponses;
                //Step4 : Perform Type Check on DateTime Fields
                var dateTimeCheckResult = await ValidateDateTimeFieldsAsync(itemRequest, filteredSettings.NonLOVList);
                if (!validationHelper.ProcessValidationResults(itemRequest, dateTimeCheckResult, validationResponses))
                    return validationResponses;
                //Step5: Perform Field Length Check
                var fieldLengthResults = await validationHelper.ValidateFieldLengthAsync(itemRequest, filteredSettings.NonLOVList);
                if (!validationHelper.ProcessValidationResults(itemRequest, fieldLengthResults, validationResponses))
                    return validationResponses;
                //Step6 : Perform Number Format Check - This will cover decimal validation and formatting
                var formatCheckResults = await ValidateNumberFormatAsync(itemRequest, filteredSettings.DecimalList);
                if (!validationHelper.ProcessValidationResults(itemRequest, formatCheckResults, validationResponses))
                    return validationResponses;
                //Step7 : Process LOV Fields
                var lovResults = await ValidateLOVFieldsAsync(itemRequest, filteredSettings.LOVList);
                if (!validationHelper.ProcessValidationResults(itemRequest, lovResults, validationResponses))
                    return validationResponses;
                //Step8 : Perform Taxonomy Validation and generate Taxonomy field
                var taxonomyResult = await ValidateTaxonomyFieldsAsync(itemRequest, filteredSettings.TaxonomyList);
                if (!validationHelper.ProcessValidationResults(itemRequest, taxonomyResult, validationResponses))
                    return validationResponses;
                //Step9 : Perform Geography Validation and generate Geography field
                var geographyResult = await ValidateGeographyFieldsAsync(itemRequest, filteredSettings.GeographyList);
                validationHelper.ProcessValidationResults(itemRequest, taxonomyResult, validationResponses);
                return validationResponses;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ValidationResponse> ValidateDateTimeFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> nonLOVFields)
        {
            Debug.WriteLine("Start DateTime Validation: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var dateFields = nonLOVFields.FindAll(x => x.FieldType == "datetime");
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier};
            var errors = new List<ErrorResponse>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        foreach (var field in dateFields)
                        {
                            if (!ValidateDateTimeField(item, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "DateTime Field Validation Failed : " + field.FieldName, IsValid = false });
                                break;
                            }
                        }
                    }
                });
                Debug.WriteLine("End DateTime Validation: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ValidationResponse> ValidateNumberFormatAsync(ItemRequest itemRequest, List<ItemAttributes> decimalList)
        {
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier };
            Debug.WriteLine("Start Number Format Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var errors = new List<ErrorResponse>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        foreach (var field in decimalList)
                        {
                            if (!ValidateDecimalFieldFormat(item, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "Number Format Check Failied : " + field.FieldName, IsValid = false });
                                break;
                            }
                        }
                    }
                });
                Debug.WriteLine("End Number Format Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ValidationResponse> ValidateLOVFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> lovFields)
        {
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier };
            Debug.WriteLine("Start LOV Match Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var errors = new List<ErrorResponse>();
            try
            {
                await Task.Run( async() =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        foreach (var field in lovFields)
                        {
                            if (!await validationHelper.ValidateLOVField(item, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "LOV Value not matching for the Field : " + field.FieldName, IsValid = false });
                                break;
                            }
                        }
                    }
                });
                Debug.WriteLine("End LOV Match Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ValidationResponse> ValidateTaxonomyFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> taxonomyFields)
        {
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier };
            Debug.WriteLine("Start LOV Match Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var errors = new List<ErrorResponse>();
            var taxonomyList = new List<TaxonomyResponse>();
            try
            {
                await Task.Run( async () =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        var categoryResponse = await validationHelper.ValidateTaxonomyField(item, "Category", "");
                        var classResponse = await validationHelper.ValidateTaxonomyField(item, "Class", categoryResponse.ParentId);
                        if (!classResponse.IsValid)
                        {
                            errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "Invalid LOV Combination for Taxonomy Field : Class", IsValid = false });
                            break;
                        }
                        var subClassResponse = await validationHelper.ValidateTaxonomyField(item, "SubClass", classResponse.ParentId);
                        if (!subClassResponse.IsValid)
                        {
                            errors.Add(new ErrorResponse { ItemNumber = item.ItemNumber, ErrorMessage = "Invalid LOV Combination for Taxonomy Field : SubClass", IsValid = false });
                            break;
                        }
                        taxonomyList.Add(new TaxonomyResponse { ItemNumber = item.ItemNumber, TaxonomyId = subClassResponse.ParentId });
                    }
                });
                Debug.WriteLine("End LOV Match Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                validationResponse.TaxonomyList = taxonomyList;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ValidationResponse> ValidateGeographyFieldsAsync(ItemRequest itemRequest, List<ItemAttributes> requiredFields)
        {
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier };
            Debug.WriteLine("Start LOV Match Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var errors = new List<ErrorResponse>();
            var geographyList = new List<GeographyResponse>();
            try
            {
                await Task.Run( async () =>
                {
                    foreach (var item in itemRequest.Items)
                    {
                        var type = item.GetType();
                        //Find the property from item based on the attribute fieldName and get the value
                        var field = item.GetType().GetProperty("Vineyard").GetValue(item, null);
                        var fieldValue = Convert.ToString(field);
                        var lovItem = await validationHelper.GetMatchingLOVItemFromCache("Vineyard", fieldValue);
                        geographyList.Add(new GeographyResponse { ItemNumber = item.ItemNumber, TaxonomyId = lovItem.Id });
                    }
                });
                Debug.WriteLine("End LOV Match Check: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                validationResponse.Errors = errors;
                validationResponse.GeographyList = geographyList;
                return validationResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Filter the customer input to generate the collections
        /// for running validation rules
        /// </summary>
        /// <param name="itemRequest"></param>
        /// <returns></returns>
        public ItemAttributesFilter GetAttributesFilter(List<ItemAttributes> itemSettings)
        {
            var filter = new ItemAttributesFilter();
            try
            {
                filter.RequiredList = itemSettings.FindAll(x => x.IsRequired == true);
                filter.NonLOVList = itemSettings.FindAll(x => x.IsLOV ==false);
                filter.LOVList = itemSettings.FindAll(x => x.IsLOV == true);
                filter.DecimalList = itemSettings.FindAll(x => x.FieldType == "decimal");
                filter.IntegerList = itemSettings.FindAll(x => x.FieldType == "int");
                filter.TaxonomyList = itemSettings.FindAll(x => x.IsTaxonomy == true);
                filter.GeographyList = itemSettings.FindAll(x => x.IsGeography == true);
                return filter;
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
        private bool ValidateDateTimeField(Item item, ItemAttributes itemAttributes)
        {
            try
            {
                DateTime resultDate = DateTime.MinValue;
                var enUS = new CultureInfo("en-US");
                var type = item.GetType();
                //Find the property from item based on the attribute fieldName an get the value
                var field = item.GetType().GetProperty(itemAttributes.FieldName).GetValue(item, null);
                var fieldValue = Convert.ToString(field);
                //Non required field, if not supplied dont have to be validated
                if (string.IsNullOrEmpty(fieldValue)) return true;
                //check whether field is integer or not
                return DateTime.TryParseExact(fieldValue, "YYYY/MM/DD", enUS, DateTimeStyles.AllowLeadingWhite, out resultDate);
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
        private bool ValidateDecimalFieldFormat(Item item, ItemAttributes itemAttributes)
        {
            try
            {
                var type = item.GetType();
                //Find the property from item based on the attribute fieldName an get the value
                var field = item.GetType().GetProperty(itemAttributes.FieldName).GetValue(item, null);
                var fieldValue = Convert.ToString(field);
                //Non required field, if not supplied dont have to be validated
                if (string.IsNullOrEmpty(fieldValue)) return true;
                return IsValidDecimalFormat(fieldValue, itemAttributes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private bool IsValidDecimalFormat(string input, ItemAttributes attributes)
        {
            try
            {
                if(input.Contains('.'))
                {
                    var result = input.Split('.');
                    if (result[0].Length <= attributes.NonDecimalLength && result[1].Length <= attributes.DecimalLength)
                        return true;
                    return false;
                }
                return input.Length <= attributes.NonDecimalLength ? true : false;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
