using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace SC90.Bot.Telegram.Models
{
    public class BsonSessionDocument
    {
        [BsonId]
        public string UserId { get; set; }

        public string CurrentStateId { get; set; }


    }
}
