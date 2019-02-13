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
    public class CacheInterceptorTests : IClassFixture<LocalInMemoryCacheFixture>
    {
        private LocalInMemoryCacheFixture Fixture { get; }

        public CacheInterceptorTests(LocalInMemoryCacheFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GivenCacheIsEmpty_WhenPeopleRequestedMultipleParameters_ThenRetrievePeopleFromRepository()
        {
            // Arrange
            var sut = Fixture.ConfigureServices((criteria, minAge, maxAge) => "key").Sut;

            // Act
            var result = sut.GetPeople("Joa", 20, 25);

            // Assert
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public void GivenCache_WhenPeopleRequestedMultipleParametersTwiceUsingSameParameters_ThenRetrievePeopleFromRepositoryOnlyTheFirstTime()
        {
            // Arrange
            var services = Fixture.ConfigureServices((criteria, minAge, maxAge) => "key");

            // Act
            var result = services.Sut.GetPeople("Joa", 20, 25);
            result = services.Sut.GetPeople("Joa", 20, 25);

            // Assert
            Assert.Equal(1, services.Repository.GetPeopleUsingParametersCallCount);
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public void GivenCache_WhenPeopleRequestedMultipleParametersTwiceUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequests()
        {
            // Arrange
            var services = Fixture.ConfigureServices((criteria, minAge, maxAge) => $"{criteria}_{minAge}_{maxAge}");

            // Act
            var result1 = services.Sut.GetPeople("J", 20, 25);
            var result2 = services.Sut.GetPeople("J", 20, 30);

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingParametersCallCount);
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
        public void GivenCache_WhenPeopleRequestedUsingMultipleParametersTwiceForEachCriteriaUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequestsOnlyOnce()
        {
            // Arrange
            var services = Fixture.ConfigureServices((criteria, minAge, maxAge) => $"{criteria}_{minAge}_{maxAge}");

            // Act
            var result1 = services.Sut.GetPeople("J", 20, 25);
            result1 = services.Sut.GetPeople("J", 20, 25);

            var result2 = services.Sut.GetPeople("J", 20, 30);
            result2 = services.Sut.GetPeople("J", 20, 30);

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingParametersCallCount);
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
        public void GivenCacheIsEmpty_WhenPeopleRequestedSingleParameter_ThenRetrievePeopleFromRepository()
        {
            // Arrange
            var sut = Fixture.ConfigureServices(request => "key").Sut;

            // Act
            var result = sut.GetPeople(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });

            // Assert
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public void GivenCache_WhenPeopleRequestedSingleParameterTwiceUsingSameParameters_ThenRetrievePeopleFromRepositoryOnlyTheFirstTime()
        {
            // Arrange
            var services = Fixture.ConfigureServices(request => "key");

            // Act
            var result = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });
            result = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });

            // Assert
            Assert.Equal(1, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public void GivenCache_WhenPeopleRequestedSingleParameterTwiceUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequests()
        {
            // Arrange
            var services = Fixture.ConfigureServices(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var result1 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            var result2 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
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
        public void GivenCache_WhenPeopleRequestedUsingSingleParameterTwiceForEachCriteriaUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequestsOnlyOnce()
        {
            // Arrange
            var services = Fixture.ConfigureServices(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var result1 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            result1 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });

            var result2 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });
            result2 = services.Sut.GetPeople(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
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
        public async Task GivenCacheIsEmpty_WhenPeopleRequestedMultipleParameters_ThenRetrievePeopleFromRepository_Async()
        {
            // Arrange
            var sut = Fixture.ConfigureServicesForAsync((criteria, minAge, maxAge) => "key").Sut;

            // Act
            var result = await sut.GetPeopleAsync("Joa", 20, 25);

            // Assert
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public async Task GivenCache_WhenPeopleRequestedMultipleParametersTwiceUsingSameParameters_ThenRetrievePeopleFromRepositoryOnlyTheFirstTime_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForAsync((criteria, minAge, maxAge) => "key");

            // Act
            var result = await services.Sut.GetPeopleAsync("Joa", 20, 25);
            result = await services.Sut.GetPeopleAsync("Joa", 20, 25);

            // Assert
            Assert.Equal(1, services.Repository.GetPeopleUsingParametersCallCount);
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public async Task GivenCache_WhenPeopleRequestedMultipleParametersTwiceUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequests_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForAsync((criteria, minAge, maxAge) => $"{criteria}_{minAge}_{maxAge}");

            // Act
            var result1 = await services.Sut.GetPeopleAsync("J", 20, 25);
            var result2 = await services.Sut.GetPeopleAsync("J", 20, 30);

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingParametersCallCount);
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
        public async Task GivenCache_WhenPeopleRequestedUsingMultipleParametersTwiceForEachCriteriaUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequestsOnlyOnce_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForAsync((criteria, minAge, maxAge) => $"{criteria}_{minAge}_{maxAge}");

            // Act
            var result1 = await services.Sut.GetPeopleAsync("J", 20, 25);
            result1 = await services.Sut.GetPeopleAsync("J", 20, 25);

            var result2 = await services.Sut.GetPeopleAsync("J", 20, 30);
            result2 = await services.Sut.GetPeopleAsync("J", 20, 30);

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingParametersCallCount);
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
        public async Task GivenCacheIsEmpty_WhenPeopleRequestedSingleParameter_ThenRetrievePeopleFromRepository_Async()
        {
            // Arrange
            var sut = Fixture.ConfigureServicesForAsync(request => "key").Sut;

            // Act
            var result = await sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });

            // Assert
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public async Task GivenCache_WhenPeopleRequestedSingleParameterTwiceUsingSameParameters_ThenRetrievePeopleFromRepositoryOnlyTheFirstTime_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForAsync(request => "key");

            // Act
            var result = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });
            result = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "Joa", MinAge = 20, MaxAge = 25 });

            // Assert
            Assert.Equal(1, services.Repository.GetPeopleUsingRequestCallCount);
            Assert.Collection(result, p =>
            {
                Assert.Equal("Joan", p.Name);
                Assert.Equal(21, p.Age);
            });
        }

        [Fact]
        public async Task GivenCache_WhenPeopleRequestedSingleParameterTwiceUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequests_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForAsync(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var result1 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            var result2 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
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
        public async Task GivenCache_WhenPeopleRequestedUsingSingleParameterTwiceForEachCriteriaUsingDifferentParameters_ThenRetrievePeopleFromRepositoryForBothRequestsOnlyOnce_Async()
        {
            // Arrange
            var services = Fixture.ConfigureServicesForAsync(request => $"{request.NameContains}_{request.MinAge}_{request.MaxAge}");

            // Act
            var result1 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });
            result1 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 25 });

            var result2 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });
            result2 = await services.Sut.GetPeopleAsync(new GetPeopleRequest { NameContains = "J", MinAge = 20, MaxAge = 30 });

            // Assert
            Assert.Equal(2, services.Repository.GetPeopleUsingRequestCallCount);
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
        public void GivenCache_WhenPeopleRequestedTwice_ThenCacheResultFirstTimeAndReturnCachedSecondTime()
        {
            // Arrange
            var services = Fixture.ConfigureServices();

            // Act
            var result = services.Sut.GetAllPeople();
            result = services.Sut.GetAllPeople();
            
            // Assert
            Assert.Collection(result, p =>
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
    }
}
