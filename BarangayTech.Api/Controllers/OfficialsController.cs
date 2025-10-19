using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BarangayTech.Api.Services;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfficialsController : ControllerBase
    {
        private readonly MongoService _mongo;
        public OfficialsController(MongoService mongo) => _mongo = mongo;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var col = _mongo.Database.GetCollection<MongoDB.Bson.BsonDocument>("officials");
            var list = await col.Find(FilterDefinition<MongoDB.Bson.BsonDocument>.Empty).ToListAsync();
            return Ok(list);
        }
    }
}
