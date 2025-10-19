using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministrativeTasksController : ControllerBase
    {
        private readonly CollectionReference _collection;

        public AdministrativeTasksController(FirebaseService firebase)
        {
            _collection = firebase.Firestore.Collection("administrative_tasks");
        }

        [HttpGet]
        public async Task<ActionResult<List<AdministrativeTask>>> GetAll()
        {
            var snapshot = await _collection
                .OrderByDescending("CreatedAt")
                .GetSnapshotAsync();

            var tasks = snapshot.Documents
                .Select(doc => doc.ConvertTo<AdministrativeTask>())
                .ToList();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdministrativeTask>> GetById(string id)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            var task = snapshot.ConvertTo<AdministrativeTask>();
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<AdministrativeTask>> Create([FromBody] AdministrativeTask input)
        {
            input.Id = null;
            input.CreatedAt = DateTime.UtcNow;
            input.UpdatedAt = DateTime.UtcNow;

            var docRef = await _collection.AddAsync(input);
            input.Id = docRef.Id;

            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AdministrativeTask update)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            update.Id = id;
            update.UpdatedAt = DateTime.UtcNow;

            await docRef.SetAsync(update, SetOptions.Overwrite);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            await docRef.DeleteAsync();
            return NoContent();
        }
    }
}
