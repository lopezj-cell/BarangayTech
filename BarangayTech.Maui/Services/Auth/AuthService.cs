using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BarangayTech.Models;

namespace BarangayTech.Services.Auth
{
    public class AuthService
    {
        private static User? _currentUser;
        private static string? _idToken;
        private static readonly HttpClient _httpClient = new HttpClient();

        // Firebase Configuration
        private const string FIREBASE_WEB_API_KEY = "AIzaSyDlAfxJmkoQYjEm7TMf1xMv2r0YtCHaqLk";
        private const string FIREBASE_AUTH_URL = "https://identitytoolkit.googleapis.com/v1/accounts";
        private const string FIRESTORE_API = "https://firestore.googleapis.com/v1/projects/barangaytech/databases/(default)/documents";

        public static User? CurrentUser => _currentUser;
        public static bool IsLoggedIn => _currentUser != null && !string.IsNullOrEmpty(_idToken);
        public static string? IdToken => _idToken;

        /// <summary>
        /// Login with email and password using Firebase (NO BACKEND REQUIRED)
        /// </summary>
        public static async Task<AuthResult> LoginAsync(string emailOrUsername, string password)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== PURE FIREBASE LOGIN ===");
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
                System.Diagnostics.Debug.WriteLine($"Email: {email}");

                // Step 1: Authenticate with Firebase Auth
                var loginRequest = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var authUrl = $"{FIREBASE_AUTH_URL}:signInWithPassword?key={FIREBASE_WEB_API_KEY}";
                var authResponse = await _httpClient.PostAsJsonAsync(authUrl, loginRequest);
                
                if (!authResponse.IsSuccessStatusCode)
                {
                    var errorContent = await authResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {errorContent}");
                    
                    if (errorContent.Contains("INVALID_PASSWORD"))
                        return new AuthResult { IsSuccess = false, ErrorMessage = "Invalid password." };
                    if (errorContent.Contains("EMAIL_NOT_FOUND"))
                        return new AuthResult { IsSuccess = false, ErrorMessage = $"No user found with email: {email}" };
                    if (errorContent.Contains("USER_DISABLED"))
                        return new AuthResult { IsSuccess = false, ErrorMessage = "Account disabled." };
                    
                    return new AuthResult { IsSuccess = false, ErrorMessage = "Invalid email or password." };
                }

                var firebaseAuth = await authResponse.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
                if (firebaseAuth == null || string.IsNullOrEmpty(firebaseAuth.IdToken))
                {
                    return new AuthResult { IsSuccess = false, ErrorMessage = "Authentication failed." };
                }

                _idToken = firebaseAuth.IdToken;
                var userId = firebaseAuth.LocalId;
                System.Diagnostics.Debug.WriteLine($"✓ Firebase Auth Success - UID: {userId}");

                // Step 2: Get user data from Firestore
                var firestoreUrl = $"{FIRESTORE_API}/users/{userId}";
                var request = new HttpRequestMessage(HttpMethod.Get, firestoreUrl);
                request.Headers.Add("Authorization", $"Bearer {_idToken}");

                var firestoreResponse = await _httpClient.SendAsync(request);
                
                if (!firestoreResponse.IsSuccessStatusCode)
                {
                    var error = await firestoreResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firestore Error: {error}");
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "User data not found. Please contact administrator."
                    };
                }

                var firestoreDoc = await firestoreResponse.Content.ReadFromJsonAsync<FirestoreDocument>();
                if (firestoreDoc == null || firestoreDoc.Fields == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Failed to load user profile."
                    };
                }

                // Step 3: Map Firestore data to User model
                _currentUser = new User
                {
                    Id = 0, // Not used anymore
                    Username = GetStringValue(firestoreDoc.Fields, "Username"),
                    FullName = GetStringValue(firestoreDoc.Fields, "FullName"),
                    Email = GetStringValue(firestoreDoc.Fields, "Email"),
                    ContactNumber = GetStringValue(firestoreDoc.Fields, "ContactNumber"),
                    Role = Enum.TryParse<UserRole>(GetStringValue(firestoreDoc.Fields, "Role"), out var role) ? role : UserRole.Resident,
                    IsActive = GetBoolValue(firestoreDoc.Fields, "IsActive"),
                    Address = GetStringValue(firestoreDoc.Fields, "Address"),
                    ResidentId = GetStringValue(firestoreDoc.Fields, "ResidentId"),
                    Department = GetStringValue(firestoreDoc.Fields, "Department"),
                    Position = GetStringValue(firestoreDoc.Fields, "Position"),
                    LastLoginDate = DateTime.Now
                };

                System.Diagnostics.Debug.WriteLine($"✓✓✓ LOGIN SUCCESS - Welcome {_currentUser.FullName}!");
                
                return new AuthResult
                {
                    IsSuccess = true,
                    User = _currentUser,
                    Message = $"Welcome back, {_currentUser.FullName}!"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login Exception: {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Login failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Register a new user (Requires backend API for Firebase Admin SDK)
        /// </summary>
        public static async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string? username = null, string? contactNumber = null, string? address = null)
        {
            // Registration still needs backend because it requires Firebase Admin SDK to create users
            return new AuthResult
            {
                IsSuccess = false,
                ErrorMessage = "Registration must be done through the backend API or Firebase Console."
            };
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public static void Logout()
        {
            _currentUser = null;
            _idToken = null;
            System.Diagnostics.Debug.WriteLine("User logged out");
        }

        /// <summary>
        /// Check if user has specific permission based on role
        /// </summary>
        public static bool HasPermission(string permission)
        {
            if (_currentUser == null)
                return false;

            return _currentUser.Role switch
            {
                UserRole.SuperAdmin => true,
                UserRole.Admin => permission != "system_management",
                UserRole.Resident =>
                    permission == "view_events" ||
                    permission == "view_announcements" ||
                    permission == "view_services" ||
                    permission == "contact_officials",
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
                var resetUrl = $"{FIREBASE_AUTH_URL}:sendOobCode?key={FIREBASE_WEB_API_KEY}";
                var resetRequest = new
                {
                    requestType = "PASSWORD_RESET",
                    email = email
                };

                var response = await _httpClient.PostAsJsonAsync(resetUrl, resetRequest);
                
                if (!response.IsSuccessStatusCode)
                {
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

        // Helper methods for Firestore field extraction
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

    // Firestore Document Response
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
