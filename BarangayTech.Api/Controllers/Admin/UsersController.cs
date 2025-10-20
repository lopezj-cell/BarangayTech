using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;
using BarangayTech.Api.Models.DTOs;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly CollectionReference _usersCollection;
        private readonly ILogger<UsersController> _logger;

        public UsersController(FirebaseService firebase, ILogger<UsersController> logger)
        {
            _usersCollection = firebase.Firestore.Collection("users");
            _logger = logger;
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UserData>>> GetAllUsers([FromHeader(Name = "Authorization")] string authorization, [FromQuery] string? role = null)
        {
            try
            {
                // Verify admin
                if (!await IsAdmin(authorization))
                {
                    return Unauthorized(new { error = "Unauthorized. Admin access required." });
                }

                Query query = _usersCollection;

                if (!string.IsNullOrWhiteSpace(role))
                {
                    query = query.WhereEqualTo("Role", role);
                }

                var snapshot = await query.GetSnapshotAsync();
                var users = snapshot.Documents
                    .Select(doc => MapToUserData(doc.ConvertTo<User>()))
                    .ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserData>> GetUserById([FromHeader(Name = "Authorization")] string authorization, string id)
        {
            try
            {
                // Verify admin
                if (!await IsAdmin(authorization))
                {
                    return Unauthorized(new { error = "Unauthorized. Admin access required." });
                }

                var userDoc = await _usersCollection.Document(id).GetSnapshotAsync();

                if (!userDoc.Exists)
                {
                    return NotFound(new { error = "User not found" });
                }

                var user = userDoc.ConvertTo<User>();
                return Ok(MapToUserData(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Update user (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser([FromHeader(Name = "Authorization")] string authorization, string id, [FromBody] User updateData)
        {
            try
            {
                // Verify admin
                if (!await IsAdmin(authorization))
                {
                    return Unauthorized(new { error = "Unauthorized. Admin access required." });
                }

                var userDoc = await _usersCollection.Document(id).GetSnapshotAsync();

                if (!userDoc.Exists)
                {
                    return NotFound(new { error = "User not found" });
                }

                var updates = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(updateData.FullName))
                    updates["FullName"] = updateData.FullName;

                if (!string.IsNullOrWhiteSpace(updateData.Username))
                    updates["Username"] = updateData.Username;

                if (!string.IsNullOrWhiteSpace(updateData.ContactNumber))
                    updates["ContactNumber"] = updateData.ContactNumber;

                if (!string.IsNullOrWhiteSpace(updateData.Address))
                    updates["Address"] = updateData.Address;

                if (!string.IsNullOrWhiteSpace(updateData.Role))
                {
                    updates["Role"] = updateData.Role;
                    // Update Firebase custom claims
                    await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(id, new Dictionary<string, object>
                    {
                        { "role", updateData.Role }
                    });
                }

                if (!string.IsNullOrWhiteSpace(updateData.Department))
                    updates["Department"] = updateData.Department;

                if (!string.IsNullOrWhiteSpace(updateData.Position))
                    updates["Position"] = updateData.Position;

                updates["IsActive"] = updateData.IsActive;

                if (updates.Count > 0)
                {
                    await _usersCollection.Document(id).UpdateAsync(updates);
                }

                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Deactivate user (Admin only)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> DeactivateUser([FromHeader(Name = "Authorization")] string authorization, string id)
        {
            try
            {
                // Verify admin
                if (!await IsAdmin(authorization))
                {
                    return Unauthorized(new { error = "Unauthorized. Admin access required." });
                }

                await _usersCollection.Document(id).UpdateAsync(new Dictionary<string, object>
                {
                    { "IsActive", false }
                });

                // Disable in Firebase Auth
                await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
                {
                    Uid = id,
                    Disabled = true
                });

                return Ok(new { message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Activate user (Admin only)
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<ActionResult> ActivateUser([FromHeader(Name = "Authorization")] string authorization, string id)
        {
            try
            {
                // Verify admin
                if (!await IsAdmin(authorization))
                {
                    return Unauthorized(new { error = "Unauthorized. Admin access required." });
                }

                await _usersCollection.Document(id).UpdateAsync(new Dictionary<string, object>
                {
                    { "IsActive", true }
                });

                // Enable in Firebase Auth
                await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
                {
                    Uid = id,
                    Disabled = false
                });

                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Delete user (SuperAdmin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser([FromHeader(Name = "Authorization")] string authorization, string id)
        {
            try
            {
                // Verify super admin
                if (!await IsSuperAdmin(authorization))
                {
                    return Unauthorized(new { error = "Unauthorized. SuperAdmin access required." });
                }

                // Delete from Firestore
                await _usersCollection.Document(id).DeleteAsync();

                // Delete from Firebase Auth
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(id);

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        // Helper methods
        private string? ExtractToken(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return null;

            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return authorization.Substring("Bearer ".Length).Trim();

            return authorization;
        }

        private async Task<bool> IsAdmin(string authorization)
        {
            try
            {
                var idToken = ExtractToken(authorization);
                if (string.IsNullOrEmpty(idToken))
                    return false;

                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                
                if (decodedToken.Claims.TryGetValue("role", out var roleClaim))
                {
                    var role = roleClaim.ToString();
                    return role == "Admin" || role == "SuperAdmin";
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> IsSuperAdmin(string authorization)
        {
            try
            {
                var idToken = ExtractToken(authorization);
                if (string.IsNullOrEmpty(idToken))
                    return false;

                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                
                if (decodedToken.Claims.TryGetValue("role", out var roleClaim))
                {
                    return roleClaim.ToString() == "SuperAdmin";
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private UserData MapToUserData(User user)
        {
            return new UserData
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                ContactNumber = user.ContactNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                Address = user.Address,
                ResidentId = user.ResidentId,
                Department = user.Department,
                Position = user.Position,
                PhotoUrl = user.PhotoUrl
            };
        }
    }
}
