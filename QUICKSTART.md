# Quick Start Guide - Firebase Authentication ??

## 3-Minute Setup

### Step 1: Firebase Console (2 minutes)
1. Go to https://console.firebase.google.com/
2. Select your **BarangayTech** project
3. **Enable Authentication**:
   - Build ? Authentication ? Get Started
   - Email/Password ? Enable ? Save

4. **Get Web API Key**:
   - ?? Settings ? Project Settings ? General
   - Copy "Web API Key"

### Step 2: Update MAUI App (1 minute)
Open `BarangayTech.Maui/Services/Auth/AuthService.cs`

Find line 17:
```csharp
private const string FIREBASE_WEB_API_KEY = "YOUR_FIREBASE_WEB_API_KEY";
```

Replace with your actual key:
```csharp
private const string FIREBASE_WEB_API_KEY = "AIzaSyBxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
```

### Step 3: Run & Test
1. **Start Backend**:
   ```bash
   cd BarangayTech.Api
   dotnet run
   ```

2. **Start MAUI App**:
   - Set BarangayTech.Maui as startup project
   - Run (F5)

3. **Create First Admin** (Choose one option):

   **Option A - Via Swagger UI**:
   1. Open https://localhost:7241/swagger
   2. POST /api/auth/register
   3. Use this JSON:
   ```json
   {
     "email": "admin@barangaytech.local",
     "password": "Admin123!",
     "fullName": "System Administrator",
     "username": "admin",
     "role": "SuperAdmin"
   }
   ```

   **Option B - Via Firebase Console**:
   1. Authentication ? Users ? Add User
   2. Email: `admin@barangaytech.local`
   3. Password: `Admin123!`
   4. Go to Firestore ? users collection
   5. Create document with ID from auth user
   6. Set `Role: "SuperAdmin"`

## Quick Test

### Test Login:
1. Open MAUI app
2. Login with:
   - Email: `admin@barangaytech.local`
   - Password: `Admin123!`

### Expected Result:
- ? Redirected to Admin page
- ? Welcome message appears
- ? Can access admin features

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "Invalid API Key" | Check Firebase Web API Key in AuthService.cs |
| "Cannot connect" | Ensure backend is running on https://localhost:7241 |
| "User not found" | Create user document in Firestore after registration |
| "Unauthorized" | Check user role in Firestore is "Admin" or "SuperAdmin" |

## Android Emulator Note

If testing on Android emulator, change API URL:
```csharp
BaseAddress = new Uri("https://10.0.2.2:7241/")
```

## What's Different Now?

### ? Before:
- Hardcoded users
- Anyone could login with "admin/admin123"
- No security

### ? Now:
- Real Firebase auth
- Must register and verify email
- Secure tokens
- Admin can manage users

## Next Steps

1. ? Setup Firebase Auth (you just did!)
2. ? Create Registration Page in MAUI
3. ? Test full registration flow
4. ? Remove demo quick-login buttons
5. ? Deploy to production

---

**Ready to go! Need a Registration Page? Just ask! ??**
