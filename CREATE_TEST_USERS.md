# Create Test Users for BarangayTech

## Prerequisites
1. Firebase Authentication enabled (Email/Password)
2. Backend API running

## Users to Create

### SuperAdmin Account
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

### Admin Account
```json
{
  "email": "secretary@barangaytech.local",
  "password": "Secretary123!",
  "fullName": "Barangay Secretary",
  "username": "secretary",
  "contactNumber": "09123456780",
  "role": "Admin"
}
```

### Resident Account
```json
{
  "email": "resident1@example.com",
  "password": "Resident123!",
  "fullName": "Juan Dela Cruz",
  "username": "resident1",
  "contactNumber": "09123456789",
  "address": "123 Main Street, Barangay Sample",
  "role": "Resident"
}
```

## How to Create Users

### Using PowerShell Script

Save this as `create-users.ps1`:

```powershell
$apiUrl = "https://localhost:7241/api/auth/register"

$users = @(
    @{
        email = "admin@barangaytech.local"
        password = "Admin123!"
        fullName = "System Administrator"
        username = "admin"
        contactNumber = "09123456789"
        role = "SuperAdmin"
    },
    @{
        email = "secretary@barangaytech.local"
        password = "Secretary123!"
        fullName = "Barangay Secretary"
        username = "secretary"
        contactNumber = "09123456780"
        role = "Admin"
    },
    @{
        email = "resident1@example.com"
        password = "Resident123!"
        fullName = "Juan Dela Cruz"
        username = "resident1"
        contactNumber = "09123456789"
        address = "123 Main Street, Barangay Sample"
        role = "Resident"
    }
)

foreach ($user in $users) {
    Write-Host "Creating user: $($user.email)" -ForegroundColor Cyan
    
    $json = $user | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $json -ContentType "application/json" -SkipCertificateCheck
        Write-Host "? Created: $($user.email)" -ForegroundColor Green
    }
    catch {
        Write-Host "? Failed: $($user.email) - $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nDone! You can now login with:" -ForegroundColor Yellow
Write-Host "  Admin: admin@barangaytech.local / Admin123!" -ForegroundColor White
Write-Host "  Secretary: secretary@barangaytech.local / Secretary123!" -ForegroundColor White
Write-Host "  Resident: resident1@example.com / Resident123!" -ForegroundColor White
```

Run it:
```powershell
.\create-users.ps1
```

### Using cURL (Cross-platform)

```bash
# Create SuperAdmin
curl -k -X POST https://localhost:7241/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@barangaytech.local",
    "password": "Admin123!",
    "fullName": "System Administrator",
    "username": "admin",
    "role": "SuperAdmin"
  }'

# Create Admin
curl -k -X POST https://localhost:7241/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "secretary@barangaytech.local",
    "password": "Secretary123!",
    "fullName": "Barangay Secretary",
    "username": "secretary",
    "role": "Admin"
  }'

# Create Resident
curl -k -X POST https://localhost:7241/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "resident1@example.com",
    "password": "Resident123!",
    "fullName": "Juan Dela Cruz",
    "username": "resident1",
    "address": "123 Main Street",
    "role": "Resident"
  }'
```

## Login Credentials

After creating users, use these to login:

| Role | Email | Password |
|------|-------|----------|
| **SuperAdmin** | `admin@barangaytech.local` | `Admin123!` |
| **Admin** | `secretary@barangaytech.local` | `Secretary123!` |
| **Resident** | `resident1@example.com` | `Resident123!` |

## Troubleshooting

### "Email already exists"
- User is already created in Firebase
- Login with existing credentials
- Or use Firebase Console to reset password

### "Cannot connect to API"
- Make sure backend is running: `dotnet run` in BarangayTech.Api
- Check URL is correct (https://localhost:7241)
- For Android emulator use: https://10.0.2.2:7241

### "Invalid credentials"
- Ensure Firebase Authentication is enabled
- Check service account key exists in `.firebase/`
- Verify user exists in both Firebase Auth and Firestore
