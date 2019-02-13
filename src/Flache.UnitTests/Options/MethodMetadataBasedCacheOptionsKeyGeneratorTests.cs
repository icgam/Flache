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


﻿using Flache.Options;
using Flache.UnitTests.Helpers;
using Xunit;

namespace Flache.UnitTests.Options
{
    public sealed class MethodMetadataBasedCacheOptionsKeyGeneratorTests
    {
        [Fact]
        public void GivenMethod_WhenNoArgumentsSupplied_ThenKeyShouldContainTypeAndMethodName()
        {
            // Arrange
            var methodInfo = typeof(ServiceToCache).GetMethod(nameof(ServiceToCache.GetDataNoArgs));
            var sut = new MethodMetadataBasedCacheOptionsKeyGenerator();

            // Act
            var result = sut.GetOptionsKey(methodInfo);

            // Assert
            Assert.Equal("Flache.UnitTests.Helpers.ServiceToCache.GetDataNoArgs", result);
        }

        [Fact]
        public void GivenMethod_WhenSingleArgumentSupplied_ThenKeyShouldContainTypeAndMethodNameAndArgumentName()
        {
            // Arrange
            var methodInfo = typeof(ServiceToCache).GetMethod(nameof(ServiceToCache.GetDataSingleArg));
            var sut = new MethodMetadataBasedCacheOptionsKeyGenerator();

            // Act
            var result = sut.GetOptionsKey(methodInfo);

            // Assert
            Assert.Equal("Flache.UnitTests.Helpers.ServiceToCache.GetDataSingleArg:System.String", result);
        }

        [Fact]
        public void GivenMethod_WhenMultipleArgumentsSupplied_ThenKeyShouldContainTypeAndMethodNameAndArgumentNames()
        {
            // Arrange
            var methodInfo = typeof(ServiceToCache).GetMethod(nameof(ServiceToCache.GetDataThreeArgs));
            var sut = new MethodMetadataBasedCacheOptionsKeyGenerator();

            // Act
            var result = sut.GetOptionsKey(methodInfo);

            // Assert
            Assert.Equal("Flache.UnitTests.Helpers.ServiceToCache.GetDataThreeArgs:System.String|System.Int32|System.Int32", result);
        }

        [Fact]
        public void GivenMethod_WhenNestedGenericArgumentSupplied_ThenKeyShouldContainTypeAndMethodNameAndNestedGenericTypeNames()
        {
            // Arrange
            var methodInfo = typeof(ServiceToCache).GetMethod(nameof(ServiceToCache.GetDataSingleNestedGenericArg));
            var sut = new MethodMetadataBasedCacheOptionsKeyGenerator();

            // Act
            var result = sut.GetOptionsKey(methodInfo);

            // Assert
            Assert.StartsWith("Flache.UnitTests.Helpers.ServiceToCache.GetDataSingleNestedGenericArg:" +
                         "Flache.UnitTests.Helpers.Request`1[[System.Collections.Generic.List`1" +
                         "[[Flache.UnitTests.Helpers.Parameter", result);
        }
    }
}
