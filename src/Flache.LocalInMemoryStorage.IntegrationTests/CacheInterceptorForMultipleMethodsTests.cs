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


using System.Threading.Tasks;
using Flache.LocalInMemoryStorage.IntegrationTests.Helpers;
using Flache.Tests.Shared;
using Xunit;

namespace Flache.LocalInMemoryStorage.IntegrationTests
{
    public class CacheInterceptorForMultipleMethodsTests : IClassFixture<LocalInMemoryCacheFixture>
    {
        private LocalInMemoryCacheFixture Fixture { get; }

        public CacheInterceptorForMultipleMethodsTests(LocalInMemoryCacheFixture fixture)
        {
            Fixture = fixture;
        }
        
        [Fact]
        public void GivenCacheIsEmpty_WhenPeopleRequested_ThenRetrievePeopleFromRepository()
        {
            // Arrange
            var sut = Fixture.ConfigureServicesForMultipleMethods(request => "key").Sut;

            // Act
            var resultForPerson = sut.GetPeople(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });
            var resultForAllPeople = sut.GetAllPeople();

            // Assert
            Assert.Collection(resultForPerson, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForAllPeople, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Josh", p.Name);
                Assert.Equal(35, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public void GivenCache_WhenPeopleRequestedTwiceUsingSameParameters_ThenRetrievePeopleFromRepositoryOnlyTheFirstTime()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForMultipleMethods(request => "key");

            // Act
            var resultForPeople = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });
            resultForPeople = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });

            var resultForAllPeople = services.Sut.GetAllPeople();
            resultForAllPeople = services.Sut.GetAllPeople();

            // Assert
            Assert.Equal(1, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Equal(1, services.Repository.GetAllPeopleCallCount);
            Assert.Collection(resultForPeople, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForAllPeople, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Josh", p.Name);
                Assert.Equal(35, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public void GivenCache_WhenPeopleRequestedTwiceUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequests()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForMultipleMethods(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var resultForPeople1 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            var resultForPeople2 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });
            var resultForAllPeople = services.Sut.GetAllPeople();

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Equal(1, services.Repository.GetAllPeopleCallCount);
            Assert.Collection(resultForPeople1, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForPeople2, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForAllPeople, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Josh", p.Name);
                Assert.Equal(35, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public void GivenCache_WhenPeopleRequestedUsingTwiceForEachCriteriaUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequestsOnlyOnce()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForMultipleMethods(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var result1 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            result1 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });

            var result2 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });
            result2 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Equal(0, services.Repository.GetAllPeopleCallCount);
            Assert.Collection(result1, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(result2, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }
        
        [Fact]
        public async Task GivenCacheIsEmpty_WhenPeopleRequested_ThenRetrievePeopleFromRepository_Async()
        {
            // Arrange
            var sut = Fixture.ConfigureServicesForMultipleMethodsAsync(request => "key").Sut;

            // Act
            var resultForPerson = await sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });
            var resultForAllPeople = await sut.GetAllPeopleAsync();

            // Assert
            Assert.Collection(resultForPerson, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForAllPeople, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Josh", p.Name);
                Assert.Equal(35, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public async Task GivenCache_WhenPeopleRequestedTwiceUsingSameParameters_ThenRetrievePeopleFromRepositoryOnlyTheFirstTime_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForMultipleMethodsAsync(request => "key");

            // Act
            var resultForPeople = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });
            resultForPeople = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });

            var resultForAllPeople = await services.Sut.GetAllPeopleAsync();
            resultForAllPeople = await services.Sut.GetAllPeopleAsync();

            // Assert
            Assert.Equal(1, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Equal(1, services.Repository.GetAllPeopleCallCount);
            Assert.Collection(resultForPeople, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForAllPeople, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Josh", p.Name);
                Assert.Equal(35, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public async Task GivenCache_WhenPeopleRequestedTwiceUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequests_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForMultipleMethodsAsync(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var resultForPeople1 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            var resultForPeople2 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });
            var resultForAllPeople = await services.Sut.GetAllPeopleAsync();

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Equal(1, services.Repository.GetAllPeopleCallCount);
            Assert.Collection(resultForPeople1, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForPeople2, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(resultForAllPeople, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Josh", p.Name);
                Assert.Equal(35, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public async Task GivenCache_WhenPeopleRequestedUsingTwiceForEachCriteriaUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequestsOnlyOnce_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForMultipleMethodsAsync(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var result1 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            result1 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });

            var result2 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });
            result2 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Equal(0, services.Repository.GetAllPeopleCallCount);
            Assert.Collection(result1, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });

            Assert.Collection(result2, p =>
            {
                Assert.Equal("John", p.Name);
                Assert.Equal(30, p.Age);
            }, p =>
            {
                Assert.Equal("Jane", p.Name);
                Assert.Equal(25, p.Age);
            }, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }
    }
}
