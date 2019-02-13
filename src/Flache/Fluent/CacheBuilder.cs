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
using Flache.Options;
using Flache.Policy;
using Flache.Region;
using Microsoft.Extensions.DependencyInjection;

namespace Flache.Fluent
{
    public sealed class CacheBuilder<TResult> : IApplyCachePolicy, ISetCacheRegion,
        IBuildCache
    {
        private readonly IServiceCollection _serviceCollection;

        private readonly ICacheKeyGenerator _cacheKeyGenerator;
        private readonly string _configurationKey;

        private string _policy = Policy.Constants.DefaultPolicy;
        private string _region = Region.Constants.DefaultRegion;

        public CacheBuilder(IServiceCollection serviceCollection, ICacheKeyGenerator cacheKeyGenerator, string configurationKey)
        {
            if (string.IsNullOrWhiteSpace(configurationKey))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(configurationKey));
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _cacheKeyGenerator = cacheKeyGenerator ?? throw new ArgumentNullException(nameof(cacheKeyGenerator));
            _configurationKey = configurationKey;
        }

        public void Build()
        {
            _serviceCollection.AddSingleton<ICacheOptions<TResult>>(sp =>
            {
                try
                {
                    var regionRegister = sp.GetRequiredService<ICacheRegionRegister>();
                    var policyProvider = sp.GetRequiredService<ICachePolicyProvider>();
                    var policy = policyProvider.GetPolicy(_policy);

                    ITypedCacheStorage<TResult> storage = policy.BuildCacheStorage<TResult>();
                    regionRegister.Register(storage);

                    return new CacheOptions<TResult>(_configurationKey, storage, _cacheKeyGenerator, _region);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to create CACHE storage. Please investigate the exception for details.",
                        e);
                }
            });
        }

        public IBuildCache AssignToRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(regionName));

            _region = regionName;

            return this;
        }

        public ISetCacheRegion UsePolicy(string cachePolicyName)
        {
            if (string.IsNullOrWhiteSpace(cachePolicyName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(cachePolicyName));
            _policy = cachePolicyName;

            return this;
        }
    }
}