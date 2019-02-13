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


﻿namespace Flache.Policy
{
    public sealed class CachePolicyException : CacheException
    {
        private CachePolicyException(string message) : base(message)
        {
        }

        public static CachePolicyException ForDuplicatePolicies(string policy, int duplicatePoliciesCount)
        {
            return new CachePolicyException(
                $"'{duplicatePoliciesCount}' caching polices have matched '{policy}' name! " +
                "Please make sure that policy names are UNIQUE when registering caching policies.");
        }

        public static CachePolicyException ForMissingPolicy(string policy)
        {
            return new CachePolicyException($"No caching polices have matched '{policy}' name! " +
                                            "Please make sure you register a caching policy with correct name before using it for CACHING.");
        }
    }
}