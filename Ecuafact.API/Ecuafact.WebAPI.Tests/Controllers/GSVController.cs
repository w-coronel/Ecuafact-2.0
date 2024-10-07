using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace Ecuafact.WebAPI.Tests.Controllers
{
    [ApiController]
    [Route("api/external-order-emission")]
    public class GSVController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
         
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<GSVController> _logger;

        public GSVController(ILogger<GSVController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ElectronicSignServiceResult> Post([FromBody] ElectronicSignServiceRequest request)
        {
            var filename = Path.Combine(_env.ContentRootPath, $"log_{DateTime.Now.ToFileTime()}") + ".txt";

            using (FileStream fs = System.IO.File.Create(filename))
            {
                await JsonSerializer.SerializeAsync(fs, request);

                fs.Close();
            } 

            return new ElectronicSignServiceResult { result = true, message = "Firmante recibido correctamente" };
        }


        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
