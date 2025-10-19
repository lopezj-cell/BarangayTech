# Debug Guide - Troubleshooting Login Issues

## ? Debug Logging Now Enabled!

I've added comprehensive debug logging to help diagnose login issues.

## ?? How to View Debug Output

### In Visual Studio (MAUI App):

1. **Start the MAUI app in Debug mode** (F5)
2. **Open the Output Window**:
   - Menu: View ? Output (or Ctrl+Alt+O)
   - In the dropdown, select **"Debug"**

3. **Try to login** in the app
4. **Watch the Output window** - you'll see detailed logs like:
```
=== LOGIN ATTEMPT START ===
Input - Email/Username: admin@barangaytech.local
Formatted Email: admin@barangaytech.local
Password Length: 9 characters
Firebase URL: https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=...
Sending request to Firebase...
Firebase Response Status: BadRequest
Firebase Error Response: {"error":{"code":400,"message":"EMAIL_NOT_FOUND"}}
ERROR: User not found in Firebase Auth with email: admin@barangaytech.local
```

### In Visual Studio (Backend API):

1. **Start the backend API** (dotnet run or F5)
2. **Check the Console window** - you'll see logs like:
```
info: BarangayTech.Api.Controllers.AuthController[0]
      === REGISTRATION ATTEMPT ===
info: BarangayTech.Api.Controllers.AuthController[0]
      Email: admin@barangaytech.local
info: BarangayTech.Api.Controllers.AuthController[0]
      ? Firebase user created with UID: abc123...
```

## ?? What the Debug Output Tells You

### Common Error Messages and Solutions:

#### 1. "EMAIL_NOT_FOUND"
**Problem**: User doesn't exist in Firebase Authentication

**Solution**: Create the user first using one of these methods:
- Run the PowerShell script: `.\create-test-users.ps1`
- Use Swagger UI: POST to `/api/auth/register`
- Use Firebase Console: Authentication ? Add User

#### 2. "INVALID_PASSWORD"
**Problem**: Wrong password

**Solution**: 
- Check you're using the correct password
- Reset password in Firebase Console
- Re-create the user with known password

#### 3. "USER_DISABLED"
**Problem**: Account is disabled

**Solution**: 
- Go to Firebase Console ? Authentication ? Users
- Find the user and click "Enable account"

#### 4. "Failed to verify user credentials with backend"
**Problem**: Backend API is not running or can't reach it

**Solution**:
- Make sure backend is running: `cd BarangayTech.Api && dotnet run`
- Check backend URL in AuthService.cs (should be `https://localhost:7241/`)
- For Android emulator, use `https://10.0.2.2:7241/`

#### 5. "No user data received from backend"
**Problem**: User exists in Firebase Auth but not in Firestore

**Solution**:
- Go to Firebase Console ? Firestore
- Check if `users` collection has a document with the user's UID
- If not, create it manually or re-register the user

## ?? Step-by-Step Debugging Process

### Step 1: Check Firebase Authentication
1. Go to: https://console.firebase.google.com/project/barangaytech/authentication/users
2. Look for your test user (e.g., `admin@barangaytech.local`)
3. If NOT there ? User doesn't exist, create it!

### Step 2: Check Firestore Database
1. Go to: https://console.firebase.google.com/project/barangaytech/firestore/data
2. Look for `users` collection
3. Find document with ID matching the user's UID from Authentication
4. If NOT there ? User document missing, create it!

### Step 3: Try Login with Debug Output
1. **Start backend**: `dotnet run` in BarangayTech.Api
2. **Start MAUI app** in Debug mode (F5)
3. **Open Output window** (Ctrl+Alt+O) ? Select "Debug"
4. **Enter credentials** and click Login
5. **Read the debug output** to see where it fails

### Step 4: Share Debug Output
If still having issues, copy the debug output and share it!

## ?? Quick Test Script

Run this to create a test admin user:

```powershell
# Make sure backend is running first!
cd BarangayTech.Api
dotnet run

# In another terminal:
.\create-test-users.ps1
```

Then try logging in with:
- Email: `admin@barangaytech.local`
- Password: `Admin123!`

## ?? Clean Start (If All Else Fails)

1. **Delete all users from Firebase Console**:
   - Authentication ? Users ? Select All ? Delete

2. **Delete all Firestore data**:
   - Firestore ? users collection ? Delete collection

3. **Re-run the user creation script**:
   ```powershell
   .\create-test-users.ps1
   ```

4. **Try login again** and watch the debug output

## ?? Screenshot Your Debug Output

If you need help:
1. Try to login
2. Take a screenshot of the Output window
3. Share the screenshot showing the error messages

The debug output will show exactly which step is failing!
