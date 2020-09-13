using System.Threading.Tasks;
using MongoDB.Bson;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface ISessionProvider
    {
        Task<BsonValue> Get(string sessionId, string fieldName);
        Task Set(string sessionId, string fieldName, BsonValue value);
        Task<BsonDocument> GetSessionDocument(string sessionId);
        Task<bool> HasSessionDocument(string sessionId);
        Task UpdateSessionDocument(string sessionId, BsonDocument session);
        Task Clear(string sessionId);
    }
}
