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
using System.Linq;
using Flache.Fluent;
using Flache.LocalInMemoryStorage.Fluent;
using Flache.LocalInMemoryStorage.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flache.LocalInMemoryStorage
{
    public sealed class InMemoryStorageFactory : ISetCacheExpiration, ICacheStorageFactory
    {
        private readonly CacheStorageConfigurator _configurator;
        private ICachingStrategy _strategy;

        public InMemoryStorageFactory(CacheStorageConfigurator configurator)
        {
            _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
        }

        public ITypedCacheStorage<TData> Build<TData>()
        {
            var logger = GetService<ILogger<InMemoryCacheStorage<TData>>>(_configurator.ServiceProvider);
            return new InMemoryCacheStorage<TData>(_strategy,
                logger);
        }

        public ICacheStorageFactory WithNoExpiration()
        {
            _strategy = new NoExpirationCachingStrategy();
            return this;
        }

        public ICacheStorageFactory WithAbsoluteExpiration(uint durationInSeconds)
        {
            var dateTimeProvider = GetService<ITimeProvider>(_configurator.ServiceProvider);
            var logger = GetService<ILogger<AbsoluteExpirationCachingStrategy>>(_configurator.ServiceProvider);
            _strategy = new AbsoluteExpirationCachingStrategy(dateTimeProvider,
                new CacheStrategyOptions(TimeSpan.FromSeconds(durationInSeconds)), logger);
            return this;
        }

        public ICacheStorageFactory WithSlidingExpiration(uint durationInSeconds)
        {
            var dateTimeProvider = GetService<ITimeProvider>(_configurator.ServiceProvider);
            var logger = GetService<ILogger<SlidingExpirationCachingStrategy>>(_configurator.ServiceProvider);
            _strategy = new SlidingExpirationCachingStrategy(dateTimeProvider,
                new CacheStrategyOptions(TimeSpan.FromSeconds(durationInSeconds)), logger);
            return this;
        }

        private TService GetService<TService>(IServiceProvider serviceProvider) where TService : class
        {
            var resolvers = serviceProvider.GetServices<TService>().ToList();
            if (resolvers.Count == 1)
                return resolvers.Single();

            throw new Exception(
                $"Failed to resolve '{typeof(TService).FullName}'. Please register the component implementation within IOC container");
        }
    }
}