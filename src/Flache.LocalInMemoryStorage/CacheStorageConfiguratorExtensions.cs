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
using Flache.LocalInMemoryStorage.Fluent;

namespace Flache.LocalInMemoryStorage
{
    public static class CacheStorageConfiguratorExtensions
    {
        /// <summary>
        /// Uses InMemory dictionary as storage for CACHING content
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="storageOptions"></param>
        /// <returns></returns>
        public static ICacheStorageFactory UseLocalMemory(this CacheStorageConfigurator configurator,
            Func<ISetCacheExpiration, ICacheStorageFactory> storageOptions)
        {
            return storageOptions(new InMemoryStorageFactory(configurator));
        }
    }
}