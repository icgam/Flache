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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Flache.CacheKey;
using Flache.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Flache.Fluent
{
    public sealed class CacheConfigurator<TService> : IConfigureCache<TService>, IApplyCachePolicy<TService>
        where TService : class
    {
        private readonly List<IBuildCache> _cacheBuilders = new List<IBuildCache>();
        private readonly ICacheOptionsKeyGenerator _cacheOptionsKeyGenerator;
        private readonly IServiceCollection _serviceCollection;
        private IApplyCachePolicy _currentCacheBuilder;

        public CacheConfigurator(IServiceCollection serviceCollection,
            ICacheOptionsKeyGenerator cacheOptionsKeyGenerator)
        {
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _cacheOptionsKeyGenerator = cacheOptionsKeyGenerator ??
                                        throw new ArgumentNullException(nameof(cacheOptionsKeyGenerator));
        }

        public void Build()
        {
            if (_currentCacheBuilder != null)
                _cacheBuilders.Add(_currentCacheBuilder);

            foreach (var cacheBuilder in _cacheBuilders) cacheBuilder.Build();
        }

        public IBuildCache<TService> AssignToRegion(string regionName)
        {
            _currentCacheBuilder.AssignToRegion(regionName);
            return this;
        }

        public ISetCacheRegion<TService> UsePolicy(string cachePolicyName)
        {
            _currentCacheBuilder.UsePolicy(cachePolicyName);
            return this;
        }

        public IApplyCachePolicy<TService> Cache<TResult>(Expression<Func<TService, TResult>> methodToIntercept)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var builder = new CacheKeyGeneratorBuilder();
            var cacheKeyGenerator = builder.GenerateKeyUsing(() => $"NO_ARGS_{info.Name}");

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TParam1, TResult>(
            Expression<Func<TService, TParam1, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1>, ICacheKeyGenerator> cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TParam1, TParam2, TResult>(
            Expression<Func<TService, TParam1, TParam2, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2>, ICacheKeyGenerator> cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1, TParam2>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TParam1, TParam2, TParam3, TResult>(
            Expression<Func<TService, TParam1, TParam2, TParam3, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3>, ICacheKeyGenerator> cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1, TParam2, TParam3>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TParam1, TParam2, TParam3, TParam4, TResult>(
            Expression<Func<TService, TParam1, TParam2, TParam3, TParam4, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3, TParam4>, ICacheKeyGenerator>
                cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1, TParam2, TParam3, TParam4>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TResult>(Expression<Func<TService, Task<TResult>>> methodToIntercept)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var builder = new CacheKeyGeneratorBuilder();
            var cacheKeyGenerator = builder.GenerateKeyUsing(() => info.Name);

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TParam1, TResult>(
            Expression<Func<TService, TParam1, Task<TResult>>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1>, ICacheKeyGenerator> cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TParam1, TParam2, TResult>(
            Expression<Func<TService, TParam1, TParam2, Task<TResult>>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2>, ICacheKeyGenerator> cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1, TParam2>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService> Cache<TParam1, TParam2, TParam3, TResult>(
            Expression<Func<TService, TParam1, TParam2, TParam3, Task<TResult>>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3>, ICacheKeyGenerator> cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1, TParam2, TParam3>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        public IApplyCachePolicy<TService>
            Cache<TParam1, TParam2, TParam3, TParam4, TResult>(
                Expression<Func<TService, TParam1, TParam2, TParam3, TParam4, Task<TResult>>> methodToIntercept,
                Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3, TParam4>, ICacheKeyGenerator>
                    cacheKeyOptions)
        {
            var info = GetMethodInfo(methodToIntercept);
            var configurationKey = _cacheOptionsKeyGenerator.GetOptionsKey(info);
            var cacheKeyGenerator = cacheKeyOptions(new CacheKeyGeneratorBuilder<TParam1, TParam2, TParam3, TParam4>());

            return CreateCacheBuilder(() =>
                new CacheBuilder<TResult>(_serviceCollection, cacheKeyGenerator, configurationKey));
        }

        private IApplyCachePolicy<TService> CreateCacheBuilder<TResult>(Func<CacheBuilder<TResult>> cacheBuilderFactory)
        {
            if (_currentCacheBuilder != null)
                _cacheBuilders.Add(_currentCacheBuilder);

            _currentCacheBuilder = cacheBuilderFactory();

            return this;
        }

        private MethodInfo GetMethodInfo(LambdaExpression e)
        {
            var methodCallExpression = e.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new NotSupportedException(
                    $"Unable to cast Expression Body to '{nameof(MethodCallExpression)}'. " +
                    $"Underlying type is '{e.Body.GetType().FullName}'! Unable to intercept method for CACHING! " +
                    "Please inspect the configuration.");

            return methodCallExpression.Method;
        }
    }
}