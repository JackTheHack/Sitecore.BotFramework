using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SC90.Bot.Telegram.Abstractions;

namespace SC90.Bot.Telegram.Services
{
    public class MongoSessionProvider : ISessionProvider
    {
        private static readonly string _connectionString;
        private static readonly MongoClient _mongoClient;
        private readonly string _dbName;
        private Dictionary<string, BsonDocument> _sessionCache = new Dictionary<string, BsonDocument>();

        static MongoSessionProvider()
        {
            _connectionString = Sitecore.Configuration.Settings.GetSetting("BotFramework:mongoSessionDb");
            var clientSettings = MongoClientSettings.FromConnectionString(_connectionString);
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(5);
            _mongoClient = new MongoClient(clientSettings);
            var startMongoSessionTask = _mongoClient.StartSessionAsync();
            startMongoSessionTask.Wait(2000);
        }

        public MongoSessionProvider()
        {
            _dbName = Sitecore.Configuration.Settings.GetSetting("BotFramework:MongoSessionDbName", "BotSessions");
        }

        public async Task<BsonValue> Get(string sessionId, string fieldName)
        {
            if(_sessionCache.ContainsKey(sessionId))
            {
                return _sessionCache[sessionId].GetValue(fieldName);
            }

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

        public async Task Set(string sessionId, string fieldName, BsonValue value)
        {
            if(_sessionCache.ContainsKey(sessionId))
            {
                _sessionCache.Remove(sessionId);
            }

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

            await sessionCollection.UpdateOneAsync(filter, new BsonDocument(){{"$set", sessionDoc}}, new UpdateOptions()
            {
                IsUpsert = true
            });            
        }

        public async Task<BsonDocument> GetSessionDocument(string sessionId)
        {
            if(_sessionCache.ContainsKey(sessionId))
            {
                return _sessionCache[sessionId];
            }

            var db = _mongoClient.GetDatabase(_dbName);

            var sessionCollection = db?.GetCollection<BsonDocument>("Sessions", new MongoCollectionSettings()
            {
                AssignIdOnInsert = true
            });

            var filter = new BsonDocument();
            filter.Set("UserId", sessionId);

            var result = await sessionCollection.FindAsync(filter, new FindOptions<BsonDocument, BsonDocument>()
            {
                BatchSize = 1
            });

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

            _sessionCache.Add(sessionId, session);
        }

        public async Task Clear(string sessionId)
        {
            if(_sessionCache.ContainsKey(sessionId))
            {
                _sessionCache.Remove(sessionId);
            }

            var db = _mongoClient.GetDatabase(_dbName);

            var sessionCollection = db?.GetCollection<BsonDocument>("Sessions", new MongoCollectionSettings()
            {
                AssignIdOnInsert = true
            });


            var filter = new BsonDocument();
            filter.Set("UserId", sessionId);

            await sessionCollection.DeleteManyAsync(filter);
        }

        public async Task<bool> HasSessionDocument(string sessionId)
        {
            if(_sessionCache.ContainsKey(sessionId))
            {
                return true;
            }

            var db = _mongoClient.GetDatabase(_dbName);

            var sessionCollection = db?.GetCollection<BsonDocument>("Sessions", new MongoCollectionSettings()
            {
                AssignIdOnInsert = true
            });

            var filter = new BsonDocument();
            filter.Set("UserId", sessionId);

            var result = await sessionCollection.FindAsync(filter, new FindOptions<BsonDocument, BsonDocument>()
            {
                BatchSize = 1
            });

            var sessionFound = result.Any();

            return sessionFound;
        }
    }
}
