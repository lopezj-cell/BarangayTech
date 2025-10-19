using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BarangayTech.Api.Services;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly MongoService _mongo;
        public ServicesController(MongoService mongo) => _mongo = mongo;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var col = _mongo.Database.GetCollection<MongoDB.Bson.BsonDocument>("services");
            var list = await col.Find(FilterDefinition<MongoDB.Bson.BsonDocument>.Empty).ToListAsync();
            return Ok(list);
        }
    }
}
