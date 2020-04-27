using System;
namespace RedisRepo
{
    public class RedisException : Exception
    {
        public RedisException():base()
        {
        }
        public RedisException(Exception e) : base(e.Message)
        {
        }
        public RedisException(string message) : base(message)
        {
        }
    }
}
