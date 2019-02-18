# Fluent Cache

[![Build status](https://ci.appveyor.com/api/projects/status/6fqw0sy0jp6s584o?svg=true)](https://ci.appveyor.com/project/moattarwork/flache)
https://img.shields.io/nuget/v/Flache.svg


## Introduction

Goal of this project is to provide an easy way to enable CACHE on any service we may use in our applications. The API is written in a 'non intrusive' way meaning, you don't have to actually change anything in your code in order to enable CACHING. Please take a look at the rest of the document for examples and extension points.

## Features

- Fluent API
- Simple one line configuration to enable CACHE for any service
- Support for reusable cache Policies
- Support for regions across different cache stores
- Integration with [CacheManager](http://cachemanager.michaco.net/) via **Flache.CacheManager** [NUGET](https://www.nuget.org/) package.
- Dictionary based InMemory cache implementation
- Support easy integration for other cache providers
- Caching multiple methods per service

## Road map

- Allow expiration on cache definition to override the policy
- Provide alternative facility to generate CACHE KEY's

## How to download

```shell
Install-Package Flache
Install-Package Flache.CacheManager

```

or using dotnet cli

```shell
dotnet add package Flache
dotnet add package Flache.CacheManager
```

## Quick Start

To configure simple cache backed by _LocalInMemoryCache_ we would need to do a few things:

- Install following [NUGET](https://www.nuget.org/) packages:
  - **Flache** - contains FluentCache
  - **Flache.LocalInMemoryStorage** - contains LocalInMemoryCacheStore for storing the cache in local memory **(This store is only for testing purpose and for production purpose use the [CacheManager](http://cachemanager.michaco.net/) implementation)**
- Configure CACHE in our **startup.cs** (irrelevant code omit for brevity):
  - Register FluentAPI -> **services.UseFluentCache();**
  - Since we are using **LocalInMemoryStorage** we need to register supporting services by calling **services.AddLocalMemoryStorage();**
  - Add default caching policy as shown in the code example below
  - FluentAPI depends on **IServiceCollection** which is a NATIVE .NET Core DI container, meaning your project should leverage it or you will need to provide an adapter to whichever DI framework your project is using
  - The last bit is to register your dependency with caching enabled using one of the 3 available methods: _AddTransientCache_, _AddScopedCache_ or _AddSingletonCache_.

```csharp

 public class Startup  {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.UseFluentCache();
            services.AddLocalMemoryStorage();
            services.AddCachePolicy(
                c => c.UseLocalMemory(
                    p => p.WithSlidingExpiration(10)));

            services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
                c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r),
                        s => s.GenerateKeyUsing(r => r.SearchCriteria)));
        }
 }

```

You may be wondering what the rest of the parameters mean and this is what we are going to look into next.

## Policies

Policy is simply a reusable piece of configuration that describes how data is being cached. Typical policy would have backing cache storage configuration, such as REDIS/NoSQL/SQL/LocalMemory, expiration rules, multiple layer configuration and so on. Keep in mind that configuration is highly specific to the storage mechanism/framework you choose. Currently we support Native InMemory storage (implemented in house for demo purposes) and [CacheManager](http://cachemanager.michaco.net/) API, which internally supports many cache stores.

> If you are interested in more details how these strategies work and some alternative approaches please refer to following [article](https://coderanch.com/wiki/660295/Caching-Strategies).

Simples cache policy would look like so:

```csharp

            services.AddCachePolicy(
                c => c.UseLocalMemory(
                    p => p.WithNoExpiration()));

```

**UseLocalMemory** is an extension method that selects storage implementation and exposes internal API for further configuration, in this case we also specify expiration policy, however different storages will have different API's that are not internal to **Flache** framework.

We also have a second parameter to give this policy a unique name, so we could register multiple policies with different configurations. An example would be, we may want a local in memory cache and another one using [REDIS](https://redis.io/) cache, both configured via [CacheManager](http://cachemanager.michaco.net/):

```csharp

            services.AddCachePolicy(c =>
                c.UseCacheManager(s => s.WithMicrosoftMemoryCacheHandle()), "CM_InMemoryPolicy");

            services.AddCachePolicy(c =>
                c.UseCacheManager(s => s.WithRedisBackplane("redis.azure.us")
                    .WithRedisCacheHandle("redis.azure.us")), "CM_RedisPolicy");

```

Here are 2 examples of in memory cache ("CM_InMemoryPolicy") and [REDIS](https://redis.io/) based cache ("CM_RedisPolicy") policies. Later we can refer to these policies using the names specified during registration.

> If more than one policy is registered using same name an exception will be thrown when this policy is being resolved by the cached service, due to inability to determine which policy should take precedence.

In order to leverage [CacheManager](http://cachemanager.michaco.net/) library you will need to install **Flache.CacheManager** [NUGET](https://www.nuget.org/) package.

## Cache Configuration

In the previous example we have seen a simple cache configuration, but perhaps there are a few things we would like to clarify. The first 2 type parameters are same as you would expect for any normal service registration using say _AddTransient_ -> **AddCache<_TInterface_,_TImplementation_>** so this is exactly what we have in our CACHED scenario _AddTransientCache_ -> **AddTransientCache<_TInterface_,_TImplementation_>**. The lambda supplied to you is a _CACHE CONFIGURATOR_ service, which has single method _CACHE_ with overloads to support from 0 up to 4 parameters for the CACHED method. _CACHE_ method itself is a generic method, that will take 1 to 5 GENERIC parameter types that reflects 1-4 parameter types of the CACHED method and the last TYPE represents a return value. Note that in case method is _async_, the return value type DOESN'T need wrapping in a **TASK<>**. The two parameters the method requires are:

- Method to CACHE -> as you can see first parameter is again a lambda -> _(s, r)_ where **s** is a reference to CACHED service and **r** is parameter required by the METHOD we are trying to CACHE. Note that number of parameters changes based on how many parameters the METHOD accepts. This is drive by the list of GENERIC types we supplied to **Cache<>** method itself. Method can have NONE or 1 to 4 arguments.
- CACHE key factory -> this is a lambda that gives you all the parameters that CACHED method requires and allows you to provide a CACHE KEY factory in a form of a **Func<>** which will be used to generate keys. This is how CACHE storage differentiates between different requests and knows can it use CACHED data or does it need to invoke the method to hydrate the data from back end service.

After following configuration is supplied we have enough information to know what we are CACHING, how to get it in case we don't have it in CACHE yet and under what KEY it sits, as well as how to build a KEY in case this is the first time we see such request.

Great! But what is the other code used for!? Well naturally we need a store for our CACHE and optionally region, in case we want to CLEAR CACHE for entire region (can span multiple services and CACHE stores). This is the part of configuration we will look in greater detail next:

```csharp

            services.AddCachePolicy(
                c => c.UseLocalMemory(
                    p => p.WithNoExpiration()));

            services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
                c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r),
                    s => s.GenerateKeyUsing(r => r.SearchCriteria)));

```

This configuration would work, assuming we have a default CACHE policy configured, as it is shown in the sample code. Default CACHE policy is considered any policy that doesn't have an explicit name assigned to it. Make sure to register ONLY 1 default CACHE policy. This configuration also uses default REGION, meaning all CACHE's with no explicit region specified will fall into single 'default' REGION.

### Caching methods without any arguments

Configuration for methods that have no arguments is sligthly different, since it doesn't need **CACHE key factory**, wich results in second argument for **Cache<T>** being ommited. Please inspect the code snippet below:

```csharp

            services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
                c => c.Cache(s => s.GetAllPeopleAsync()));

```

### Caching multiple methods on same service

It is quite common to have more than one method on a service that would benefit from caching, which is why we also support such scenario. Configuration is exaclty the same, except we would need to provide configuration for the extra methods we want to cache, like shown in the code snippet below:

```csharp

            services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
                c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r),
                                                                 s => s.GenerateKeyUsing(r => r.SearchCriteria.ToLowerInvariant()))
                      .Cache(s => s.GetAllPeopleAsync())
                );

```

As we can see, we call the **Cache<T>** method and provide configuration for the choosen method.

> Caching **POLICY** and/or **REGION** can be specified on a per method basis

### Using Policies

If we would like to use one of our previously defined policies such as **"CM_InMemoryPolicy"** or **"CM_RedisPolicy"** we assign a policy via **.UsePolicy("MY_POLICY_NAME")** method, as shown in the code snippet below:

```csharp

            services.AddCachePolicy(c =>
                c.UseCacheManager(s => s.WithMicrosoftMemoryCacheHandle()), "CM_InMemoryPolicy");

            services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
                c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetPeopleAsync(r),
                        s => s.GenerateKeyUsing(r => r.SearchCriteria))
                    .UsePolicy("CM_InMemoryPolicy"));

```

Same policy can be used for many services.

### Using Regions

The last piece to the puzzle assigning our CACHE's to regions. You may be wondering, why do I care about regions at all? Well I guess for the most part you don't. However there are some scenarios where you may CACHE data from more than 1 service, it may even span multiple CACHE stores, or use layered CACHE that somewhat related in your domain, meaning you want to CLEAR CACHE for multiple sets of data at the same time. You can achieve this by assigning same REGION for multiple CACHE configurations as it is shown in the code snippet below:

```csharp

            services.AddCachePolicy(c =>
                c.UseCacheManager(s => s.WithMicrosoftMemoryCacheHandle()));

            services.AddTransientCache<IPersonStore, InMemorySlowPersonStore>(
                c => c.Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetFriendsAsync(r),
                        s => s.GenerateKeyUsing(r => r.SearchCriteria))
                              .AssignToRegion("CustomRegion")
                    .Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetEveryoneAsync(r),
                        s => s.GenerateKeyUsing(r => r.SearchCriteria))
                    .Cache<PersonSearchRequest, List<Person>>((s, r) => s.GetFamilyAsync(r),
                        s => s.GenerateKeyUsing(r => r.SearchCriteria))
                              .AssignToRegion("CustomRegion"));

```

As we can see, we have 3 methods cached: **GetFriendsAsync**, **GetEveryoneAsync**, **GetFamilyAsync**. We may assume that in-case we have some changes either in our Friends or Family list we want to clear CACHE for both methods, since same person can be our friend as well as a family member. However we want to leave **GetEveryoneAsync** untouched. In this case we can group those methods by using same REGION, in our case its **"CustomRegion"**. To actually flush the CACHE we would need to inject **ICacheSupervisor** to whichever service does the CACHE flushing as shown in the code snippet below:

```csharp

    public sealed class MyService
    {
        private readonly ICacheSupervisor _cacheSupervisor;

        public MyService(ICacheSupervisor cacheSupervisor)
        {
            _cacheSupervisor = cacheSupervisor ?? throw new ArgumentNullException(nameof(cacheSupervisor));
        }

        public void ResetFriendsAndFamily()
        {
            _cacheSupervisor.ClearCacheAsync("MyCustomRegion");
        }
    }

```

Pay special attention to **\_cacheSupervisor.ClearCacheAsync("MyCustomRegion");** and the fact the we pass the same REGION name as we have used when configuring CACHE's for both methods. Note that **\_cacheSupervisor.ClearCacheAsync();** can be called with no parameters, at which point it would simply clear all CACHE's.

> Currently we are NOT restricted on how many regions we want to use. Keep in mind that it only makes sense to create a separate region if there will be more than 1 CACHE store using it.

Ok so you are hungry for more CACHING knowledge? Well ...

## Advanced (GEEK) stuff

- Integration of external 3rd party CACHE libraries
- Implementing your own CACHE :)
- Extension methods to encapsulate most commonly used CACHE configurations
- More ...

Coming soon ... :)
