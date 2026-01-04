namespace Service.Implementations
{
    public class CacheService(ICacheRepository _cacheRepository) : ICacheService
    {
        public async Task<string?> GetCacheAsync(string key)
        => await _cacheRepository.GetCacheAsync(key);

        public async Task SetCacheAsync(string key, object Value, TimeSpan TimeToLive)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            await _cacheRepository.SetCacheAsync(key, Value, TimeToLive);
        }
    }
}
