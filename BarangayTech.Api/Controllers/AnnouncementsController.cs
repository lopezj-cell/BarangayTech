using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly CollectionReference _collection;

        public AnnouncementsController(FirebaseService firebase)
        {
            _collection = firebase.Firestore.Collection("announcements");
        }

        [HttpGet]
        public async Task<ActionResult<List<Announcement>>> GetAll()
        {
            var snapshot = await _collection
                .OrderByDescending("DatePosted")
                .GetSnapshotAsync();

            var announcements = snapshot.Documents
                .Select(doc => doc.ConvertTo<Announcement>())
                .ToList();

            return Ok(announcements);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Announcement>> GetById(string id)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            var announcement = snapshot.ConvertTo<Announcement>();
            return Ok(announcement);
        }

        [HttpPost]
        public async Task<ActionResult<Announcement>> Create([FromBody] Announcement input)
        {
            input.Id = null;
            input.DatePosted = DateTime.UtcNow;

            var docRef = await _collection.AddAsync(input);
            input.Id = docRef.Id;

            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Announcement update)
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
