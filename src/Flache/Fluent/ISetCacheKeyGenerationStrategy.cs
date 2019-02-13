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

namespace Flache.Fluent
{
    public interface ISetCacheKeyGenerationStrategy
    {
        ICacheKeyGenerator GenerateKeyUsing(Func<string> keyGenerator);
    }

    public interface ISetCacheKeyGenerationStrategy<TParam1>
    {
        ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, string> keyGenerator);
    }

    public interface ISetCacheKeyGenerationStrategy<TParam1, TParam2>
    {
        ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, TParam2, string> keyGenerator);
    }

    public interface ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3>
    {
        ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, TParam2, TParam3, string> keyGenerator);
    }

    public interface ISetCacheKeyGenerationStrategy<TParam1, TParam2, TParam3, TParam4>
    {
        ICacheKeyGenerator GenerateKeyUsing(Func<TParam1, TParam2, TParam3, TParam4, string> keyGenerator);
    }
}