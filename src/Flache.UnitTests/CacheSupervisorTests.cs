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


﻿using System.Threading.Tasks;
using Flache.Region;
using NSubstitute;
using Xunit;

namespace Flache.UnitTests
{
    public sealed class CacheSupervisorTests
    {
        public CacheSupervisorTests()
        {
            Sut = new CacheSupervisor();
        }

        public CacheSupervisor Sut { get; set; }

        [Fact]
        public async Task Given_CacheCleared_When_NoCacheStoragesRegistered_Then_Return()
        {
            // Arrange
            // Act
            // Assert
            await Sut.ClearCacheAsync();
        }

        [Fact]
        public async Task Given_CacheCleared_When_CacheStoragesAreAvailable_Then_ClearAllCacheStorages()
        {
            // Arrange
            var store1Mock = Substitute.For<ITypedCacheStorage<string>>();
            var store2Mock = Substitute.For<ITypedCacheStorage<string>>();

            // Act
            Sut.Register(store1Mock);
            Sut.Register(store2Mock);
            await Sut.ClearCacheAsync();

            // Assert
            await store1Mock.Received(1).ClearAsync();
            await store2Mock.Received(1).ClearAsync();
        }

        [Fact]
        public async Task Given_CacheClearedForGivenRegion_When_CacheStoragesAreAvailable_Then_ClearAllCacheStorages()
        {
            // Arrange
            var store1Mock = Substitute.For<ITypedCacheStorage<string>>();
            var store2Mock = Substitute.For<ITypedCacheStorage<string>>();

            // Act
            Sut.Register(store1Mock);
            Sut.Register(store2Mock);
            await Sut.ClearCacheAsync("region1");

            // Assert
            await store1Mock.Received(1).ClearAsync(Arg.Is<string>(r => r == "region1"));
            await store2Mock.Received(1).ClearAsync(Arg.Is<string>(r => r == "region1"));
        }
    }
}
