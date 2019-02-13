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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flache.Policy
{
    public sealed class CachePolicyBuilder
    {
        private readonly string _policyName;
        private readonly IServiceCollection _serviceCollection;

        public CachePolicyBuilder(IServiceCollection serviceCollection, string policyName)
        {
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _policyName = policyName;
        }
        
        public void ConfigurePolicy(Func<CacheStorageConfigurator, ICacheStorageFactory> cacheStorageFactory)
        {
            _serviceCollection.AddSingleton(sp =>
            {
                try
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var storage =
                        cacheStorageFactory(new CacheStorageConfigurator(sp, configuration));

                    return new CachePolicy(_policyName, storage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new Exception("Failed to create CACHE storage. Please investigate the exception for details.", e);
                }
            });
        }
    }
}