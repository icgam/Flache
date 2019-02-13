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
using System.Collections.Generic;
using System.Reflection;
using Flache.Options;
using NSubstitute;
using Xunit;

namespace Flache.UnitTests.Options
{
    public sealed class CacheOptionsProviderTests
    {
        public CacheOptionsProviderTests()
        {
            KeyGenerator = Substitute.For<ICacheOptionsKeyGenerator>();
            ServiceProvider = Substitute.For<IServiceProvider>();
            Sut = new CacheOptionsProvider(KeyGenerator, ServiceProvider);
            MethodInfo = Substitute.For<MethodInfo>();
        }

        private ICacheOptionsKeyGenerator KeyGenerator { get; }

        private IServiceProvider ServiceProvider { get; }

        private CacheOptionsProvider Sut { get; }
        private MethodInfo MethodInfo { get; }

        [Fact]
        public void GivenMultipleOptionsConfigured_WhenRequested_ThenReturnMatchingOptions()
        {
            // Arrange
            var matchingOptions = Substitute.For<ICacheOptions<string>>();
            var nonmatchingOptions = Substitute.For<ICacheOptions<string>>();
            ServiceProvider.GetService(typeof(IEnumerable<ICacheOptions<string>>)).Returns(new List<ICacheOptions<string>>
            {
                matchingOptions,
                nonmatchingOptions
            });

            matchingOptions.MethodToInterceptKey.Returns("key1");
            nonmatchingOptions.MethodToInterceptKey.Returns("key2");

            KeyGenerator.GetOptionsKey(Arg.Is<MethodInfo>(mi => mi.Equals(MethodInfo))).Returns("key1");

            // Act
            var result = Sut.GetOptions<string>(MethodInfo);

            // Assert
            Assert.Same(matchingOptions, result);
        }

        [Fact]
        public void GivenMultipleOptionsConfigured_WhenRequestedAndMultipleMatchingOptionsFound_ThenThrow()
        {
            // Arrange
            var firstMatchingOptions = Substitute.For<ICacheOptions<string>>();
            var secondMatchingOptions = Substitute.For<ICacheOptions<string>>();
            ServiceProvider.GetService(typeof(IEnumerable<ICacheOptions<string>>)).Returns(new List<ICacheOptions<string>>
            {
                firstMatchingOptions,
                secondMatchingOptions
            });

            firstMatchingOptions.MethodToInterceptKey.Returns("key1");
            secondMatchingOptions.MethodToInterceptKey.Returns("key1");

            KeyGenerator.GetOptionsKey(Arg.Is<MethodInfo>(mi => mi.Equals(MethodInfo))).Returns("key1");

            // Act
            // Assert
            Assert.Throws<CacheOptionsException>(() => Sut.GetOptions<string>(MethodInfo));
        }

        [Fact]
        public void GivenMultipleOptionsConfigured_WhenRequestedAndNoMatchingOptionsFound_ThenThrow()
        {
            // Arrange
            var firstNonmatchingOptions = Substitute.For<ICacheOptions<string>>();
            var secondNonmatchingOptions = Substitute.For<ICacheOptions<string>>();
            ServiceProvider.GetService(typeof(IEnumerable<ICacheOptions<string>>)).Returns(new List<ICacheOptions<string>>
            {
                firstNonmatchingOptions,
                secondNonmatchingOptions
            });

            firstNonmatchingOptions.MethodToInterceptKey.Returns("key1");
            secondNonmatchingOptions.MethodToInterceptKey.Returns("key2");

            KeyGenerator.GetOptionsKey(Arg.Is<MethodInfo>(mi => mi.Equals(MethodInfo))).Returns("key3");

            // Act
            // Assert
            Assert.Throws<CacheOptionsException>(() => Sut.GetOptions<string>(MethodInfo));
        }

        [Fact]
        public void GivenSingleOptionConfigured_WhenRequested_ThenReturnMatchingOptions()
        {
            // Arrange
            var options = Substitute.For<ICacheOptions<string>>();
            ServiceProvider.GetService(typeof(IEnumerable<ICacheOptions<string>>)).Returns(new List<ICacheOptions<string>>
            {
                options
            });
            options.MethodToInterceptKey.Returns("key");
            KeyGenerator.GetOptionsKey(Arg.Is<MethodInfo>(mi => mi.Equals(MethodInfo))).Returns("key");

            // Act
            var result = Sut.GetOptions<string>(MethodInfo);

            // Assert
            Assert.Same(options, result);
        }
    }
}