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
using Microsoft.Extensions.DependencyInjection;

namespace Flache.Policy
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCachePolicy(this IServiceCollection services,
            Func<CacheStorageConfigurator, ICacheStorageFactory> cacheStorageFactory)
        {
            return services.AddCachePolicy(cacheStorageFactory, Constants.DefaultPolicy);
        }

        public static IServiceCollection AddCachePolicy(this IServiceCollection services,
            Func<CacheStorageConfigurator, ICacheStorageFactory> cacheStorageFactory, string policyName)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (cacheStorageFactory == null) throw new ArgumentNullException(nameof(cacheStorageFactory));

            var policyBuilder = new CachePolicyBuilder(services, policyName);
            policyBuilder.ConfigurePolicy(cacheStorageFactory);

            return services;
        }
    }
}