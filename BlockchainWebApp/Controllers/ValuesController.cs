using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainWebApp.Models;
using BlockchainWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlockchainWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //var client = new MongoClient("mongodb+srv://cars-user-1:HjOTA330X1qMtSgN@cluster0-xiaju.mongodb.net/cars_db?retryWrites=true&w=majority");
            //var database = client.GetDatabase("cars_db");
            //var collection = database.GetCollection<CarData>("cars");

            //await collection.InsertOneAsync(new CarData
            //{
            //    Date = "2",
            //    License = "2",
            //    Owners = "2",
            //    Registration = "2",
            //    Vin = "2"
            //});

            //var resp = await collection.AsQueryable().ToListAsync();

            //var client = new MongoClient("mongodb+srv://cars-user-1:HjOTA330X1qMtSgN@cluster0-xiaju.mongodb.net/cars_db?retryWrites=true&w=majority");
            //var database = client.GetDatabase("cars_db");
            //var collection = database.GetCollection<CarData>("cars");

            //await collection.InsertOneAsync(new CarData
            //{
            //    Date = "2",
            //    License = "2",
            //    Owners = "2",
            //    Registration = "2",
            //    Vin = "2"
            //});

            return Ok("done");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
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
