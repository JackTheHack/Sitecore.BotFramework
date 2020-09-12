using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace SC90.Bot.Telegram.Models
{
    public class SchedulingJobData
    {
        public string EventName { get; set; }
        public string JobId { get; set; }
        public string SessionId { get; set; }
        public bool IsRecurrent { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public Guid BotId { get; set; }
        public string UserId { get; internal set; }
        public string Source { get; internal set; }
        public bool IsGlobal { get; internal set; }

        public JobDataMap ToJobData()
        {
            var obj = new Dictionary<string, object>();
            obj.Add("EventName", EventName);
            obj.Add("UserId", UserId);
            obj.Add("Source", Source);
            obj.Add("JobId", JobId);
            obj.Add("SessionId", SessionId); 
            obj.Add("IsRecurrent", IsRecurrent);
            obj.Add("BotId", BotId);
            return new JobDataMap((IDictionary<string,object>)obj);
        }

        public SchedulingJobData()
        {

        }

        public SchedulingJobData(JobDataMap properties)
        {
            Parameters = new Dictionary<string, string>();
            if (properties == null) return;
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "EventName":
                        EventName = property.Value?.ToString();
                        break;
                    case "JobId":
                        JobId = property.Value?.ToString();
                        break;
                    case "SessionId":
                        SessionId = property.Value?.ToString();
                        break;
                    case "UserId":
                        UserId = property.Value?.ToString();
                        break;
                    case "Source":
                        Source = property.Value?.ToString();
                        break;
                    case "IsRecurrent":
                        IsRecurrent = property.Value != null ? (bool)property.Value : false;
                        break;
                    case "BotId":
                        BotId = property.Value != null ? (Guid)property.Value : Guid.Empty;
                        break;
                    default:
                        Parameters.Add(property.Key, property.Value?.ToString());
                        break;
                }
            }
        }
    }
}
