using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jwell.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jwell.ConfigurationManager.WebTest
{
    public class Startup
    {
        public Startup(IJwellConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IJwellConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var timeout = Configuration.GetAppSettingConfig("timeout");
            var timeout1 = Configuration.GetCustomSettingConfig("timeout");
            var timeout2 = Configuration.GetConfig("timeout");
            var timeout3 = Configuration.GetConfig("timeout", "app");

            var timeout4=  JwellConfiguration.GetAppSettingConfig("timeout");
            var timeout5 = JwellConfiguration.GetCustomSettingConfig("timeout");
            var timeout6 = JwellConfiguration.GetConfig("timeout");
            var timeout7 =  JwellConfiguration.GetConfig("timeout", "app");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
