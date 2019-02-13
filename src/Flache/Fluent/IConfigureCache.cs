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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flache.Fluent
{
    public interface IConfigureCache<TService> where TService : class
    {
        IApplyCachePolicy<TService> Cache<TResult>(Expression<Func<TService, TResult>> methodToIntercept);

        IApplyCachePolicy<TService> Cache<TParam1, TResult>(
            Expression<Func<TService, TParam1, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1>, ICacheKeyGenerator> cacheKeyOptions);

        IApplyCachePolicy<TService> Cache<TParam1, TParam2, TResult>(
            Expression<Func<TService, TParam1, TParam2, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2>, ICacheKeyGenerator> cacheKeyOptions);

        IApplyCachePolicy<TService> Cache<TParam1, TParam2, TParam3, TResult>(
            Expression<Func<TService, TParam1, TParam2, TParam3, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3>, ICacheKeyGenerator> cacheKeyOptions);

        IApplyCachePolicy<TService> Cache<TParam1, TParam2, TParam3, TParam4, TResult>(
            Expression<Func<TService, TParam1, TParam2, TParam3, TParam4, TResult>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3, TParam4>, ICacheKeyGenerator>
                cacheKeyOptions);

        IApplyCachePolicy<TService> Cache<TResult>(Expression<Func<TService, Task<TResult>>> methodToIntercept);

        IApplyCachePolicy<TService> Cache<TParam1, TResult>(
            Expression<Func<TService, TParam1, Task<TResult>>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1>, ICacheKeyGenerator> cacheKeyOptions);

        IApplyCachePolicy<TService> Cache<TParam1, TParam2, TResult>(
            Expression<Func<TService, TParam1, TParam2, Task<TResult>>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2>, ICacheKeyGenerator> cacheKeyOptions);

        IApplyCachePolicy<TService> Cache<TParam1, TParam2, TParam3, TResult>(
            Expression<Func<TService, TParam1, TParam2, TParam3, Task<TResult>>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3>, ICacheKeyGenerator> cacheKeyOptions);

        IApplyCachePolicy<TService> Cache<TParam1, TParam2, TParam3, TParam4, TResult>(
            Expression<Func<TService, TParam1, TParam2, TParam3, TParam4, Task<TResult>>> methodToIntercept,
            Func<ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3, TParam4>, ICacheKeyGenerator>
                cacheKeyOptions);
    }
}