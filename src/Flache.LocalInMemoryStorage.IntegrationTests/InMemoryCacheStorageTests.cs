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
using Flache.LocalInMemoryStorage.Strategies;
using Flache.Region;
using Flache.Tests.Shared;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Flache.LocalInMemoryStorage.IntegrationTests
{
    public sealed class InMemoryCacheStorageTests
    {
        public InMemoryCacheStorageTests()
        {
            DataServiceMock = Substitute.For<IDataService>();
            Sut = new InMemoryCacheStorage<string>(new NoExpirationCachingStrategy(), Substitute.For<ILogger>());
        }

        private IDataService DataServiceMock { get; }

        private InMemoryCacheStorage<string> Sut { get; }

        private const string CacheKey = "key";

        [Fact]
        public async Task GivenCache_WhenDataRequestedAfterClearing_ThenRetrieveItemFromTheBackingStore()
        {
            // Arrange
            DataServiceMock.GetDataAsync(CacheKey).Returns("Data1");

            // Act
            var result = await Sut.GetOrAddAsync(CacheKey, async key => await DataServiceMock.GetDataAsync(key),
                Constants.DefaultRegion);

            await Sut.ClearAsync();

            result = await Sut.GetOrAddAsync(CacheKey, async key => await DataServiceMock.GetDataAsync(key),
                Constants.DefaultRegion);

            // Assert
            Assert.Equal("Data1", result);

            await DataServiceMock.Received(2).GetDataAsync(Arg.Is<string>(request => request == CacheKey));
        }

        [Fact]
        public async Task GivenCache_WhenDataRequestedAndDataFoundInCache_ThenRetrieveCachedItem()
        {
            // Arrange
            DataServiceMock.GetDataAsync(CacheKey).Returns("Data1");

            // Act
            var result = await Sut.GetOrAddAsync(CacheKey, async key => await DataServiceMock.GetDataAsync(key),
                Constants.DefaultRegion);
            result = await Sut.GetOrAddAsync(CacheKey, async key => await DataServiceMock.GetDataAsync(key),
                Constants.DefaultRegion);

            // Assert
            Assert.Equal("Data1", result);

            await DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == CacheKey));
        }

        [Fact]
        public async Task GivenCache_WhenDataSet_ThenWeShouldBeAbleToRetrieveCachedItem()
        {
            // Arrange
            DataServiceMock.GetDataAsync(CacheKey).Returns("Data1");

            // Act
            await Sut.SetAsync(CacheKey, "Data2", Constants.DefaultRegion);
            var result = await Sut.GetOrAddAsync(CacheKey, async key => await DataServiceMock.GetDataAsync(key),
                Constants.DefaultRegion);

            // Assert
            Assert.Equal("Data2", result);

            DataServiceMock.DidNotReceiveWithAnyArgs();
        }

        [Fact]
        public async Task GivenEmptyCache_WhenDataRequested_ThenCreateAndCacheItem()
        {
            // Arrange
            DataServiceMock.GetDataAsync(CacheKey).Returns("Data1");

            // Act
            var result = await Sut.GetOrAddAsync(CacheKey, async key => await DataServiceMock.GetDataAsync(key),
                Constants.DefaultRegion);

            // Assert
            Assert.Equal("Data1", result);

            await DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == CacheKey));
        }

        // clear all
        // clear region

        [Fact]
        public async Task GivenCacheWithItemsOverMultipleRegions_WhenRegionCleared_ThenFirstItemShouldBeRetrievedViaStore_And_SecondFromCache()
        {
            // Arrange
            DataServiceMock.GetDataAsync("key1").Returns("Data1");
            DataServiceMock.GetDataAsync("key2").Returns("Data2");

            // Act
            var result1 = await Sut.GetOrAddAsync("key1", async key => await DataServiceMock.GetDataAsync(key),
                "region1");

            var result2 = await Sut.GetOrAddAsync("key2", async key => await DataServiceMock.GetDataAsync(key),
                "region2");

            await Sut.ClearAsync("region1");

            result1 = await Sut.GetOrAddAsync("key1", async key => await DataServiceMock.GetDataAsync(key),
                "region1");

            result2 = await Sut.GetOrAddAsync("key2", async key => await DataServiceMock.GetDataAsync(key),
                "region2");

            // Assert
            Assert.Equal("Data1", result1);
            Assert.Equal("Data2", result2);

            Received.InOrder(() =>
            {
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key1"));
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key2"));
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key1"));
            });
        }

        [Fact]
        public async Task GivenCacheWithItemsOverMultipleRegions_WhenCacheCleared_ThenAllItemsShouldBeRetrievedViaStore()
        {
            // Arrange
            DataServiceMock.GetDataAsync("key1").Returns("Data1");
            DataServiceMock.GetDataAsync("key2").Returns("Data2");

            // Act
            var result1 = await Sut.GetOrAddAsync("key1", async key => await DataServiceMock.GetDataAsync(key),
                "region1");

            var result2 = await Sut.GetOrAddAsync("key2", async key => await DataServiceMock.GetDataAsync(key),
                "region2");

            await Sut.ClearAsync();

            result1 = await Sut.GetOrAddAsync("key1", async key => await DataServiceMock.GetDataAsync(key),
                "region1");

            result2 = await Sut.GetOrAddAsync("key2", async key => await DataServiceMock.GetDataAsync(key),
                "region2");

            // Assert
            Assert.Equal("Data1", result1);
            Assert.Equal("Data2", result2);

            Received.InOrder(() =>
            {
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key1"));
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key2"));
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key1"));
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key2"));
            });
        }

        [Fact]
        public async Task GivenCacheWithItemsOverMultipleRegions_WhenNonExistingRegionCleared_ThenCacheShouldntBeAffected()
        {
            // Arrange
            DataServiceMock.GetDataAsync("key1").Returns("Data1");
            DataServiceMock.GetDataAsync("key2").Returns("Data2");

            // Act
            var result1 = await Sut.GetOrAddAsync("key1", async key => await DataServiceMock.GetDataAsync(key),
                "region1");

            var result2 = await Sut.GetOrAddAsync("key2", async key => await DataServiceMock.GetDataAsync(key),
                "region2");

            await Sut.ClearAsync("non_existing_region");

            result1 = await Sut.GetOrAddAsync("key1", async key => await DataServiceMock.GetDataAsync(key),
                "region1");

            result2 = await Sut.GetOrAddAsync("key2", async key => await DataServiceMock.GetDataAsync(key),
                "region2");

            // Assert
            Assert.Equal("Data1", result1);
            Assert.Equal("Data2", result2);

            Received.InOrder(() =>
            {
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key1"));
                DataServiceMock.Received(1).GetDataAsync(Arg.Is<string>(request => request == "key2"));
            });
        }
    }
}