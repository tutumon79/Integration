using Microsoft.Extensions.Configuration;
using SouthernApi.Interfaces;
using SouthernApi.Validators;

namespace SouthernApi.Factory
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly ICacheHelper cacheHelper;
        private readonly IConfiguration configuration;
        private readonly IJsonHelper jsonHelper;
        private readonly IValidationHelper validationHelper;
        public ValidatorFactory(ICacheHelper cacheHelper, IConfiguration configuration, IJsonHelper jsonHelper, IValidationHelper validationHelper)
        {
            this.cacheHelper = cacheHelper;
            this.configuration = configuration;
            this.jsonHelper = jsonHelper;
            this.validationHelper = validationHelper;
        }
        public IValidator GetObject(string type)
        {
            switch (type)
            {
                case "ItemValidator":
                    return new ItemValidator(this.cacheHelper, this.configuration, this.jsonHelper, this.validationHelper);
                case "DigitalAssetValidator":
                    return new ItemValidator(this.cacheHelper, this.configuration, this.jsonHelper, this.validationHelper);
                case "ComponentValidator":
                    return new ComponentValidator(this.cacheHelper, this.configuration, this.jsonHelper, this.validationHelper);
                default:
                    return new ItemValidator(this.cacheHelper, this.configuration, this.jsonHelper, this.validationHelper);
            }
        }
    }
}
