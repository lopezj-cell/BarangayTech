using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace BarangayTech.Api.Services
{
    public class FirebaseService
    {
        public FirestoreDb Firestore { get; }

        public FirebaseService(IConfiguration config)
        {
            // Initialize Firebase Admin SDK
            var credentialPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")
                              ?? config["Firebase:CredentialPath"];

            var projectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID")
                         ?? config["Firebase:ProjectId"]
                         ?? "barangaytech-default";

            // Initialize Firebase App if not already initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                if (!string.IsNullOrEmpty(credentialPath) && File.Exists(credentialPath))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(credentialPath)
                    });
                }
                else
                {
                    // Try using default credentials (useful for Cloud Run, App Engine, etc.)
                    try
                    {
                        FirebaseApp.Create(new AppOptions
                        {
                            Credential = GoogleCredential.GetApplicationDefault()
                        });
                    }
                    catch
                    {
                        // Development mode: initialize without credentials (will use emulator if available)
                        FirebaseApp.Create();
                    }
                }
            }

            // Initialize Firestore
            Firestore = FirestoreDb.Create(projectId);
        }
    }
}
