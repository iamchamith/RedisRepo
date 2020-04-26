using System;
namespace Redis.Models
{
    public class CacheKeyNotFound : Exception
    {
        public CacheKeyNotFound():base()
        {
        }
        public CacheKeyNotFound(string message) : base(message)
        {
        }
    }
}
