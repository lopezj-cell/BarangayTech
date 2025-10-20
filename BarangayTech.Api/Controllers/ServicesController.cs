using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly CollectionReference _collection;

        public ServicesController(FirebaseService firebase)
        {
            _collection = firebase.Firestore.Collection("services");
        }

        [HttpGet]
        public async Task<ActionResult<List<Service>>> GetAll()
        {
            var snapshot = await _collection
                .WhereEqualTo("IsActive", true)
                .GetSnapshotAsync();

            var services = snapshot.Documents
                .Select(doc => doc.ConvertTo<Service>())
                .ToList();

            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetById(string id)
        {
            var docRef = _collection.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return NotFound();

            var service = snapshot.ConvertTo<Service>();
            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult<Service>> Create([FromBody] Service input)
        {
            input.Id = null;
            var docRef = await _collection.AddAsync(input);
            input.Id = docRef.Id;

            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Service update)
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
