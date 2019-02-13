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


﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Flache.LocalInMemoryStorage.Strategies;
using Microsoft.Extensions.Logging;

namespace Flache.LocalInMemoryStorage
{
    public sealed class InMemoryCacheStorage<TData> : ITypedCacheStorage<TData>
    {
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, TData>> _cacheByRegion = new ConcurrentDictionary<string, ConcurrentDictionary<string, TData>>();
        private readonly ICachingStrategy _cacheStrategy;

        public InMemoryCacheStorage(ICachingStrategy cacheStrategy, ILogger logger)
        {
            _cacheStrategy = cacheStrategy ?? throw new ArgumentNullException(nameof(cacheStrategy));
            Log = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private ILogger Log { get; }
        
        public async Task<TData> GetOrAddAsync(string key, Func<string, Task<TData>> dataFactoryAsync, string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(region));

            var regionKey = GetRegionKey(region, key);

            LogCheckingCache(region, key);

            TData data;

            _cacheLock.EnterWriteLock();
            try
            {
                var regionStore =
                    _cacheByRegion.GetOrAdd(region, newRegionKey => new ConcurrentDictionary<string, TData>());

                if (_cacheStrategy.IsCacheValid(regionKey))
                {
                    _cacheStrategy.CacheHit(regionKey);
                }
                else
                {
                    regionStore.TryRemove(key, out var staleCachedItem);
                }
                
                data = regionStore.GetOrAdd(key, newKey =>
                {
                    LogCachingData(region, key);
                    var newData = dataFactoryAsync(newKey).Result;
                    _cacheStrategy.CacheSet(GetRegionKey(region, newKey));
                    return newData;
                });
            }
            finally 
            {
                _cacheLock.ExitWriteLock();
            }

            LogReturingDataFor(region, key);

            return await Task.FromResult(data);
        }

        private string GetRegionKey(string region, string key)
        {
            return $"{region}|{key}";
        }

        public Task SetAsync(string key, TData data, string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(region));

            LogCachingData(region, key);

            var regionKey = GetRegionKey(region, key);

            _cacheLock.EnterWriteLock();
            try
            {
                var regionStore =
                    _cacheByRegion.GetOrAdd(region, newRegionKey => new ConcurrentDictionary<string, TData>());

                regionStore.AddOrUpdate(key, newKey => data, (existingKey, oldValue) => data);
                _cacheStrategy.CacheSet(regionKey);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            LogDataCached(region, key);
            return Task.CompletedTask;
        }

        public Task ClearAsync(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(region));

            LogClearingCache(region);

            _cacheLock.EnterWriteLock();
            try
            {
                _cacheByRegion.TryRemove(region, out var regionStore);

                if (regionStore == null)
                    return Task.CompletedTask;
               
                foreach (var key in regionStore.Keys)
                {
                    var regionKey = GetRegionKey(region, key);
                    _cacheStrategy.InvalidateCache(regionKey);
                }

                regionStore.Clear();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            LogCacheCleared(region);
            return Task.CompletedTask;
        }
        public Task ClearAsync()
        {
            LogClearingCache();

            _cacheLock.EnterWriteLock();
            try
            {
                _cacheByRegion.Clear();
                _cacheStrategy.InvalidateCache();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            LogCacheCleared();
            return Task.CompletedTask;
        }

        #region Logs

        private void LogCheckingCache(string region, string key)
        {
            Log.LogDebug($"Checking cache for '{region}:{key}' ...");
        }

        private void LogReturingDataFor(string region, string key)
        {
            Log.LogDebug($"Returning data from cache for '{region}:{key}' ...");
        }

        private void LogCachingData(string region, string key)
        {
            Log.LogDebug($"Caching data for '{region}:{key}' ...");
        }

        private void LogDataCached(string region, string key)
        {
            Log.LogDebug($"Data '{region}:{key}' has been cached.");
        }

        private void LogClearingCache(string region)
        {
            Log.LogDebug($"Clearing cache for '{region}' region ...");
        }

        private void LogCacheCleared(string region)
        {
            Log.LogDebug($"Cache for '{region}' region cleared.");
        }
        private void LogClearingCache()
        {
            Log.LogDebug("Clearing ENTIRE cache ...");
        }

        private void LogCacheCleared()
        {
            Log.LogDebug("Cache cleared.");
        }

        #endregion
    }
}