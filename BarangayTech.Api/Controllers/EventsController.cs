using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly CollectionReference _collection;

        public EventsController(FirebaseService firebase)
        {
            _collection = firebase.Firestore.Collection("events");
        }

        [HttpGet]
        public async Task<ActionResult<List<Event>>> GetAll()
        {
            var snapshot = await _collection
                .OrderBy("Date")
                .GetSnapshotAsync();

            var events = snapshot.Documents
                .Select(doc => doc.ConvertTo<Event>())
                .ToList();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetById(string id)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            var evt = snapshot.ConvertTo<Event>();
            return Ok(evt);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> Create([FromBody] Event input)
        {
            input.Id = null;
            input.CreatedAt = DateTime.UtcNow;

            var docRef = await _collection.AddAsync(input);
            input.Id = docRef.Id;

            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Event update)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            update.Id = id;
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
