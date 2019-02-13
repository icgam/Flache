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
using Flache.CacheKey;
using Xunit;

namespace Flache.UnitTests
{
    public sealed class LambdaBasedCacheKeyGeneratorTests
    {
        [Theory]
        [InlineData(1, "value", "value_2")]
        [InlineData(2, "value", "value_4")]
        [InlineData(5, "value", "value_10")]
        public void GivenDelegateThatReturnsValueBasedOnMultipleArguments_WhenKeyRequested_ThenReturnCorrectCombination(
            int paramDigit, string paramValue, string expectedValue)
        {
            // Arrange
            Func<int, string, string> calculatedKey = (digit, value) => $"{value}_{digit * 2}";
            var sut = new LambdaBasedCacheKeyGenerator(calculatedKey);

            // Act
            var result = sut.GetKey(paramDigit, paramValue);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GivenDelegateThatReturnsStaticValue_WhenKeyRequested_ThenReturnStaticValue()
        {
            // Arrange
            Func<string> staticKey = () => "key";
            var sut = new LambdaBasedCacheKeyGenerator(staticKey);

            // Act
            var result = sut.GetKey();

            // Assert
            Assert.Equal("key", result);
        }
    }
}