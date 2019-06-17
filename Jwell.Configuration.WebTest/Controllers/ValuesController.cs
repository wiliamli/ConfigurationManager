using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jwell.Configuration;
using Jwell.Configuration.WebTest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Jwell.ConfigurationManager.WebTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IJwellConfiguration Configuration { get; set; }
        public ValuesController(IJwellConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var timeout = Configuration.GetAppSettingConfig("timeout");
            var timeout1 = Configuration.GetCustomSettingConfig("timeout");
            var timeout2 = Configuration.GetConfig("timeout");
            var timeout3 = Configuration.GetConfig("timeout", "app");

            var timeout4 = JwellConfiguration.GetAppSettingConfig("timeout");
            var timeout5 = JwellConfiguration.GetCustomSettingConfig("timeout");
            var timeout6 = JwellConfiguration.GetConfig("timeout");
            var timeout7 = JwellConfiguration.GetConfig("timeout", "app");

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
             return Directory.GetCurrentDirectory(); 
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
