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
using Flache.Options;
using Flache.Policy;
using Flache.Region;
using Foil;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Flache
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseFluentCache(this IServiceCollection services)
        {
            services.TryAddSingleton<ICachePolicyProvider, CachePolicyProvider>();
            services.TryAddSingleton<ICacheOptionsProvider, CacheOptionsProvider>();
            services.TryAddSingleton<ICacheOptionsKeyGenerator, MethodMetadataBasedCacheOptionsKeyGenerator>();
            services.TryAddSingleton<CacheSupervisor>();
            services.TryAddSingleton<ICacheSupervisor>(sp => sp.GetRequiredService<CacheSupervisor>());
            services.TryAddSingleton<ICacheRegionRegister>(sp => sp.GetRequiredService<CacheSupervisor>());

            return services;
        }

        public static IServiceCollection AddTransientCache<TService, TImplementation>(this IServiceCollection services,
            Func<IConfigureCache<TImplementation>, IBuildCache<TImplementation>> cacheConfigurator)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            ConfigureCache(services, cacheConfigurator);
            services.AddTransientWithInterception<TService, TImplementation>(m => m.InterceptBy<CacheInterceptor>());

            return services;
        }

        public static IServiceCollection AddTransientCache<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> implementationFactory,
            Func<IConfigureCache<TImplementation>, IBuildCache<TImplementation>> cacheConfigurator)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            ConfigureCache(services, cacheConfigurator);
            services.AddTransientWithInterception<TService, TImplementation>(implementationFactory,
                m => m.InterceptBy<CacheInterceptor>());

            return services;
        }

        public static IServiceCollection AddScopedCache<TService, TImplementation>(this IServiceCollection services,
            Func<IConfigureCache<TImplementation>, IBuildCache<TImplementation>> cacheConfigurator)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            ConfigureCache(services, cacheConfigurator);
            services.AddScopedWithInterception<TService, TImplementation>(m => m.InterceptBy<CacheInterceptor>());

            return services;
        }

        public static IServiceCollection AddScopedCache<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> implementationFactory,
            Func<IConfigureCache<TImplementation>, IBuildCache<TImplementation>> cacheConfigurator)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            ConfigureCache(services, cacheConfigurator);
            services.AddScopedWithInterception<TService, TImplementation>(implementationFactory,
                m => m.InterceptBy<CacheInterceptor>());

            return services;
        }

        public static IServiceCollection AddSingletonCache<TService, TImplementation>(this IServiceCollection services,
            Func<IConfigureCache<TImplementation>, IBuildCache<TImplementation>> cacheConfigurator)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            ConfigureCache(services, cacheConfigurator);
            services.AddSingletonWithInterception<TService, TImplementation>(m => m.InterceptBy<CacheInterceptor>());

            return services;
        }

        public static IServiceCollection AddSingletonCache<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> implementationFactory,
            Func<IConfigureCache<TImplementation>, IBuildCache<TImplementation>> cacheConfigurator)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            ConfigureCache(services, cacheConfigurator);
            services.AddSingletonWithInterception<TService, TImplementation>(implementationFactory,
                m => m.InterceptBy<CacheInterceptor>());

            return services;
        }

        private static void ConfigureCache<TImplementation>(IServiceCollection services,
            Func<IConfigureCache<TImplementation>, IBuildCache<TImplementation>> cacheConfigurator)
            where TImplementation : class
        {
            if (cacheConfigurator == null) throw new ArgumentNullException(nameof(cacheConfigurator));

            cacheConfigurator(
                    new CacheConfigurator<TImplementation>(services, new MethodMetadataBasedCacheOptionsKeyGenerator()))
                .Build();
        }
    }
}