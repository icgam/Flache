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
using System.Threading.Tasks;
using CacheManager.Core;

namespace Flache.CacheManager
{
    public sealed class CacheManagerStorageAdapter<TData> : ITypedCacheStorage<TData>
    {
        private readonly ICacheManager<TData> _cache;

        public CacheManagerStorageAdapter(ICacheManager<TData> cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public Task<TData> GetOrAddAsync(string key, Func<string, Task<TData>> dataFactoryAsync, string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(region));

            var result = _cache.GetOrAdd(key, region,
                (cacheKey, r) => dataFactoryAsync(cacheKey).GetAwaiter().GetResult());
            return Task.FromResult(result);
        }

        public Task SetAsync(string key, TData data, string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(region));

            _cache.AddOrUpdate(key, region, data, oldData => data);
            return Task.CompletedTask;
        }

        public Task ClearAsync(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(region));

            _cache.ClearRegion(region);
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _cache.Clear();
            return Task.CompletedTask;
        }
    }
}