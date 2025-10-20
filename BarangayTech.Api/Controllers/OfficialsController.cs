using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfficialsController : ControllerBase
    {
        private readonly CollectionReference _collection;

        public OfficialsController(FirebaseService firebase)
        {
            _collection = firebase.Firestore.Collection("officials");
        }

        [HttpGet]
        public async Task<ActionResult<List<Official>>> GetAll()
        {
            var snapshot = await _collection
                .WhereEqualTo("IsActive", true)
                .OrderBy("DisplayOrder")
                .GetSnapshotAsync();

            var officials = snapshot.Documents
                .Select(doc => doc.ConvertTo<Official>())
                .ToList();

            return Ok(officials);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Official>> GetById(string id)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            var official = snapshot.ConvertTo<Official>();
            return Ok(official);
        }

        [HttpPost]
        public async Task<ActionResult<Official>> Create([FromBody] Official input)
        {
            input.Id = null;
            var docRef = await _collection.AddAsync(input);
            input.Id = docRef.Id;

            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Official update)
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
