using MongoDB.Driver;

namespace BarangayTech.Api.Services
{
    public class MongoService
    {
        public IMongoDatabase Database { get; }
        public MongoService(IConfiguration config)
        {
            var uri = Environment.GetEnvironmentVariable("MONGODB_URI")
                      ?? config.GetConnectionString("MongoDb")
                      ?? "";
            var mongoUrl = new MongoUrl(uri);
            var client = new MongoClient(mongoUrl);
            Database = client.GetDatabase("barangaytech");
        }
    }
}
