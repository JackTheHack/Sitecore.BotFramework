using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SC90.Bot.Telegram.Abstractions;

namespace SC90.Bot.Telegram.Services
{
    public class MongoSessionProvider : ISessionProvider
    {
        private readonly string _connectionString;
        private readonly MongoClient _mongoClient;
        private readonly string _dbName;

        public MongoSessionProvider()
        {
            _connectionString = Sitecore.Configuration.Settings.GetSetting("BotFramework:mongoSessionDb");
            _dbName = Sitecore.Configuration.Settings.GetSetting("BotFramework:MongoSessionDbName", "BotSessions");
            var clientSettings = MongoClientSettings.FromConnectionString(_connectionString);
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(5);
            _mongoClient = new MongoClient(clientSettings);
        }

        public async Task<BsonValue> Get(string sessionId, string fieldName)
        {
           
            
            var db = _mongoClient.GetDatabase(_dbName);
            var sessionCollection = db?.GetCollection<BsonDocument>("Sessions");

            var filter = new BsonDocument();
                filter.Set("UserId", sessionId);

            var userSessionSearchResult = await sessionCollection.FindAsync(filter);

            var document = userSessionSearchResult.Current.FirstOrDefault();
            if (document != null && document.TryGetValue(fieldName, out var value))
            {
                return value;
            }

            return null;

        }

        public async void Set(string sessionId, string fieldName, BsonValue value)
        {
            var db = _mongoClient.GetDatabase(_dbName);
            var sessionCollection = db?.GetCollection<BsonDocument>("Sessions", new MongoCollectionSettings()
            {
                AssignIdOnInsert = true
            });

            var filter = new BsonDocument();
            filter.Set("UserId", sessionId);

            var sessionDoc = new BsonDocument();
            sessionDoc.Set("UserId", sessionId);
            sessionDoc.Set("UpdatedAt", DateTime.UtcNow);
            sessionDoc.Set(fieldName, value);

            await sessionCollection.UpdateOneAsync(filter, sessionDoc, new UpdateOptions()
            {
                IsUpsert = true
            });
        }

        public async Task<BsonDocument> GetSessionDocument(string sessionId)
        {
            await _mongoClient.StartSessionAsync();

            var db = _mongoClient.GetDatabase(_dbName);

            var sessionCollection = db?.GetCollection<BsonDocument>("Sessions", new MongoCollectionSettings()
            {
                AssignIdOnInsert = true
            });

            var filter = new BsonDocument();
            filter.Set("UserId", sessionId);

            var result = await sessionCollection.FindAsync(filter);

            var sessionFound = result.FirstOrDefault();

            return sessionFound?.ToBsonDocument();
        }

        public async Task UpdateSessionDocument(string sessionId, BsonDocument session)
        {
            var db = _mongoClient.GetDatabase(_dbName);
            var sessionCollection = db?.GetCollection<BsonDocument>("Sessions", new MongoCollectionSettings()
            {
                AssignIdOnInsert = true
            });

            var filter = new BsonDocument();
            filter.Set("UserId", sessionId);

            session.Set("UserId", sessionId);


            await sessionCollection.UpdateOneAsync(filter, 
                new BsonDocument { {"$set", session }}, 
                new UpdateOptions(){ IsUpsert = true});
        }
    }
}
