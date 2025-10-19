using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministrativeTasksController : ControllerBase
    {
        private readonly IMongoCollection<AdministrativeTask> _col;
        public AdministrativeTasksController(MongoService mongo)
        {
            _col = mongo.Database.GetCollection<AdministrativeTask>("administrative_tasks");
        }

        [HttpGet]
        public async Task<ActionResult<List<AdministrativeTask>>> GetAll()
        {
            var list = await _col.Find(FilterDefinition<AdministrativeTask>.Empty)
                                 .SortByDescending(t => t.CreatedAt)
                                 .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdministrativeTask>> GetById(string id)
        {
            var item = await _col.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<AdministrativeTask>> Create([FromBody] AdministrativeTask input)
        {
            input.Id = null;
            input.CreatedAt = DateTime.UtcNow;
            input.UpdatedAt = DateTime.UtcNow;
            await _col.InsertOneAsync(input);
            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AdministrativeTask update)
        {
            update.Id = id;
            update.UpdatedAt = DateTime.UtcNow;
            var result = await _col.ReplaceOneAsync(t => t.Id == id, update);
            if (result.MatchedCount == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _col.DeleteOneAsync(t => t.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return NoContent();
        }
    }
}
