namespace TareasAPI.Helpers
{
    public static class Memoization
    {
        private static readonly Dictionary<string, object> _cache = new();

        public static TResult GetOrAdd<TResult>(string key, Func<TResult> valueFactory)
        {
            if (_cache.TryGetValue(key, out var result))
                return (TResult)result;

            var value = valueFactory();

            _cache[key] = value;
            return value;
        }

        public static async Task<TResult> GetOrAddAsync<TResult>(string key, Func<Task<TResult>> valueFactory)
        {
            if (_cache.TryGetValue(key, out var result))
                return (TResult)result;

            var value = await valueFactory();

            _cache[key] = value;
            return value;
        }

        public static void ClearCache() => _cache.Clear();
    }
}