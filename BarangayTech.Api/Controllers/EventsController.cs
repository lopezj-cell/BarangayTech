using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BarangayTech.Api.Services;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly MongoService _mongo;
        public EventsController(MongoService mongo) => _mongo = mongo;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var col = _mongo.Database.GetCollection<MongoDB.Bson.BsonDocument>("events");
            var list = await col.Find(FilterDefinition<MongoDB.Bson.BsonDocument>.Empty)
                                 .Sort(Builders<MongoDB.Bson.BsonDocument>.Sort.Ascending("Date"))
                                 .ToListAsync();
            return Ok(list);
        }
    }
}
