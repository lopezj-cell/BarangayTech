# ? Firebase Configuration Complete!

## What's Been Automatically Configured

### ? 1. Firebase Web API Key Updated
**File**: `BarangayTech.Maui/Services/Auth/AuthService.cs`
```csharp
private const string FIREBASE_WEB_API_KEY = "AIzaSyDlAfxJmkoQYjEm7TMf1xMv2r0YtCHaqLk";
```

### ? 2. Project ID Configured
**File**: `BarangayTech.Api/appsettings.json`
```json
{
  "Firebase": {
    "ProjectId": "barangaytech",
    "CredentialPath": ".firebase/barangaytech-firebase-key.json"
  }
}
```

### ? 3. Firebase Project Files
- `firebase.json` - Firestore configuration
- `.firebaserc` - Project selection (barangaytech)
- `firestore.rules` - Security rules
- `firestore.indexes.json` - Database indexes

### ? 4. Build Successful
All code compiles without errors!

---

## ?? CRITICAL: One Manual Step Required

### Download Service Account Key

You **MUST** download the Firebase service account credentials file:

1. Go to [Firebase Console](https://console.firebase.google.com/project/barangaytech/settings/serviceaccounts/adminsdk)
2. Click **"Generate New Private Key"**
3. Click **"Generate Key"** in the confirmation dialog
4. **Save the downloaded JSON file** to:
   ```
   F:\Programming Projects 2\BarangayTech\.firebase\barangaytech-firebase-key.json
   ```

**Why?** This file allows your backend API to communicate with Firebase securely.

---

## ?? Next Steps (After Downloading Key)

### Step 1: Enable Services in Firebase Console

#### Enable Authentication
1. Go to https://console.firebase.google.com/project/barangaytech/authentication
2. Click **"Get Started"**
3. Click **"Email/Password"**
4. **Enable** "Email/Password"
5. Click **"Save"**

#### Verify Firestore is Enabled
1. Go to https://console.firebase.google.com/project/barangaytech/firestore
2. If not enabled, click **"Create database"**
3. Choose **"Start in test mode"**
4. Location: **us-central** (already configured)
5. Click **"Enable"**

### Step 2: Deploy Firestore Security Rules

Run this command in your project root:
```bash
npx firebase-tools@latest deploy --only firestore:rules
```

This will deploy the security rules that:
- Allow everyone to read announcements, events, services, officials
- Only admins can create/edit content
- Users can read their own profile
- Admins can manage all users

### Step 3: Create Your First Admin User

#### Option A: Via Swagger API (Recommended)
1. Start your backend:
   ```bash
   cd BarangayTech.Api
   dotnet run
   ```

2. Open Swagger UI: https://localhost:7241/swagger

3. POST to `/api/auth/register`:
   ```json
   {
     "email": "admin@barangaytech.local",
     "password": "Admin123!",
     "fullName": "System Administrator",
     "username": "admin",
     "contactNumber": "09123456789",
     "role": "SuperAdmin"
   }
   ```

#### Option B: Via Firebase Console
1. Go to https://console.firebase.google.com/project/barangaytech/authentication
2. Click **"Add User"**
3. Email: `admin@barangaytech.local`
4. Password: `Admin123!`
5. Click **"Add User"**

6. Copy the **User UID** from the list

7. Go to https://console.firebase.google.com/project/barangaytech/firestore
8. Create collection: **`users`**
9. Add document with ID = User UID
10. Add fields:
    - `Email`: "admin@barangaytech.local"
    - `FullName`: "System Administrator"
    - `Username`: "admin"
    - `Role`: "SuperAdmin"
    - `IsActive`: true
    - `EmailVerified`: true
    - `CreatedDate`: (current timestamp)

### Step 4: Test Login

1. **Start Backend API**:
   ```bash
   cd BarangayTech.Api
   dotnet run
   ```

2. **Run MAUI App**:
   - Set `BarangayTech.Maui` as startup project
   - Press F5

3. **Login**:
   - Email: `admin@barangaytech.local`
   - Password: `Admin123!`

---

## ?? Configuration Summary

| Setting | Value | Status |
|---------|-------|--------|
| **Firebase Project ID** | `barangaytech` | ? Configured |
| **Project Number** | `716522359221` | ? Auto-detected |
| **Web API Key** | `AIzaSyDI...CHaqLk` | ? Configured |
| **Service Account Key** | `.firebase/barangaytech-firebase-key.json` | ?? **Download Required** |
| **Firestore Database** | us-central | ? Configured |
| **Authentication** | Email/Password | ?? Enable in Console |
| **Security Rules** | firestore.rules | ?? Deploy Required |

---

## ?? Quick Commands Reference

### Deploy Firestore Rules
```bash
npx firebase-tools@latest deploy --only firestore:rules
```

### Run Backend API
```bash
cd BarangayTech.Api
dotnet run
```

### View Swagger API Docs
Open: https://localhost:7241/swagger

### Check Firestore Data
Open: https://console.firebase.google.com/project/barangaytech/firestore

### Check Authentication Users
Open: https://console.firebase.google.com/project/barangaytech/authentication

---

## ? Verification Checklist

- [x] Firebase Web API Key configured in MAUI app
- [x] Project ID configured in API
- [x] firebase.json created
- [x] Firestore security rules created
- [x] Build successful
- [ ] **Service account key downloaded** ??
- [ ] **Authentication enabled in console**
- [ ] **Firestore database enabled in console**
- [ ] **Security rules deployed**
- [ ] **Admin user created**
- [ ] **Successfully logged in**

---

## ?? Need Help?

If you encounter any issues:

1. **"Cannot connect to API"**
   - Make sure backend is running (`dotnet run`)
   - For Android emulator, use `https://10.0.2.2:7241/`

2. **"Invalid credentials"**
   - Service account key must be downloaded
   - Authentication must be enabled in Firebase Console

3. **"User not found"**
   - Create admin user first (see Step 3 above)
   - Verify user exists in Firestore `users` collection

---

**?? You're almost there! Just download the service account key and enable the services, and you'll be ready to go!**
