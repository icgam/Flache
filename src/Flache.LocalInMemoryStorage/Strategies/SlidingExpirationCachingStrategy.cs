// This software is part of the Flache Library
// Copyright (C) 2018 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


using System;
using System.Collections.Concurrent;
using Flache.LocalInMemoryStorage.Fluent;
using Microsoft.Extensions.Logging;

namespace Flache.LocalInMemoryStorage.Strategies
{
    public sealed class SlidingExpirationCachingStrategy : ICachingStrategy
    {
        private readonly CacheStrategyOptions _options;
        private readonly ITimeProvider _timeProvider;
        private readonly ConcurrentDictionary<string, CacheEntry> _cachedKeys = new ConcurrentDictionary<string, CacheEntry>();

        public SlidingExpirationCachingStrategy(ITimeProvider timeProvider, CacheStrategyOptions options,
            ILogger log)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        private ILogger Log { get; }

        public bool IsCacheValid(string key)
        {
            var cacheSet = _cachedKeys.GetOrAdd(key, (missingEntry) => new CacheEntry(false, _timeProvider.Now));
            if (cacheSet.CacheSet == false)
            {
                LogCacheNotFound();
                return false;
            }

            if (_timeProvider.Now <= cacheSet.CacheSetAt.Add(_options.CacheExpiresAfter))
            {
                LogCacheValid(cacheSet.CacheSetAt);
                return true;
            }

            LogCacheExpired();
            return false;
        }

        public void CacheHit(string key)
        {
            var cacheSetAt = _timeProvider.Now;
            _cachedKeys.AddOrUpdate(key, newKey => new CacheEntry(true, cacheSetAt),
                (existingKey, entryToUpdate) => new CacheEntry(true, cacheSetAt));
            
            LogCacheExpirationExtended(cacheSetAt);
        }

        public void CacheSet(string key)
        {
            var cacheSetAt = _timeProvider.Now;
            _cachedKeys.AddOrUpdate(key, newKey => new CacheEntry(true, cacheSetAt),
                (existingKey, entryToUpdate) => new CacheEntry(true, cacheSetAt));

            LogCacheIsSet(cacheSetAt);
        }

        public void InvalidateCache(string key)
        {
            _cachedKeys.TryRemove(key, out _);
            LogCacheCleared(key);
        }

        public void InvalidateCache()
        {
            _cachedKeys.Clear();
            LogCacheCleared();
        }

        #region Logs

        private void LogCacheNotFound()
        {
            Log.LogDebug("No cache found!");
        }

        private void LogCacheValid(DateTime cacheSetAt)
        {
            Log.LogDebug(
                $"Cache is found and valid! Will expire in {cacheSetAt.Add(_options.CacheExpiresAfter).Subtract(_timeProvider.Now).TotalSeconds}s.");
        }

        private void LogCacheIsSet(DateTime cacheSetAt)
        {
            Log.LogDebug(
                $"Cache is set and will expire in {cacheSetAt.Add(_options.CacheExpiresAfter).Subtract(_timeProvider.Now).TotalSeconds}s.");
        }

        private void LogCacheExpired()
        {
            Log.LogDebug("Cache has been found, but expired!");
        }

        private void LogCacheExpirationExtended(DateTime cacheSetAt)
        {
            Log.LogDebug($"Cache timeout reset, cache will expire at {cacheSetAt.Add(_options.CacheExpiresAfter)}");
        }

        private void LogCacheCleared()
        {
            Log.LogDebug("Cache has been cleared for all keys!");
        }

        private void LogCacheCleared(string key)
        {
            Log.LogDebug($"Cache has been cleared for '{key}' key!");
        }

        #endregion
    }
}