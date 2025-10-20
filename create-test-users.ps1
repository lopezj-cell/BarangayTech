$apiUrl = "https://localhost:7241/api/auth/register"

Write-Host "?? BarangayTech User Creation Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if backend is running
Write-Host "Checking if backend is running..." -ForegroundColor Yellow
try {
    $testResponse = Invoke-WebRequest -Uri "https://localhost:7241/swagger/index.html" -Method Head -SkipCertificateCheck -TimeoutSec 5 -ErrorAction Stop
    Write-Host "? Backend is running!" -ForegroundColor Green
}
catch {
    Write-Host "? Backend is NOT running!" -ForegroundColor Red
    Write-Host "Please start the backend first:" -ForegroundColor Yellow
    Write-Host "  cd BarangayTech.Api" -ForegroundColor White
    Write-Host "  dotnet run" -ForegroundColor White
    exit 1
}

Write-Host ""

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

$successCount = 0
$failCount = 0

foreach ($user in $users) {
    Write-Host "Creating user: $($user.email) ($($user.role))..." -ForegroundColor Cyan
    
    $json = $user | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $json -ContentType "application/json" -SkipCertificateCheck -ErrorAction Stop
        Write-Host "  ? Success: $($user.fullName)" -ForegroundColor Green
        $successCount++
    }
    catch {
        $errorMessage = $_.Exception.Message
        if ($errorMessage -like "*already exists*") {
            Write-Host "  ? Already exists: $($user.email)" -ForegroundColor Yellow
        }
        else {
            Write-Host "  ? Failed: $errorMessage" -ForegroundColor Red
            $failCount++
        }
    }
    
    Start-Sleep -Milliseconds 500
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Yellow
Write-Host "  Created: $successCount users" -ForegroundColor Green
if ($failCount -gt 0) {
    Write-Host "  Failed: $failCount users" -ForegroundColor Red
}
Write-Host ""
Write-Host "?? Login Credentials:" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "SuperAdmin:" -ForegroundColor Magenta
Write-Host "  Email: admin@barangaytech.local" -ForegroundColor White
Write-Host "  Password: Admin123!" -ForegroundColor White
Write-Host ""
Write-Host "Admin:" -ForegroundColor Blue
Write-Host "  Email: secretary@barangaytech.local" -ForegroundColor White
Write-Host "  Password: Secretary123!" -ForegroundColor White
Write-Host ""
Write-Host "Resident:" -ForegroundColor Green
Write-Host "  Email: resident1@example.com" -ForegroundColor White
Write-Host "  Password: Resident123!" -ForegroundColor White
Write-Host ""
Write-Host "?? You can now login to the MAUI app!" -ForegroundColor Cyan
