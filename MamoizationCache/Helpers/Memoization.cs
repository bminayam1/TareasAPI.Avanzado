using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasAPI.Helpers
{
    public static class Memoization
    {
        private static readonly Dictionary<string, (object Value, DateTime Expiration)> _cache = new();
        private static readonly object _lock = new();

        public static TResult GetOrAdd<TResult>(string key, Func<TResult> factory, TimeSpan expiration)
        {
            if (_cache.TryGetValue(key, out var entry) && entry.Expiration > DateTime.Now && entry.Value is TResult result)
            {
                return result;
            }

            var newValue = factory();

            lock (_lock)
            {
                _cache[key] = (newValue!, DateTime.Now.Add(expiration));
            }
            return newValue;
        }

        public static async Task<TResult> GetOrAddAsync<TResult>(string key, Func<Task<TResult>> factory, TimeSpan expiration)
        {
            if (_cache.TryGetValue(key, out var entry) && entry.Expiration > DateTime.Now && entry.Value is TResult result)
            {
                return result;
            }

            TResult newValue;
            try
            {
                newValue = await factory();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Error en memoization: {ex.Message}");
                throw;
            }

            lock (_lock)
            {
                _cache[key] = (newValue!, DateTime.Now.Add(expiration));
            }
            return newValue;
        }

        public static void ClearCache(params string[] keys)
        {
            lock (_lock)
            {
                if (keys.Length == 0)
                {
                    _cache.Clear(); // Borra toda la caché si no se especifican claves
                }
                else
                {
                    foreach (var key in keys)
                    {
                        _cache.Remove(key);
                    }
                }
            }
        }

        public static void CleanupExpiredCache(Dictionary<string, (object Value, DateTime Expiration)> _cache)
        {
            lock (_lock)
            {
                _cache = _cache.Where(pair => pair.Value.Expiration > DateTime.Now)
                               .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }
    }
}
