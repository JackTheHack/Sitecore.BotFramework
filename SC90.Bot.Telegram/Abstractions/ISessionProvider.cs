using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface ISessionProvider
    {
        Task<BsonValue> Get(string sessionId, string fieldName);
        void Set(string sessionId, string fieldName, BsonValue value);
        Task<BsonDocument> GetSessionDocument(string sessionId);
        Task UpdateSessionDocument(string sessionId, BsonDocument session);
    }
}
