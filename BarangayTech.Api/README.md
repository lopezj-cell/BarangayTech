# BarangayTech.Api

ASP.NET Core Web API for BarangayTech mobile app using MongoDB Atlas.

Environment
- Set MongoDB connection string via environment variable MONGODB_URI

Windows PowerShell example:
$env:MONGODB_URI = "mongodb+srv://<user>:<pass>@cluster0.k2gqfbm.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0"

Run
- dotnet build
- dotnet run

Default URLs
- HTTPS: https://localhost:7241
- HTTP:  http://localhost:5000 (port may vary; see console output)

Endpoints
- GET /api/announcements
- GET /api/events
- GET /api/services
- GET /api/officials
