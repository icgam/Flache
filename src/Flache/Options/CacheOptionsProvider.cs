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
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Flache.Options
{
    public sealed class CacheOptionsProvider : ICacheOptionsProvider
    {
        private readonly IServiceProvider _componentResolver;
        private readonly ICacheOptionsKeyGenerator _optionsKeyGenerator;

        public CacheOptionsProvider(ICacheOptionsKeyGenerator optionsKeyGenerator, IServiceProvider serviceProvider)
        {
            _optionsKeyGenerator = optionsKeyGenerator ?? throw new ArgumentNullException(nameof(optionsKeyGenerator));
            _componentResolver = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ICacheOptions<TData> GetOptions<TData>(MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));

            var key = _optionsKeyGenerator.GetOptionsKey(methodInfo);
            var options = _componentResolver.GetServices<ICacheOptions<TData>>().Where(o => o.MethodToInterceptKey.Equals(key))
                .ToList();

            if (options.Count > 1)
                throw CacheOptionsException.ForDuplicateMatches(key, options.Count);

            if (options.Count == 0)
                throw CacheOptionsException.ForNoMatches(key);

            return options.Single();
        }
    }
}