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
using Flache.Fluent;
using Flache.Policy;
using Flache.Region;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Flache.Tests.Shared
{
    public abstract class CacheFixture<TFixtureServices> where TFixtureServices : ICacheServices
    {
        protected CacheFixture()
        {
            People = new List<Person>
            {
                new Person
                {
                    Name = "John",
                    Age = 30
                },
                new Person
                {
                    Name = "Jane",
                    Age = 25
                },
                new Person
                {
                    Name = "Josh",
                    Age = 35
                },
                new Person
                {
                    Name = "Joan",
                    Age = 21
                }
            };
        }

        public IReadOnlyList<Person> People { get; }

        protected abstract Func<CacheStorageConfigurator, ICacheStorageFactory> GetCachePolicy();

        protected virtual IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }

        protected abstract TFixtureServices CreateServices(IPeopleRepository sut, InMemoryPeopleRepository repository,
            ICacheSupervisor supervisor, ServiceProvider provider);

        public TFixtureServices ConfigureServices(Func<string, int, int, string> keyGenerator)
        {
            return ConfigureCacheServices((repository, services) =>
                services.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<string, int, int, IEnumerable<Person>>((s, p1, p2, p3) => s.GetPeople(p1, p2, p3),
                        s => s.GenerateKeyUsing(keyGenerator))));
        }

        public TFixtureServices ConfigureServicesForAsync(Func<string, int, int, string> keyGenerator)
        {
            return ConfigureCacheServices((repository, services) =>
                services.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<string, int, int, IEnumerable<Person>>((s, p1, p2, p3) => s.GetPeopleAsync(p1, p2, p3),
                        s => s.GenerateKeyUsing(keyGenerator))));
        }

        public TFixtureServices ConfigureServices(Func<GetPeopleRequest, string> keyGenerator)
        {
            return ConfigureCacheServices((repository, services) =>
                services.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<GetPeopleRequest, IEnumerable<Person>>((s, r) => s.GetPeople(r),
                        s => s.GenerateKeyUsing(keyGenerator))));
        }

        public TFixtureServices ConfigureServices()
        {
            return ConfigureCacheServices((repository, services) =>
                services.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache(s => s.GetAllPeople())));
        }

        public TFixtureServices ConfigureServicesForAsync(Func<GetPeopleRequest, string> keyGenerator)
        {
            return ConfigureCacheServices((repository, services) =>
                services.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<GetPeopleRequest, IEnumerable<Person>>((s, r) => s.GetPeopleAsync(r),
                        s => s.GenerateKeyUsing(keyGenerator))));
        }


        public TFixtureServices ConfigureServicesForMultipleMethods(Func<GetPeopleRequest, string> keyGenerator)
        {
            return ConfigureCacheServices((repository, services) =>
                services.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<GetPeopleRequest, IEnumerable<Person>>((s, r) => s.GetPeople(r),
                        s => s.GenerateKeyUsing(keyGenerator))
                        .Cache(s => s.GetAllPeople())));
        }

        public TFixtureServices ConfigureServicesForMultipleMethodsAsync(Func<GetPeopleRequest, string> keyGenerator)
        {
            return ConfigureCacheServices((repository, services) =>
                services.AddTransientCache<IPeopleRepository, InMemoryPeopleRepository>(sp => repository,
                    c => c.Cache<GetPeopleRequest, IEnumerable<Person>>((s, r) => s.GetPeopleAsync(r),
                        s => s.GenerateKeyUsing(keyGenerator))
                        .Cache(s => s.GetAllPeopleAsync())));
        }


        public TFixtureServices ConfigureServices(Action<InMemoryPeopleRepository, IServiceCollection> cacheConfigurator)
        {
            return ConfigureCacheServices(cacheConfigurator);
        }

        private TFixtureServices ConfigureCacheServices(Action<InMemoryPeopleRepository, IServiceCollection> configureCache)
        {
            IServiceCollection services = new ServiceCollection();
            var repository = new InMemoryPeopleRepository(People);

            services.AddSingleton(sp => Substitute.For<IConfiguration>());
            services = ConfigureServices(services);
            services.UseFluentCache();

            services.AddCachePolicy(GetCachePolicy());
            configureCache(repository, services);

            var provider = services.BuildServiceProvider();
            var sut = provider.GetRequiredService<IPeopleRepository>();
            var supervisor = provider.GetRequiredService<ICacheSupervisor>();

            return CreateServices(sut, repository, supervisor, provider);
        }
    }
}