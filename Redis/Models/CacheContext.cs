using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.Models
{
    public class CacheContext<T> where T : BaseEntity<int>
    {
        IDatabase _cache;
        private string _database;
        public CacheContext(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase(0);
        }
        public CacheContext<T> SetDatabase(string database)
        {
            _database = database.Trim().ToLower();
            return this;
        }
        public async Task<List<T>> Refill(List<T> items)
        {
            await _cache.KeyDeleteAsync(_database);
            var hs = items.ToHashEntity();
            await _cache.HashSetAsync(_database, hs);
            return items;
        }
        public async Task<List<T>> RefillIfNot(List<T> items)
        {
            if (!await _cache.KeyExistsAsync(_database))
            {
                await Refill(items);
            }
            return items;
        }
        public async Task AddOrUpdate(T item)
        {
            await _cache.HashDeleteAsync(_database, item.Id);
            var hs = new HashEntry[] { new HashEntry(item.Id, item.ToJsonString()) };
            await _cache.HashSetAsync(_database, hs);
        }

        public async Task Delete(int id)
        {
            await _cache.HashDeleteAsync(_database, id);
        }

        public async Task<(bool, T)> GetById(int id)
        {
            var isKey = await _cache.KeyExistsAsync(_database);
            if (isKey)
            {
                var item = await _cache.HashGetAsync(_database, id);
                if (item.HasValue)
                {
                    return (true, item.ToString().ToObject<T>());
                }
                return (true, default);
            }
            else
            {
                return (false, default);
            }
        }
        public async Task<(bool, List<T>)> Get()
        {
            var isKey = await _cache.KeyExistsAsync(_database);
            if (isKey)
            {
                var item = await _cache.HashGetAllAsync(_database);
                return (true, item.ToObjectList<T>());
            }
            else
            {
                return (false, new List<T>());
            }
        }
    }
}
