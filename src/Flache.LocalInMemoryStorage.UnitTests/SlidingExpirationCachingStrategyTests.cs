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


using System;
using Flache.LocalInMemoryStorage.Fluent;
using Flache.LocalInMemoryStorage.Strategies;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Flache.LocalInMemoryStorage.UnitTests
{
    public class SlidingExpirationCachingStrategyTests
    {
        public SlidingExpirationCachingStrategyTests()
        {
            _timeProviderMock = Substitute.For<ITimeProvider>();
            _sut = new SlidingExpirationCachingStrategy(_timeProviderMock,
                new CacheStrategyOptions(_cacheExpiresAfter), Substitute.For<ILogger>());
        }

        private readonly SlidingExpirationCachingStrategy _sut;
        private readonly ITimeProvider _timeProviderMock;
        private readonly TimeSpan _cacheExpiresAfter = TimeSpan.FromMinutes(10);
        private const string FirstKey = "key1";
        private const string SecondKey = "key2";

        [Fact]
        public void GivenCache_WhenCacheIsPresentAndHitBeforeExpiration_ThenResetCacheTimeout()
        {
            // Arrange
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 00, 00));
            _sut.CacheSet(FirstKey);
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 10, 00));
            _sut.CacheHit(FirstKey);
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 20, 00));

            // Act
            var result = _sut.IsCacheValid(FirstKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentAndHitBeforeExpiration_ThenResetCacheTimeoutButInvalidateAfter()
        {
            // Arrange
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 00, 00));
            _sut.CacheSet(FirstKey);
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 10, 00));
            _sut.CacheHit(FirstKey);
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 20, 01));

            // Act
            var result = _sut.IsCacheValid(FirstKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentAndNotExpired_ThenReturnValidCache()
        {
            // Arrange
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 00, 00));
            _sut.CacheSet(FirstKey);

            // Act
            var result = _sut.IsCacheValid(FirstKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentButExpired_ThenReturnInValidCache()
        {
            // Arrange
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 00, 00));
            _sut.CacheSet(FirstKey);
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 10, 01));

            // Act
            var result = _sut.IsCacheValid(FirstKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentForFirstKeyButExpiredForSecond_ThenReturnInValidCacheForSecondKeyButValidForFirst()
        {
            // Arrange
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 00, 00));
            _sut.CacheSet(SecondKey);
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 05, 00));
            _sut.CacheSet(FirstKey);
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 10, 01));

            // Act
            var resultForFirstKey = _sut.IsCacheValid(FirstKey);
            var resultForSecondKey = _sut.IsCacheValid(SecondKey);

            // Assert
            Assert.True(resultForFirstKey);
            Assert.False(resultForSecondKey);
        }

        [Theory]
        [InlineData(FirstKey)]
        [InlineData(SecondKey)]
        public void GivenEmptyCache_WhenCheckingCache_ThenReturnInvalidCache(string key)
        {
            // Arrange
            // Act
            var result = _sut.IsCacheValid(key);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentForBothKeysAndInvalidatedForFirst_ThenReturnInvalidForFirstKeyAndValidForSecond()
        {
            // Arrange
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 00, 00));
            _sut.CacheSet(FirstKey);
            _sut.CacheSet(SecondKey);

            // Act
            _sut.InvalidateCache(FirstKey);

            var resultForFirstKey = _sut.IsCacheValid(FirstKey);
            var resultForSecondKey = _sut.IsCacheValid(SecondKey);

            // Assert
            Assert.False(resultForFirstKey);
            Assert.True(resultForSecondKey);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentForBothKeysAndInvalidated_ThenReturnInvalidForBothKeys()
        {
            // Arrange
            _timeProviderMock.Now.Returns(new DateTime(2017, 01, 01, 12, 00, 00));
            _sut.CacheSet(FirstKey);
            _sut.CacheSet(SecondKey);

            // Act
            _sut.InvalidateCache();

            var resultForFirstKey = _sut.IsCacheValid(FirstKey);
            var resultForSecondKey = _sut.IsCacheValid(SecondKey);

            // Assert
            Assert.False(resultForFirstKey);
            Assert.False(resultForSecondKey);
        }
    }
}