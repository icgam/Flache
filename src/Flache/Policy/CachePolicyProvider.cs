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
using Microsoft.Extensions.DependencyInjection;

namespace Flache.Policy
{
    public sealed class CachePolicyProvider : ICachePolicyProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public CachePolicyProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public CachePolicy GetPolicy(string policy)
        {
            var policies = _serviceProvider.GetServices<CachePolicy>();
            var matchingPolicies =
                policies.Where(p => p.Name.Equals(policy, StringComparison.OrdinalIgnoreCase)).ToList();

            if (matchingPolicies.Count > 1)
                throw CachePolicyException.ForDuplicatePolicies(policy, matchingPolicies.Count);

            if (matchingPolicies.Count == 0)
                throw CachePolicyException.ForMissingPolicy(policy);

            return matchingPolicies.Single();
        }
    }
}