using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SouthernApi.Extensions;
using SouthernApi.Factory;
using SouthernApi.Helpers;
using SouthernApi.Interfaces;
using SouthernApi.Service;

namespace SouthernApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDistributedRedisCache(Options =>
            {
                Options.InstanceName = Configuration.GetValue<string>("redis:name");
                Options.Configuration = Configuration.GetValue<string>("redis:host");
            });

            //One HttpClient per thread for optimal performance.
            services.AddSingleton<HttpClient>();
            services.AddTransient<IItemDataService, ItemDataService>();
            services.AddTransient<IItemDataHelper, ItemDataHelper>();
            services.AddTransient<IAssetService, AssetService>();
            services.AddTransient<IObjectFactory, SgwsObjectFactory>();
            services.AddTransient<IJsonHelper, JsonHelper>();
            services.AddTransient<IWebHelper, WebHelper>();
            services.AddTransient<IExcelHelper, ExcelHelper>();
            services.AddTransient<IImageHelper, ImageHelper>();
            services.AddTransient<IMappingHelper, MappingHelper>();
            services.AddTransient<ICacheHelper, CacheHelper>();
            services.AddTransient<IHttpHelper, HttpHelper>();
            services.AddTransient<IValidatorFactory, ValidatorFactory>();
            services.AddTransient<IComplexResponse, ComplexResponseFactory>();
            services.AddTransient<IValidationHelper, ValidationHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseCacheMiddleWare();
        }
    }
}
