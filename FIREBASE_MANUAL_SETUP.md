# Firebase Setup Instructions

## Step 1: Create/Select Firebase Project

1. Go to https://console.firebase.google.com/
2. Make sure you're logged in with your Firebase Google account (not oarbenairda@gmail.com)
3. Either:
   - **Create new project**: Click "Add project" ? Name it "BarangayTech" ? Continue
   - **Use existing project**: Select your existing BarangayTech project

4. Note down your **Project ID** (shown in project settings)

## Step 2: Enable Required Services

### Enable Firestore
1. In Firebase Console ? Build ? Firestore Database
2. Click "Create database"
3. Choose "Start in test mode"
4. Select location: **us-central** (or your preferred region)
5. Click "Enable"

### Enable Authentication
1. In Firebase Console ? Build ? Authentication
2. Click "Get started"
3. Go to "Sign-in method" tab
4. Click "Email/Password"
5. **Enable** both "Email/Password" and "Email link (passwordless sign-in)"
6. Click "Save"

## Step 3: Get Configuration Keys

### Get Web API Key
1. In Firebase Console ? Project Settings (?? icon)
2. Under "General" tab ? "Your apps" section
3. If no web app exists:
   - Click "</>" (Web app icon)
   - Register app with nickname "BarangayTech Web"
   - Copy the **apiKey** value
4. If web app exists, find **Web API Key** in the config

### Download Service Account Key
1. In Firebase Console ? Project Settings ? Service Accounts tab
2. Click "Generate new private key"
3. Click "Generate key" in the dialog
4. Save the downloaded JSON file as: `.firebase/barangaytech-firebase-key.json`

## Step 4: Update Your Code

### Update MAUI App (BarangayTech.Maui/Services/Auth/AuthService.cs)

Replace line 17:
```csharp
private const string FIREBASE_WEB_API_KEY = "YOUR_FIREBASE_WEB_API_KEY";
```

With:
```csharp
private const string FIREBASE_WEB_API_KEY = "AIzaSy..."; // Paste your actual key
```

### Update API appsettings.json

In `BarangayTech.Api/appsettings.json`, update:
```json
{
  "Firebase": {
    "ProjectId": "your-actual-project-id",  // e.g., "barangaytech-abc123"
    "CredentialPath": ".firebase/barangaytech-firebase-key.json"
  }
}
```

## Step 5: Initialize Firebase in Your Project

Run this command in your project root:
```bash
npx firebase-tools@latest init firestore
```

Follow prompts:
- Use existing project
- Select your project
- Use default Firestore rules
- Don't set up indexes

## Step 6: Create Your First Admin User

### Option A: Via Firebase Console
1. Go to Authentication ? Users
2. Click "Add user"
3. Email: `admin@barangaytech.local`
4. Password: `Admin123!`
5. Click "Add user"

6. Go to Firestore Database
7. Create collection: `users`
8. Add document with ID matching the UID from Authentication
9. Set fields:
```
Email: admin@barangaytech.local
FullName: System Administrator  
Role: SuperAdmin
Username: admin
IsActive: true
CreatedDate: (current timestamp)
EmailVerified: true
```

### Option B: Via API (After backend is running)
Use Swagger UI at https://localhost:7241/swagger

POST to `/api/auth/register`:
```json
{
  "email": "admin@barangaytech.local",
  "password": "Admin123!",
  "fullName": "System Administrator",
  "username": "admin",
  "role": "SuperAdmin"
}
```

## ? Verification Checklist

- [ ] Firebase project created
- [ ] Firestore database enabled
- [ ] Authentication (Email/Password) enabled
- [ ] Web API Key obtained
- [ ] Service account key downloaded to `.firebase/`
- [ ] MAUI app `FIREBASE_WEB_API_KEY` updated
- [ ] API `appsettings.json` updated with ProjectId
- [ ] firebase.json created in project root
- [ ] Admin user created
- [ ] Backend API runs without errors
- [ ] Can register new users
- [ ] Can login with admin account

## Next: After Setup

Once you complete these steps, tell me and I'll help you:
1. Create the Registration Page for MAUI
2. Set up Firestore security rules
3. Test the complete authentication flow
4. Deploy to production

---

**When you're done, share your Firebase Project ID with me and I'll help configure everything automatically!** ??
