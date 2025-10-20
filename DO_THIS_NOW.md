# ?? IMMEDIATE ACTION REQUIRED

## Do These 3 Things Right Now:

### 1?? Download Service Account Key (5 minutes)
1. Click: https://console.firebase.google.com/project/barangaytech/settings/serviceaccounts/adminsdk
2. Click **"Generate New Private Key"**
3. Save file as: `.firebase\barangaytech-firebase-key.json`

### 2?? Enable Authentication (2 minutes)
1. Click: https://console.firebase.google.com/project/barangaytech/authentication
2. Click **"Get Started"**
3. Enable **"Email/Password"**
4. Save

### 3?? Verify Firestore Database (1 minute)
1. Click: https://console.firebase.google.com/project/barangaytech/firestore
2. Make sure database exists (us-central location)
3. If not, click "Create Database" ? Test Mode ? us-central ? Enable

---

## ? Then Run These Commands:

### Deploy Security Rules
```bash
npx firebase-tools@latest deploy --only firestore:rules
```

### Start Backend
```bash
cd BarangayTech.Api
dotnet run
```

### Create Admin User (in Swagger UI)
1. Open: https://localhost:7241/swagger
2. POST /api/auth/register:
```json
{
  "email": "admin@barangaytech.local",
  "password": "Admin123!",
  "fullName": "System Administrator",
  "username": "admin",
  "role": "SuperAdmin"
}
```

### Test Login (in MAUI App)
- Email: `admin@barangaytech.local`
- Password: `Admin123!`

---

## ?? Total Time: ~10 minutes

That's it! Everything else is already configured! ??
