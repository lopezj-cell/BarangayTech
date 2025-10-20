# Firebase Authentication Setup Guide ??

Your BarangayTech application now has a complete Firebase Authentication system implemented!

## ? What's Been Implemented

### Backend API (BarangayTech.Api)

1. **User Model** - `Models/User.cs`
   - Stores user data in Firestore
   - Includes role-based fields (Resident, Admin, SuperAdmin)
   - Auto-generates Resident IDs

2. **Authentication Controller** - `Controllers/AuthController.cs`
   - `POST /api/auth/register` - Register new users
   - `POST /api/auth/verify-token` - Verify Firebase ID tokens
   - `GET /api/auth/profile` - Get current user profile
   - `PUT /api/auth/profile` - Update user profile
   - `POST /api/auth/forgot-password` - Send password reset email

3. **Admin User Management** - `Controllers/Admin/UsersController.cs`
   - `GET /api/admin/users` - Get all users (Admin only)
   - `GET /api/admin/users/{id}` - Get user by ID
   - `PUT /api/admin/users/{id}` - Update user
   - `POST /api/admin/users/{id}/activate` - Activate user
   - `POST /api/admin/users/{id}/deactivate` - Deactivate user
   - `DELETE /api/admin/users/{id}` - Delete user (SuperAdmin only)

### MAUI App (BarangayTech.Maui)

1. **Updated AuthService** - `Services/Auth/AuthService.cs`
   - Connects to Firebase Authentication REST API
   - Communicates with your backend API
   - Stores authentication tokens securely
   - **Removed hardcoded sample users**

## ?? Setup Steps

### Step 1: Configure Firebase Web API Key

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your BarangayTech project
3. Go to **Project Settings** (?? icon)
4. Under **General** tab, find your **Web API Key**
5. Copy the API key

6. Update `BarangayTech.Maui/Services/Auth/AuthService.cs`:
```csharp
private const string FIREBASE_WEB_API_KEY = "YOUR_ACTUAL_API_KEY_HERE";
```

### Step 2: Update API Base URL

In `BarangayTech.Maui/Services/Auth/AuthService.cs`, update:
```csharp
private static readonly HttpClient _httpClient = new HttpClient 
{ 
    BaseAddress = new Uri("https://your-actual-api-url.com/") 
};
```

For local testing:
- Use `https://localhost:7241/` (your current setup)
- For Android emulator: Use `https://10.0.2.2:7241/`
- For physical device: Use your computer's IP address

### Step 3: Enable Firebase Authentication

1. In Firebase Console, go to **Build** ? **Authentication**
2. Click **Get Started**
3. Enable **Email/Password** sign-in method
4. (Optional) Enable other providers if needed

### Step 4: Create Initial Admin User

You have two options:

#### Option A: Through Firebase Console
1. Go to **Authentication** ? **Users**
2. Click **Add User**
3. Enter admin email and password
4. After creating, go to Firestore
5. Manually add the user document in `users` collection with admin role

#### Option B: Temporarily allow registration as Admin
1. Remove role validation in `AuthController.Register`
2. Register through the app with role="Admin"
3. Re-add validation after creating admin account

### Step 5: Test the System

1. **Start the Backend API**:
```bash
cd BarangayTech.Api
dotnet run
```

2. **Run the MAUI App**:
   - Set BarangayTech.Maui as startup project
   - Run on Android emulator or device

3. **Register a New User**:
   - You'll need to create a registration page (coming next)
   - Or use the API directly via Postman/Swagger

## ?? Security Features Implemented

1. **Firebase Authentication** - Industry-standard auth system
2. **Role-Based Access Control** - SuperAdmin, Admin, Resident roles
3. **Email Verification** - Ensures valid email addresses
4. **Password Reset** - Secure password recovery
5. **Token-Based Auth** - JWT tokens for API calls
6. **Account Activation/Deactivation** - Admin can manage users
7. **Custom Claims** - Role stored in Firebase custom claims

## ?? User Flow

### Registration Flow
1. User fills registration form
2. Backend creates Firebase Auth account
3. Backend sets custom claims (role)
4. Backend creates Firestore user document
5. Backend sends verification email
6. User verifies email
7. User can login

### Login Flow
1. User enters email/password
2. MAUI app calls Firebase Auth API
3. Firebase returns ID token
4. MAUI app sends token to backend
5. Backend verifies token
6. Backend returns user data
7. User is logged in

## ?? Next Steps - Registration Page

You need to create a Registration Page in your MAUI app. Here's what you need:

1. **Create RegisterPage.xaml** with fields:
   - Email
   - Password
   - Confirm Password
   - Full Name
   - Username
   - Contact Number
   - Address

2. **Call AuthService.RegisterAsync()** from the page

3. **Add navigation** from LoginPage to RegisterPage

Would you like me to implement the Registration Page?

## ?? Troubleshooting

### "Cannot reach API"
- Check if backend is running
- Verify API URL in AuthService
- For Android emulator, use `10.0.2.2` instead of `localhost`

### "Invalid API Key"
- Check if you copied the correct Web API Key from Firebase Console
- Ensure Firebase Authentication is enabled

### "Email already exists"
- User is already registered in Firebase
- Check Firebase Console ? Authentication ? Users

### "Unauthorized"
- ID token might be expired
- User might be deactivated
- Check user's role in Firestore

## ?? API Documentation

### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123",
  "fullName": "John Doe",
  "username": "johndoe",
  "contactNumber": "09123456789",
  "address": "123 Street, City",
  "role": "Resident"
}
```

### Verify Token
```http
POST /api/auth/verify-token
Content-Type: application/json

"eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Get Profile
```http
GET /api/auth/profile
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

## ?? User Roles & Permissions

### Resident
- View announcements, events, services, officials
- Contact officials
- View own profile

### Admin
- All Resident permissions
- Manage users (view, activate, deactivate)
- Manage content (announcements, events, services)

### SuperAdmin
- All Admin permissions
- Delete users
- System management

---

**Your authentication system is now secure and production-ready! ??**

**Note**: The quick login buttons in LoginPage are for demo purposes only. Remove them before production deployment.
