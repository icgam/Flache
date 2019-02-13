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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Flache.LocalInMemoryStorage
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all required services for LocalInMemoryStorage provider
        /// </summary>
        /// <param name="services"></param>
        public static void AddLocalMemoryStorage(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.TryAddSingleton<ITimeProvider, SystemTimeProvider>();
        }
    }
}