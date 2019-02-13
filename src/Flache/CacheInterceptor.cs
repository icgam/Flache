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
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Flache.Options;
using Foil;

namespace Flache
{
    public sealed class CacheInterceptor : AsyncInterceptor
    {
        private readonly ICacheOptionsProvider _cacheOptionsProvider;

        public CacheInterceptor(ICacheOptionsProvider cacheOptionsProvider)
        {
            _cacheOptionsProvider =
                cacheOptionsProvider ?? throw new ArgumentNullException(nameof(cacheOptionsProvider));
        }

        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            await proceed(invocation);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation,
            Func<IInvocation, Task<TResult>> proceed)
        {
            var options = _cacheOptionsProvider.GetOptions<TResult>(invocation.MethodInvocationTarget);
            var cacheKey = options.CachedItemKeyGenerator.GetKey(invocation.Arguments);
            var result = await options.Storage.GetOrAddAsync(cacheKey, async newKey => await proceed(invocation),
                options.Region);

            invocation.ReturnValue = result;

            return result;
        }
    }
}