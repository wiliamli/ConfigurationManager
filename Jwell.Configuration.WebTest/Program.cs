using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Jwell.ConfigurationManager.WebTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            //var timeout = JwellConfiguration.GetAppSettingConfig("timeout");
            //var timeout1 = JwellConfiguration.GetCustomSettingConfig("timeout");
            //var timeout2 = JwellConfiguration.GetConfig("timeout");
            //var timeout3 = JwellConfiguration.GetConfig("timeout", "app");

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseJwellConfigCenter();
    }
}
