using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ToDoListProj.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        public async Task SetCodeAsync(string key, string code)
        {
            await _database.StringSetAsync(key, code, TimeSpan.FromMinutes(10));
        }

        public async Task<string> GetCodeAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task<bool> IsCodeExpiredAsync(string key)
        {
            return !await _database.KeyExistsAsync(key);
        }

        public async Task RemoveCodeAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
