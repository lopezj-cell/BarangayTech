using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BarangayTech.Models;

namespace BarangayTech.Services.Auth
{
    public class AuthService
    {
        private static User? _currentUser;
        private static string? _idToken;
        private static string? _refreshToken;
        
        private static readonly HttpClient _httpClient = new HttpClient();

        // Firebase configuration
        private const string FIREBASE_WEB_API_KEY = "AIzaSyDIAfxJmkoQYjEm7TMf1xMvr20YtCHaqLk";
        private const string FIREBASE_PROJECT_ID = "barangaytech";
        private const string FIRESTORE_API_BASE = "https://firestore.googleapis.com/v1";

        public static User? CurrentUser => _currentUser;
        public static bool IsLoggedIn => _currentUser != null && !string.IsNullOrEmpty(_idToken);
        public static string? IdToken => _idToken;

        /// <summary>
        /// Login with email and password using Firebase
        /// </summary>
        public static async Task<AuthResult> LoginAsync(string emailOrUsername, string password)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LOGIN ATTEMPT START (Firebase Only) ===");
                System.Diagnostics.Debug.WriteLine($"Input - Email/Username: {emailOrUsername}");
                
                if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrWhiteSpace(password))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Email/Username and password are required."
                    };
                }

                // Format email
                var email = emailOrUsername.Contains("@") ? emailOrUsername : $"{emailOrUsername}@barangaytech.local";
                System.Diagnostics.Debug.WriteLine($"Formatted Email: {email}");

                // Authenticate with Firebase
                var authUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FIREBASE_WEB_API_KEY}";
                
                var loginRequest = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                System.Diagnostics.Debug.WriteLine("Authenticating with Firebase...");
                var authResponse = await _httpClient.PostAsJsonAsync(authUrl, loginRequest);
                
                if (!authResponse.IsSuccessStatusCode)
                {
                    var errorContent = await authResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {errorContent}");
                    
                    string errorMessage = "Invalid email or password.";
                    if (errorContent.Contains("INVALID_PASSWORD")) errorMessage = "Invalid password.";
                    else if (errorContent.Contains("EMAIL_NOT_FOUND")) errorMessage = $"No account found with email: {email}";
                    else if (errorContent.Contains("USER_DISABLED")) errorMessage = "This account has been disabled.";
                    
                    return new AuthResult { IsSuccess = false, ErrorMessage = errorMessage };
                }

                var firebaseAuthResult = await authResponse.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
                
                if (firebaseAuthResult == null || string.IsNullOrEmpty(firebaseAuthResult.IdToken))
                {
                    return new AuthResult { IsSuccess = false, ErrorMessage = "Authentication failed." };
                }

                _idToken = firebaseAuthResult.IdToken;
                _refreshToken = firebaseAuthResult.RefreshToken;
                var userId = firebaseAuthResult.LocalId;

                System.Diagnostics.Debug.WriteLine($"? Firebase Auth Success - User ID: {userId}");

                // Get user data from Firestore
                System.Diagnostics.Debug.WriteLine("Fetching user data from Firestore...");
                var firestoreUrl = $"{FIRESTORE_API_BASE}/projects/{FIREBASE_PROJECT_ID}/databases/(default)/documents/users/{userId}";
                
                var firestoreRequest = new HttpRequestMessage(HttpMethod.Get, firestoreUrl);
                firestoreRequest.Headers.Add("Authorization", $"Bearer {_idToken}");
                
                var firestoreResponse = await _httpClient.SendAsync(firestoreRequest);
                
                if (!firestoreResponse.IsSuccessStatusCode)
                {
                    var error = await firestoreResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firestore Error: {error}");
                    return new AuthResult 
                    { 
                        IsSuccess = false, 
                        ErrorMessage = "User profile not found. Please contact administrator." 
                    };
                }

                var firestoreDoc = await firestoreResponse.Content.ReadFromJsonAsync<FirestoreDocument>();
                
                if (firestoreDoc?.Fields == null)
                {
                    return new AuthResult { IsSuccess = false, ErrorMessage = "Failed to load user profile." };
                }

                // Map Firestore document to User model
                _currentUser = new User
                {
                    Id = 0, // Not used with Firebase
                    Username = GetStringValue(firestoreDoc.Fields, "Username"),
                    FullName = GetStringValue(firestoreDoc.Fields, "FullName"),
                    Email = GetStringValue(firestoreDoc.Fields, "Email"),
                    ContactNumber = GetStringValue(firestoreDoc.Fields, "ContactNumber"),
                    Role = Enum.Parse<UserRole>(GetStringValue(firestoreDoc.Fields, "Role") ?? "Resident", true),
                    IsActive = GetBoolValue(firestoreDoc.Fields, "IsActive"),
                    Address = GetStringValue(firestoreDoc.Fields, "Address"),
                    ResidentId = GetStringValue(firestoreDoc.Fields, "ResidentId"),
                    Department = GetStringValue(firestoreDoc.Fields, "Department"),
                    Position = GetStringValue(firestoreDoc.Fields, "Position"),
                    LastLoginDate = DateTime.Now
                };

                System.Diagnostics.Debug.WriteLine($"? LOGIN SUCCESS - Welcome {_currentUser.FullName}!");
                System.Diagnostics.Debug.WriteLine("=== LOGIN COMPLETE ===");

                return new AuthResult
                {
                    IsSuccess = true,
                    User = _currentUser,
                    Message = $"Welcome back, {_currentUser.FullName}!"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LOGIN ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Login failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Register a new user (creates Firebase Auth account and Firestore profile)
        /// </summary>
        public static async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string? username = null, string? contactNumber = null, string? address = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== REGISTRATION START (Firebase Only) ===");
                
                // Create Firebase Auth account
                var authUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FIREBASE_WEB_API_KEY}";
                
                var signUpRequest = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var authResponse = await _httpClient.PostAsJsonAsync(authUrl, signUpRequest);
                
                if (!authResponse.IsSuccessStatusCode)
                {
                    var error = await authResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firebase SignUp Error: {error}");
                    
                    string errorMessage = "Registration failed.";
                    if (error.Contains("EMAIL_EXISTS")) errorMessage = "This email is already registered.";
                    else if (error.Contains("WEAK_PASSWORD")) errorMessage = "Password is too weak. Use at least 6 characters.";
                    
                    return new AuthResult { IsSuccess = false, ErrorMessage = errorMessage };
                }

                var firebaseAuthResult = await authResponse.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
                var userId = firebaseAuthResult?.LocalId;
                var idToken = firebaseAuthResult?.IdToken;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(idToken))
                {
                    return new AuthResult { IsSuccess = false, ErrorMessage = "Registration failed." };
                }

                System.Diagnostics.Debug.WriteLine($"? Firebase Auth account created - ID: {userId}");

                // Create user profile in Firestore
                var firestoreUrl = $"{FIRESTORE_API_BASE}/projects/{FIREBASE_PROJECT_ID}/databases/(default)/documents/users/{userId}";
                
                var userProfile = new
                {
                    fields = new
                    {
                        Email = new { stringValue = email },
                        FullName = new { stringValue = fullName },
                        Username = new { stringValue = username ?? email.Split('@')[0] },
                        ContactNumber = new { stringValue = contactNumber ?? "" },
                        Address = new { stringValue = address ?? "" },
                        Role = new { stringValue = "Resident" },
                        IsActive = new { booleanValue = true },
                        EmailVerified = new { booleanValue = false },
                        CreatedDate = new { timestampValue = DateTime.UtcNow.ToString("o") },
                        ResidentId = new { stringValue = $"RES-{DateTime.Now.Year}-{new Random().Next(1, 999):D3}" }
                    }
                };

                var firestoreRequest = new HttpRequestMessage(HttpMethod.Patch, firestoreUrl);
                firestoreRequest.Headers.Add("Authorization", $"Bearer {idToken}");
                firestoreRequest.Content = JsonContent.Create(userProfile);
                
                var firestoreResponse = await _httpClient.SendAsync(firestoreRequest);
                
                if (!firestoreResponse.IsSuccessStatusCode)
                {
                    var error = await firestoreResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firestore Error: {error}");
                    return new AuthResult { IsSuccess = false, ErrorMessage = "Failed to create user profile." };
                }

                System.Diagnostics.Debug.WriteLine("? User profile created in Firestore");
                System.Diagnostics.Debug.WriteLine("=== REGISTRATION COMPLETE ===");

                return new AuthResult
                {
                    IsSuccess = true,
                    Message = "Registration successful! You can now login."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"REGISTRATION ERROR: {ex.Message}");
                return new AuthResult { IsSuccess = false, ErrorMessage = $"Registration failed: {ex.Message}" };
            }
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public static void Logout()
        {
            _currentUser = null;
            _idToken = null;
            _refreshToken = null;
        }

        /// <summary>
        /// Check if user has specific permission based on role
        /// </summary>
        public static bool HasPermission(string permission)
        {
            if (_currentUser == null) return false;

            return _currentUser.Role switch
            {
                UserRole.SuperAdmin => true,
                UserRole.Admin => permission != "system_management",
                UserRole.Resident => permission is "view_events" or "view_announcements" or "view_services" or "contact_officials",
                _ => false
            };
        }

        /// <summary>
        /// Get current user's auth token for API calls
        /// </summary>
        public static string? GetAuthToken()
        {
            return _idToken;
        }

        /// <summary>
        /// Send password reset email
        /// </summary>
        public static async Task<AuthResult> ResetPasswordAsync(string email)
        {
            try
            {
                var resetUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={FIREBASE_WEB_API_KEY}";
                
                var resetRequest = new
                {
                    requestType = "PASSWORD_RESET",
                    email = email
                };

                var response = await _httpClient.PostAsJsonAsync(resetUrl, resetRequest);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Password Reset Error: {error}");
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Failed to send password reset email."
                    };
                }

                return new AuthResult
                {
                    IsSuccess = true,
                    Message = "Password reset email sent. Please check your inbox."
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Failed to reset password: {ex.Message}"
                };
            }
        }

        // Helper methods for parsing Firestore document fields
        private static string? GetStringValue(System.Collections.Generic.Dictionary<string, FirestoreValue> fields, string fieldName)
        {
            return fields.TryGetValue(fieldName, out var value) ? value.StringValue : null;
        }

        private static bool GetBoolValue(System.Collections.Generic.Dictionary<string, FirestoreValue> fields, string fieldName)
        {
            return fields.TryGetValue(fieldName, out var value) && value.BooleanValue == true;
        }
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public User? User { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Firebase Auth Response
    internal class FirebaseAuthResponse
    {
        public string? IdToken { get; set; }
        public string? Email { get; set; }
        public string? RefreshToken { get; set; }
        public string? ExpiresIn { get; set; }
        public string? LocalId { get; set; }
    }

    // Firestore Document Structure
    internal class FirestoreDocument
    {
        public string? Name { get; set; }
        public System.Collections.Generic.Dictionary<string, FirestoreValue>? Fields { get; set; }
        public string? CreateTime { get; set; }
        public string? UpdateTime { get; set; }
    }

    internal class FirestoreValue
    {
        public string? StringValue { get; set; }
        public bool? BooleanValue { get; set; }
        public long? IntegerValue { get; set; }
        public double? DoubleValue { get; set; }
        public string? TimestampValue { get; set; }
    }
}
