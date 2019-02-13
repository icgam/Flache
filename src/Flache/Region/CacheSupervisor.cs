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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flache.Region
{
    public sealed class CacheSupervisor : ICacheSupervisor, ICacheRegionRegister
    {
        private readonly ReaderWriterLockSlim _cacheStoresLock = new ReaderWriterLockSlim();
        private readonly List<ICacheStorage> _cacheStores = new List<ICacheStorage>();

        public void Register(ICacheStorage cache)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));

            _cacheStoresLock.EnterWriteLock();
            try
            {
                _cacheStores.Add(cache);
            }
            finally
            {
                _cacheStoresLock.ExitWriteLock();
            }
        }

        public Task ClearCacheAsync(string region)
        {
            return InvalidateCacheInternal(c => c.ClearAsync(region));
        }

        public Task ClearCacheAsync()
        {
            return InvalidateCacheInternal(c => c.ClearAsync());
        }

        private Task InvalidateCacheInternal(Func<ICacheStorage, Task> clearCacheDelegate)
        {
            var clearTasks = new ConcurrentBag<Task>();

            _cacheStoresLock.EnterReadLock();
            try
            {
                foreach (var cacheStore in _cacheStores)
                {
                    clearTasks.Add(Task.Run(() => clearCacheDelegate(cacheStore)));
                }
            }
            finally
            {
                _cacheStoresLock.ExitReadLock();
            }

            return Task.WhenAll(clearTasks);
        }
    }
}