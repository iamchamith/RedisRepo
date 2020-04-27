using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisRepo
{
    public class RedisContext<T> where T : BaseEntity<int>
    {
        IConnectionMultiplexer _redis;
        IDatabase _cachedb;
        private string _table;
        public RedisContext(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public RedisContext<T> SetDatabase(string table, int database = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(table))
                    throw new RedisException("Table name cannot be null or empty."); 
                
                _table = table.Trim().ToLower();
                _cachedb = _redis.GetDatabase(database);
                return this;
            }
            catch (System.Exception e)
            {
                throw new RedisException(e);
            }
        }
        public async Task<List<T>> Refill(List<T> items)
        {
            try
            {
                ConnectionValidation();
                await _cachedb.KeyDeleteAsync(_table);
                var hs = items.ToHashEntity();
                await _cachedb.HashSetAsync(_table, hs);
                return items;
            }
            catch (System.Exception e)
            {
                throw new RedisException(e);
            }
        }

        public async Task<List<T>> RefillIfNot(List<T> items)
        {
            try
            {
                ConnectionValidation();
                if (!await _cachedb.KeyExistsAsync(_table))
                {
                    await Refill(items);
                }
                return items;
            }
            catch (System.Exception e)
            {
                throw new RedisException(e);
            }
        }

        public async Task AddOrUpdate(T item)
        {
            try
            {
                ConnectionValidation();
                await _cachedb.HashDeleteAsync(_table, item.Id);
                var hs = new HashEntry[] { new HashEntry(item.Id, item.ToJsonString()) };
                await _cachedb.HashSetAsync(_table, hs);
            }
            catch (System.Exception e)
            {
                throw new RedisException(e);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                ConnectionValidation();
                await _cachedb.HashDeleteAsync(_table, id);
            }
            catch (System.Exception e)
            {
                throw new RedisException(e);
            }
        }

        public async Task<(bool, T)> GetById(int id)
        {
            try
            {
                ConnectionValidation();
                var isKey = await _cachedb.KeyExistsAsync(_table);
                if (isKey)
                {
                    var item = await _cachedb.HashGetAsync(_table, id);
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
            catch (System.Exception e)
            {
                throw new RedisException(e);
            }
        }

        public async Task<(bool, List<T>)> Get()
        {
            try
            {
                ConnectionValidation();
                var isKey = await _cachedb.KeyExistsAsync(_table);
                if (isKey)
                {
                    var item = await _cachedb.HashGetAllAsync(_table);
                    return (true, item.ToObjectList<T>());
                }
                else
                {
                    return (false, new List<T>());
                }
            }
            catch (System.Exception e)
            {
                throw new RedisException(e);
            }
        }

        private void ConnectionValidation()
        {

            if (_cachedb == null)
            {
                throw new RedisException($"Database cannot be found. check {nameof(SetDatabase)} method.");
            }
        }
    }
}
