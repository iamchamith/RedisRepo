using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Redis.Models
{
    public static class Extensions
    {
        public static HashEntry[] ToHashEntity<T>(this List<T> items) where T : BaseEntity<int>
        {
            var hashList = new List<HashEntry>();
            if (items != null)
            {
                items.ForEach(item =>
                {
                    hashList.Add(new HashEntry(item.Id, item.ToJsonString()));
                });
            }
            return hashList.ToArray();
        }

        public static string ToJsonString<T>(this T item) where T : class
        {
            return JsonConvert.SerializeObject(item);
        }
        public static T ToObject<T>(this string item)
        {
            return JsonConvert.DeserializeObject<T>(item);
        }

        public static List<T> ToObjectList<T>(this HashEntry[] items)
        {
            var result = new List<T>();
            if (items != null)
            {
                items.ToList().ForEach(item =>
                {
                    result.Add(item.Value.ToString().ToObject<T>());
                });
            }
            return result;
        }
    }
}
