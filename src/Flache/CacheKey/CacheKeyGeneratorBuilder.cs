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

namespace Flache.CacheKey
{
    public sealed class CacheKeyGeneratorBuilder : ISetCacheKeyGenerationStrategy
    {
        public ICacheKeyGenerator GenerateKeyUsing(Func<string> keyGenerator)
        {
            return new LambdaBasedCacheKeyGenerator(keyGenerator);
        }
    }

    public sealed class CacheKeyGeneratorBuilder<TParam1> : ISetCacheKeyGenerationStrategy<TParam1>
    {
        public ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, string> keyGenerator)
        {
            return new LambdaBasedCacheKeyGenerator(keyGenerator);
        }
    }

    public sealed class CacheKeyGeneratorBuilder<TParam1, TParam2> : ISetCacheKeyGenerationStrategy<TParam1, TParam2>
    {
        public ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, TParam2, string> keyGenerator)
        {
            return new LambdaBasedCacheKeyGenerator(keyGenerator);
        }
    }

    public sealed class CacheKeyGeneratorBuilder<TParam1, TParam2, TParam3> : ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3>
    {
        public ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, TParam2, TParam3, string> keyGenerator)
        {
            return new LambdaBasedCacheKeyGenerator(keyGenerator);
        }
    }

    public sealed class CacheKeyGeneratorBuilder<TParam1, TParam2, TParam3, TParam4> : ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3, TParam4>
    {
        public ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, TParam2, TParam3, TParam4, string> keyGenerator)
        {
            return new LambdaBasedCacheKeyGenerator(keyGenerator);
        }
    }
}
