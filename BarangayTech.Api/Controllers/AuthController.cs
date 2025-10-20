using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using BarangayTech.Api.Services;
using BarangayTech.Api.Models;
using BarangayTech.Api.Models.DTOs;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CollectionReference _usersCollection;
        private readonly ILogger<AuthController> _logger;

        public AuthController(FirebaseService firebase, ILogger<AuthController> logger)
        {
            _usersCollection = firebase.Firestore.Collection("users");
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("=== REGISTRATION ATTEMPT ===");
                _logger.LogInformation($"Email: {request.Email}");
                _logger.LogInformation($"Role: {request.Role}");
                
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Registration failed: Email or password missing");
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Error = "Email and password are required."
                    });
                }

                if (request.Password.Length < 6)
                {
                    _logger.LogWarning("Registration failed: Password too short");
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Error = "Password must be at least 6 characters long."
                    });
                }

                // Check if username already exists
                if (!string.IsNullOrWhiteSpace(request.Username))
                {
                    _logger.LogInformation($"Checking if username exists: {request.Username}");
                    var usernameQuery = await _usersCollection
                        .WhereEqualTo("Username", request.Username)
                        .Limit(1)
                        .GetSnapshotAsync();

                    if (usernameQuery.Documents.Count > 0)
                    {
                        _logger.LogWarning($"Username already exists: {request.Username}");
                        return BadRequest(new AuthResponse
                        {
                            Success = false,
                            Error = "Username already exists."
                        });
                    }
                }

                _logger.LogInformation("Creating Firebase Auth user...");
                // Create Firebase Auth user
                var userRecordArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.FullName,
                    EmailVerified = false
                };

                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
                _logger.LogInformation($"? Firebase user created with UID: {userRecord.Uid}");

                // Set custom claims for role
                var claims = new Dictionary<string, object>
                {
                    { "role", request.Role }
                };
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
                _logger.LogInformation($"? Custom claims set: role={request.Role}");

                // Create user document in Firestore
                var user = new User
                {
                    Id = userRecord.Uid,
                    Email = request.Email,
                    FullName = request.FullName,
                    Username = request.Username ?? request.Email.Split('@')[0],
                    ContactNumber = request.ContactNumber,
                    Address = request.Address,
                    Role = request.Role,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    EmailVerified = false
                };

                // Generate ResidentId if role is Resident
                if (request.Role == "Resident")
                {
                    user.ResidentId = await GenerateResidentId();
                    _logger.LogInformation($"? Resident ID generated: {user.ResidentId}");
                }

                await _usersCollection.Document(userRecord.Uid).SetAsync(user);
                _logger.LogInformation("? User document created in Firestore");

                // Send verification email
                var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(request.Email);
                _logger.LogInformation($"? Verification email link generated");
                // TODO: Send email with verificationLink

                _logger.LogInformation($"??? REGISTRATION SUCCESSFUL for {request.Email} ???");

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful! Please check your email to verify your account.",
                    User = MapToUserData(user)
                });
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, "Firebase Auth error during registration");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Error = "An error occurred during registration. Please try again."
                });
            }
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Note: Firebase doesn't provide server-side login with password
                // The client should use Firebase SDK to sign in and send the ID token here
                // This endpoint validates the token and returns user data

                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Error = "Please use the client SDK to sign in with Firebase Authentication. Then call /verify-token with your ID token."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Error = "An error occurred during login. Please try again."
                });
            }
        }

        /// <summary>
        /// Verify Firebase ID token and return user data
        /// </summary>
        [HttpPost("verify-token")]
        public async Task<ActionResult<AuthResponse>> VerifyToken([FromBody] string idToken)
        {
            try
            {
                // Verify the ID token
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                var uid = decodedToken.Uid;

                // Get user from Firestore
                var userDoc = await _usersCollection.Document(uid).GetSnapshotAsync();

                if (!userDoc.Exists)
                {
                    return NotFound(new AuthResponse
                    {
                        Success = false,
                        Error = "User not found."
                    });
                }

                var user = userDoc.ConvertTo<User>();

                if (!user.IsActive)
                {
                    return Unauthorized(new AuthResponse
                    {
                        Success = false,
                        Error = "Your account has been deactivated. Please contact support."
                    });
                }

                // Update last login
                user.LastLoginDate = DateTime.UtcNow;
                await _usersCollection.Document(uid).SetAsync(user, SetOptions.MergeAll);

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Login successful!",
                    User = MapToUserData(user),
                    IdToken = idToken
                });
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, "Invalid token");
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Error = "Invalid or expired token."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token");
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Error = "An error occurred. Please try again."
                });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<UserData>> GetProfile([FromHeader(Name = "Authorization")] string authorization)
        {
            try
            {
                var idToken = ExtractToken(authorization);
                if (string.IsNullOrEmpty(idToken))
                {
                    return Unauthorized(new { error = "No token provided" });
                }

                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                var uid = decodedToken.Uid;

                var userDoc = await _usersCollection.Document(uid).GetSnapshotAsync();
                if (!userDoc.Exists)
                {
                    return NotFound(new { error = "User not found" });
                }

                var user = userDoc.ConvertTo<User>();
                return Ok(MapToUserData(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile([FromHeader(Name = "Authorization")] string authorization, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var idToken = ExtractToken(authorization);
                if (string.IsNullOrEmpty(idToken))
                {
                    return Unauthorized(new { error = "No token provided" });
                }

                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                var uid = decodedToken.Uid;

                var updates = new Dictionary<string, object>();
                
                if (!string.IsNullOrWhiteSpace(request.FullName))
                    updates["FullName"] = request.FullName;
                
                if (!string.IsNullOrWhiteSpace(request.Username))
                    updates["Username"] = request.Username;
                
                if (!string.IsNullOrWhiteSpace(request.ContactNumber))
                    updates["ContactNumber"] = request.ContactNumber;
                
                if (!string.IsNullOrWhiteSpace(request.Address))
                    updates["Address"] = request.Address;
                
                if (!string.IsNullOrWhiteSpace(request.PhotoUrl))
                    updates["PhotoUrl"] = request.PhotoUrl;

                if (updates.Count > 0)
                {
                    await _usersCollection.Document(uid).UpdateAsync(updates);
                }

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Send password reset email
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var resetLink = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(request.Email);
                // TODO: Send email with resetLink

                return Ok(new { message = "Password reset email sent. Please check your inbox." });
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, "Error sending password reset");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset");
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

        private async Task<string> GenerateResidentId()
        {
            var year = DateTime.Now.Year;
            var query = await _usersCollection
                .WhereEqualTo("Role", "Resident")
                .OrderByDescending("CreatedDate")
                .Limit(1)
                .GetSnapshotAsync();

            int nextNumber = 1;
            if (query.Documents.Count > 0)
            {
                var lastUser = query.Documents[0].ConvertTo<User>();
                if (!string.IsNullOrEmpty(lastUser.ResidentId))
                {
                    var parts = lastUser.ResidentId.Split('-');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
            }

            return $"RES-{year}-{nextNumber:D3}";
        }
    }
}
