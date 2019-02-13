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


using Flache.LocalInMemoryStorage.Strategies;
using Xunit;

namespace Flache.LocalInMemoryStorage.UnitTests
{
    public class NoExpirationCachingStrategyTests
    {
        private readonly NoExpirationCachingStrategy _sut;
        private const string FirstKey = "key1";
        private const string SecondKey = "key2";

        public NoExpirationCachingStrategyTests()
        {
            _sut = new NoExpirationCachingStrategy();
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
        public void GivenCache_WhenCacheIsPresent_ThenReturnValidCache()
        {
            // Arrange
            _sut.CacheSet(FirstKey);

            // Act
            var result = _sut.IsCacheValid(FirstKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentForFirstKey_ThenReturnInvalidForSecondKey()
        {
            // Arrange
            _sut.CacheSet(FirstKey);

            // Act
            var result = _sut.IsCacheValid(SecondKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GivenCache_WhenCacheIsPresentForBothKeysAndInvalidatedForFirst_ThenReturnInvalidForFirstKeyAndValidForSecond()
        {
            // Arrange
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
