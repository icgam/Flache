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
using Flache.Region;
using Flache.Tests.Shared;

namespace Flache.CacheManager.IntegrationTests.Helpers
{
    public sealed class Services : ICacheServices
    {
        public IPeopleRepository Sut { get; }
        public InMemoryPeopleRepository Repository { get; }
        public ICacheSupervisor CacheSupervisor { get; }

        public Services(IPeopleRepository sut, InMemoryPeopleRepository repository, ICacheSupervisor cacheSupervisor)
        {
            Sut = sut ?? throw new ArgumentNullException(nameof(sut));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            CacheSupervisor = cacheSupervisor ?? throw new ArgumentNullException(nameof(cacheSupervisor));
        }
    }
}