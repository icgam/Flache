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


﻿using System.Collections.Concurrent;

namespace Flache.LocalInMemoryStorage.Strategies
{
    public sealed class NoExpirationCachingStrategy : ICachingStrategy
    {
        private readonly ConcurrentDictionary<string, bool> _cachedKeys = new ConcurrentDictionary<string, bool>();

        public bool IsCacheValid(string key)
        {
            _cachedKeys.TryGetValue(key, out var value);
            return value;
        }

        public void CacheHit(string key)
        {
            // Do nothing
        }

        public void CacheSet(string key)
        {
            _cachedKeys.AddOrUpdate(key, newKey => true, (existingKey, oldValue) => true);
        }

        public void InvalidateCache(string key)
        {
            _cachedKeys.AddOrUpdate(key, newKey => false, (existingKey, oldValue) => false);
        }

        public void InvalidateCache()
        {
            _cachedKeys.Clear();
        }
    }
}