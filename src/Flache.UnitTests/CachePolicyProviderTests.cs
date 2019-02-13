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
using Flache.Policy;
using NSubstitute;
using Xunit;

namespace Flache.UnitTests
{
    public sealed class CachePolicyProviderTests
    {
        public CachePolicyProviderTests()
        {
            ServiceProviderMock = Substitute.For<IServiceProvider>();
            Sut = new CachePolicyProvider(ServiceProviderMock);
        }

        private CachePolicyProvider Sut { get; }

        private IServiceProvider ServiceProviderMock { get; }


        [Fact]
        public void Given_PolicyRequested_When_Available_Then_Return()
        {
            // Arrange
            var policy = new CachePolicy("Policy1", Substitute.For<ICacheStorageFactory>());

            ServiceProviderMock.GetService(Arg.Any<Type>()).Returns(new List<CachePolicy>
            {
                policy
            });

            // Act
            var result = Sut.GetPolicy("Policy1");

            // Assert
            Assert.Same(policy, result);
        }

        [Fact]
        public void Given_PolicyRequested_When_NotAvailable_Then_Throw()
        {
            // Arrange
            var policy2 = new CachePolicy("Policy2", Substitute.For<ICacheStorageFactory>());
            var policy3 = new CachePolicy("Policy3", Substitute.For<ICacheStorageFactory>());
            ServiceProviderMock.GetService(Arg.Any<Type>()).Returns(new List<CachePolicy>
            {
                policy2,
                policy3
            });

            // Act
            // Assert
            Assert.Throws<CachePolicyException>(() => Sut.GetPolicy("Policy1"));
        }

        [Fact]
        public void Given_PolicyRequested_When_MultiplePoliciesMatch_Then_Throw()
        {
            // Arrange
            var policy1 = new CachePolicy("Policy1", Substitute.For<ICacheStorageFactory>());
            var policy2 = new CachePolicy("Policy1", Substitute.For<ICacheStorageFactory>());
            var policy3 = new CachePolicy("Policy3", Substitute.For<ICacheStorageFactory>());
            ServiceProviderMock.GetService(Arg.Any<Type>()).Returns(new List<CachePolicy>
            {
                policy1,
                policy2,
                policy3
            });

            // Act
            // Assert
            Assert.Throws<CachePolicyException>(() => Sut.GetPolicy("Policy1"));
        }
    }
}
