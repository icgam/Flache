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
using Flache.Fluent;

namespace Flache.Options
{
    public sealed class CacheOptions<TData> : ICacheOptions<TData>
    {
        public CacheOptions(string methodToInterceptKey, ITypedCacheStorage<TData> storage,
            ICacheKeyGenerator cachedItemKeyGenerator, string region)
        {
            if (string.IsNullOrWhiteSpace(methodToInterceptKey))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(methodToInterceptKey));
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(region));

            MethodToInterceptKey = methodToInterceptKey;
            CachedItemKeyGenerator =
                cachedItemKeyGenerator ?? throw new ArgumentNullException(nameof(cachedItemKeyGenerator));
            Region = region;
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public string MethodToInterceptKey { get; }
        public ITypedCacheStorage<TData> Storage { get; }
        public ICacheKeyGenerator CachedItemKeyGenerator { get; }
        public string Region { get; }
    }
}