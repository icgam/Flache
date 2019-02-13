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
using System.Collections.Generic;
using Flache.LocalInMemoryStorage.IntegrationTests.Helpers;
using Flache.Policy;
using Flache.Tests.Shared;
using NSubstitute;
using Xunit;

namespace Flache.LocalInMemoryStorage.IntegrationTests
{
    public class CacheInterceptorUsingMultiplePoliciesTests : IClassFixture<LocalInMemoryCacheFixture>
    {
        private LocalInMemoryCacheFixture Fixture { get; }

        public CacheInterceptorUsingMultiplePoliciesTests(LocalInMemoryCacheFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GivenCache_WhenDataCachedUsingDifferentPolicies_Then_CacheDataBasedOnPolicyRules()
        {
            // Arrange
            var services = Fixture.ConfigureServices((repository, container) =>
            {
                container.AddCachePolicy(
                    c => CacheStorageConfiguratorExtensions.UseLocalMemory(c, p => p.WithNoExpiration()), "policy1");

                container.AddCachePolicy(
                    c => c.UseLocalMemory(
                        p => p.WithAbsoluteExpiration(10)), "policy2");

                container.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<GetPeopleRequest, IEnumerable<Person>>((s, r) => s.GetPeople(r),
                        s => s.GenerateKeyUsing(r => $"{r.NameContains}_{r.MinAge}_{r.MaxAge}"))
                        .UsePolicy("policy1"));

                container.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<string, int, int, IEnumerable<Person>>((s, p1, p2, p3) => s.GetPeople(p1, p2, p3),
                        s => s.GenerateKeyUsing((criteria, minAge, maxAge) => $"{criteria}_{minAge}_{maxAge}"))
                        .UsePolicy("policy2"));
            });

            services.TimeProvider.Now.Returns(new DateTime(2018, 02, 28, 12, 00, 00));

            // Act
            var request = new GetPeopleRequest
            {
                NameContains = "Joa",
                MinAge = 20,
                MaxAge = 25
            };

            var result1 = services.Sut.GetPeople("Jane", 20, 25);
            var result2 = services.Sut.GetPeople(request);

            services.TimeProvider.Now.Returns(new DateTime(2018, 02, 28, 12, 00, 11));

            result1 = services.Sut.GetPeople("Jane", 20, 25);
            result2 = services.Sut.GetPeople(request);

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingParametersCallCount);
            Assert.Equal(1, services.Repository.GetPeopleUsingRequestCallCount);

            Assert.Collection(result1, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            });
            Assert.Collection(result2, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }
    }
}
