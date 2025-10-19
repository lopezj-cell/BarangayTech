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

        /// <summary>
        /// Login with email and password using Firebase
        /// </summary>
        public static async Task<AuthResult> LoginAsync(string emailOrUsername, string password)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LOGIN ATTEMPT START ===");
                System.Diagnostics.Debug.WriteLine($"Input - Email/Username: {emailOrUsername}");
                
                if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrWhiteSpace(password))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Email/Username or password is empty");
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Email/Username and password are required."
                    };
                }

                // Determine if email or username was provided
                var email = emailOrUsername.Contains("@") ? emailOrUsername : $"{emailOrUsername}@barangaytech.local";
                System.Diagnostics.Debug.WriteLine($"Formatted Email: {email}");
                System.Diagnostics.Debug.WriteLine($"Password Length: {password.Length} characters");

                // Try to login with Firebase Auth REST API
                var firebaseLoginUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FIREBASE_WEB_API_KEY}";
                System.Diagnostics.Debug.WriteLine($"Firebase URL: {firebaseLoginUrl}");
                
                var loginRequest = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                System.Diagnostics.Debug.WriteLine("Sending request to Firebase...");
                var response = await _httpClient.PostAsJsonAsync(firebaseLoginUrl, loginRequest);
                System.Diagnostics.Debug.WriteLine($"Firebase Response Status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firebase Error Response: {errorContent}");
                    
                    // Parse Firebase error for more details
                    string detailedError = "Invalid email or password.";
                    if (errorContent.Contains("INVALID_PASSWORD"))
                    {
                        detailedError = "Invalid password.";
                        System.Diagnostics.Debug.WriteLine("ERROR: Wrong password provided");
                    }
                    else if (errorContent.Contains("EMAIL_NOT_FOUND"))
                    {
                        detailedError = $"No user found with email: {email}";
                        System.Diagnostics.Debug.WriteLine($"ERROR: User not found in Firebase Auth with email: {email}");
                    }
                    else if (errorContent.Contains("USER_DISABLED"))
                    {
                        detailedError = "This account has been disabled.";
                        System.Diagnostics.Debug.WriteLine("ERROR: User account is disabled");
                    }
                    
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = detailedError
                    };
                }

                var firebaseResponse = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
                System.Diagnostics.Debug.WriteLine($"Firebase Token Received: {firebaseResponse?.IdToken?.Substring(0, 20)}...");
                
                if (firebaseResponse == null || string.IsNullOrEmpty(firebaseResponse.IdToken))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: No token received from Firebase");
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Authentication failed - no token received."
                    };
                }

                _idToken = firebaseResponse.IdToken;
                System.Diagnostics.Debug.WriteLine($"Local ID from Firebase: {firebaseResponse.LocalId}");

                // Verify token and get user data from our API
                System.Diagnostics.Debug.WriteLine("Verifying token with backend API...");
                System.Diagnostics.Debug.WriteLine($"Backend URL: {_httpClient.BaseAddress}api/auth/verify-token");
                
                var verifyResponse = await _httpClient.PostAsJsonAsync("api/auth/verify-token", _idToken);
                System.Diagnostics.Debug.WriteLine($"Backend Response Status: {verifyResponse.StatusCode}");
                
                if (!verifyResponse.IsSuccessStatusCode)
                {
                    var backendError = await verifyResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Backend Error: {backendError}");
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Failed to verify user credentials with backend."
                    };
                }

                var authResponse = await verifyResponse.Content.ReadFromJsonAsync<ApiAuthResponse>();
                System.Diagnostics.Debug.WriteLine($"Backend Success: {authResponse?.Success}");
                System.Diagnostics.Debug.WriteLine($"User ID: {authResponse?.User?.Id}");
                System.Diagnostics.Debug.WriteLine($"User Role: {authResponse?.User?.Role}");
                
                if (authResponse == null || authResponse.User == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: No user data received from backend");
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Failed to retrieve user information."
                    };
                }

                // Map API user to local user model
                _currentUser = new User
                {
                    Id = int.Parse(authResponse.User.Id ?? "0"),
                    Username = authResponse.User.Username,
                    FullName = authResponse.User.FullName,
                    Email = authResponse.User.Email,
                    ContactNumber = authResponse.User.ContactNumber,
                    Role = Enum.Parse<UserRole>(authResponse.User.Role ?? "Resident", true),
                    IsActive = authResponse.User.IsActive,
                    Address = authResponse.User.Address,
                    ResidentId = authResponse.User.ResidentId,
                    Department = authResponse.User.Department,
                    Position = authResponse.User.Position,
                    LastLoginDate = DateTime.Now
                };

                System.Diagnostics.Debug.WriteLine($"? LOGIN SUCCESS - Welcome {_currentUser.FullName}!");
                System.Diagnostics.Debug.WriteLine("=== LOGIN ATTEMPT END ===");

                return new AuthResult
                {
                    IsSuccess = true,
                    User = _currentUser,
                    Message = $"Welcome back, {_currentUser.FullName}!"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== EXCEPTION OCCURRED ===");
                System.Diagnostics.Debug.WriteLine($"Exception Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Exception Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Login failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public static async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string? username = null, string? contactNumber = null, string? address = null)
        {
            try
            {
                var registerRequest = new
                {
                    email = email,
                    password = password,
                    fullName = fullName,
                    username = username ?? email.Split('@')[0],
                    contactNumber = contactNumber,
                    address = address,
                    role = "Resident" // Default role
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Registration failed. Please try again."
                    };
                }

                var authResponse = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();

                return new AuthResult
                {
                    IsSuccess = true,
                    Message = authResponse?.Message ?? "Registration successful! Please check your email to verify your account."
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Registration failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public static void Logout()
        {
            _currentUser = null;
            _idToken = null;
        }

        /// <summary>
        /// Check if user has specific permission based on role
        /// </summary>
        public static bool HasPermission(string permission)
        {
            if (_currentUser == null)
            {
                return false;
            }

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
                var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", new { email = email });
                
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
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public User? User { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Firebase Auth Response models
    internal class FirebaseAuthResponse
    {
        public string? IdToken { get; set; }
        public string? Email { get; set; }
        public string? RefreshToken { get; set; }
        public string? ExpiresIn { get; set; }
        public string? LocalId { get; set; }
    }

    internal class ApiAuthResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
        public UserData? User { get; set; }
        public string? IdToken { get; set; }
    }

    internal class UserData
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public string? Address { get; set; }
        public string? ResidentId { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
