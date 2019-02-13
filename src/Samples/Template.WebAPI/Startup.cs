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
using CacheManager.Core;
using Flache;
using Flache.CacheManager;
using Flache.LocalInMemoryStorage;
using Flache.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Template.WebAPI.Services;

namespace Template.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"}); });
            services.UseFluentCache();

            services.AddLocalMemoryStorage();
            services.AddCachePolicy(
                c => c.UseLocalMemory(p => p.WithNoExpiration()));

            services.AddCachePolicy(
                c => c.UseLocalMemory(
                    p => p.WithSlidingExpiration(10)), "MyCustomPolicy");

            services.AddCachePolicy(c =>
                c.UseCacheManager(s => s.UseMemoryCache(30)), "CM_InMemoryPolicy");


            services.AddCachePolicy(c =>
                c.UseCacheManager(s => s.WithMicrosoftMemoryCacheHandle()), "CM_InMemoryPolicy");

            services.AddCachePolicy(c =>
                c.UseCacheManager(s => s.WithRedisBackplane("redis.azure.us")
                    .WithRedisCacheHandle("redis.azure.us")), "CM_RedisPolicy");

            // Default policy and Default region
            services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
                c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r), s => s.GenerateKeyUsing(r => r.SearchCriteria.ToLowerInvariant()))
                      .Cache(s => s.GetAllPeopleAsync())
                );

            // SAMPLE CONFIGURATIONS

            //// Custom policy and Default region
            //services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
            //    c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r),
            //            s => s.GenerateKeyUsing(r => r.SearchCriteria.ToLowerInvariant()))
            //        .UsePolicy("MyCustomPolicy"));

            //// Custom policy and Custom region
            //services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
            //    c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r),
            //            s => s.GenerateKeyUsing(r => r.SearchCriteria.ToLowerInvariant()))
            //        .UsePolicy("MyCustomPolicy").AssignToRegion("CustomRegion"));

            //// Default policy and Custom region
            //services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
            //    c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r),
            //            s => s.GenerateKeyUsing(r => r.SearchCriteria.ToLowerInvariant()))
            //        .AssignToRegion("CustomRegion"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            app.UseMvc();
        }
    }
}