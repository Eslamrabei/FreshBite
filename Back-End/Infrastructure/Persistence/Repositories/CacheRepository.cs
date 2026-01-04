using StackExchange.Redis;

namespace Persistence.Repositories
{
    public class CacheRepository(IConnectionMultiplexer _connection) : ICacheRepository
    {
        private readonly IDatabase _database = _connection.GetDatabase();
        public async Task<string?> GetCacheAsync(string key)
        {
            var Value = await _database.StringGetAsync(key);
            return Value.IsNullOrEmpty ? default : Value;
        }

        public async Task SetCacheAsync(string key, object Value, TimeSpan TimeToLive)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var resultValue = JsonSerializer.Serialize(Value, options);
            await _database.StringSetAsync(key, resultValue, TimeToLive);
        }
    }
}
