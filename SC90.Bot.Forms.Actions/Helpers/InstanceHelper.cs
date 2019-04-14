using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Reflection;

namespace SC90.Bot.Forms.Actions.Helpers
{
    internal static class InstanceHelper
    {
        private static readonly ConcurrentDictionary<string, Type> ModelTypes = new ConcurrentDictionary<string, Type>();

        internal static object CreateInstance(string modelType, params object[] parameters)
        {
            if (string.IsNullOrEmpty(modelType))
                return (object) null;
            Type type;
            if (!InstanceHelper.ModelTypes.TryGetValue(modelType, out type))
            {
                Type typeInfo = ReflectionUtil.GetTypeInfo(modelType);
                if (typeInfo == (Type) null)
                    return (object) null;
                InstanceHelper.ModelTypes.TryAdd(modelType, typeInfo);
                type = typeInfo;
            }
            return ReflectionUtil.CreateObject(type, parameters);
        }
    }
}
