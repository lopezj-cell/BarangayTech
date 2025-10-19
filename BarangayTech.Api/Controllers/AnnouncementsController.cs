using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BarangayTech.Api.Services;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly MongoService _mongo;
        public AnnouncementsController(MongoService mongo) => _mongo = mongo;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var col = _mongo.Database.GetCollection<MongoDB.Bson.BsonDocument>("announcements");
            var list = await col.Find(FilterDefinition<MongoDB.Bson.BsonDocument>.Empty)
                                 .Sort(Builders<MongoDB.Bson.BsonDocument>.Sort.Descending("DatePosted"))
                                 .ToListAsync();
            return Ok(list);
        }
    }
}
