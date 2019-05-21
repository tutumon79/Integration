using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SouthernApi.Interfaces;
using SouthernApi.Model;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;

namespace SouthernApi.Validators
{
    public class ComponentValidator : IValidator
    {
        private readonly IJsonHelper jsonHelper;
        private readonly ICacheHelper cacheHelper;
        private readonly IConfiguration configuration;
        private readonly IValidationHelper validationHelper;
        
        public ComponentValidator(ICacheHelper cacheHelper, IConfiguration configuration, IJsonHelper jsonHelper, IValidationHelper validationHelper)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ValidationResponse>> ValidateItems(ItemRequest itemRequest)
        {
            var validationResponses = new List<ValidationResponse>();
            Task<ValidationResponse>[] validationTasks = new Task<ValidationResponse>[3];
            //TODO : Enable this back from office
            //var itemSettings = cacheHelper.GetCache<List<ItemAttributes>>("ItemSettings");
            var itemSettings = jsonHelper.LoadSettings(configuration.GetValue<string>("files:componentSettings"));
            try
            {
                //Get Filtered Results from Settings JSON
                var filteredSettings = GetAttributesFilter(itemSettings);
                //Step1 : Process Required Fields
                var requiredFiledResult = await ValidateRequiredFieldsAsync(itemRequest, filteredSettings.RequiredList);
                if (!ProcessValidationResults(itemRequest, requiredFiledResult, validationResponses))
                    return validationResponses;
                //Step2 : Filter the rows which failed on Required Field Validation check from ItemRequst
                itemRequest = FilterItemRequest(itemRequest, validationResponses);
                //Step3 : Perform Type Check on Int Fields
                var intCheckResult = await ValidateIntegerFieldsAsync(itemRequest, filteredSettings.IntegerList );
                if (!ProcessValidationResults(itemRequest, intCheckResult, validationResponses))
                    return validationResponses;
                //Step5: Perform Field Length Check
                var fieldLengthResults = await ValidateFieldLengthAsync(itemRequest, filteredSettings.NonLOVList);
                ProcessValidationResults(itemRequest, fieldLengthResults, validationResponses);
                return validationResponses;
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
        public ItemAttributesFilter GetAttributesFilter(List<ItemAttributes> componentSettings)
        {
            var filter = new ItemAttributesFilter();
            try
            {
                filter.RequiredList = componentSettings.FindAll(x => x.IsRequired == true);
                filter.NonLOVList = componentSettings.FindAll(x => x.IsLOV == false);
                filter.IntegerList = componentSettings.FindAll(x => x.FieldType == "int");
                return filter;
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
            Debug.WriteLine("Start Required Components: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
            var validationResponse = new ValidationResponse { SupplierId = itemRequest.Supplier };
            var errors = new List<ErrorResponse>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var component in itemRequest.Components)
                    {
                        foreach (var field in requiredFields)
                        {
                            if (!ValidateRequiredField(component, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = component.ItemNumber, ErrorMessage = "Required Field Missing : " + field.FieldName, IsValid = false });
                                break;
                            }
                        }
                    }
                });
                Debug.WriteLine("End Required Components: " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
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
                    foreach (var component in itemRequest.Components)
                    {
                        foreach (var field in decimalFields)
                        {
                            if (!ValidateIntegerField(component, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = component.ItemNumber, ErrorMessage = "Integer Field Validation Failed : " + field.FieldName, IsValid = false });
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
                    foreach (var component in itemRequest.Components)
                    {
                        foreach (var field in nonLOVFields)
                        {
                            if (!ValidateFieldLength(component, field))
                            {
                                errors.Add(new ErrorResponse { ItemNumber = component.ItemNumber, ErrorMessage = "Field Length Check Failed : " + field.FieldName, IsValid = false });
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


        /// <summary>
        /// Check whether the required field has any value
        /// </summary>
        /// <param name="component"></param>
        /// <param name="itemAttributes"></param>
        /// <returns></returns>
        public bool ValidateRequiredField(KitComponent component, ItemAttributes itemAttributes)
        {
            try
            {
                var type = component.GetType();
                //Find the property from item based on the attribute fieldName an get the value
                var field = component.GetType().GetProperty(itemAttributes.FieldName).GetValue(component, null);
                //check whether property has any value 
                return (string.IsNullOrEmpty(Convert.ToString(field))) ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Filter the Components which failed Required field check
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
                        var component = itemRequest.Components.FindAll(x => response.Errors.Any(y => y.ItemNumber != x.ItemNumber));
                        itemRequest.Components = component;
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
                if (validationResult.Errors.Count > 0 && itemRequest.Components.Count == validationResult.Errors.Count)
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
        /// <param name="component"></param>
        /// <param name="itemAttributes"></param>
        /// <returns></returns>
        public bool ValidateIntegerField(KitComponent component, ItemAttributes itemAttributes)
        {
            try
            {
                int result = 0;
                var type = component.GetType();
                //Find the property from item based on the attribute fieldName an get the value
                var field = component.GetType().GetProperty(itemAttributes.FieldName).GetValue(component, null);
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
        /// <param name="component"></param>
        /// <param name="itemAttributes"></param>
        /// <returns></returns>
        public bool ValidateFieldLength(KitComponent component, ItemAttributes itemAttributes)
        {
            try
            {
                var type = component.GetType();
                //Find the property from item based on the attribute fieldName an get the value
                var field = component.GetType().GetProperty(itemAttributes.FieldName).GetValue(component, null);
                var fieldValue = Convert.ToString(field);
                //Non required field, if not supplied dont have to be validated
                if (string.IsNullOrEmpty(fieldValue)) return true;
                return fieldValue.Length <= itemAttributes.Length ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
