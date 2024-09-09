using Redis.OM;
using Restaurant.API.Caching.Models;

namespace Restaurant.API.Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "connectionString is null");

        return services
            .AddSingleton((_) => new RedisConnectionProvider(connectionString));
    }

    public static IServiceCollection AddRedisIndexes(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<RedisConnectionProvider>()
            ?? throw new InvalidOperationException("cannot register indexes for redis cache");

        try
        {
            provider.Connection.CreateIndex(typeof(EmployeeRoleCacheModel));
            provider.Connection.CreateIndex(typeof(EmployeeCacheModel));
            provider.Connection.CreateIndex(typeof(CustomerCacheModel));
            provider.Connection.CreateIndex(typeof(DeskCacheModel));
        }
        catch { }

        return services;
    }

    public static IServiceCollection AddRedisModels(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<RedisConnectionProvider>()
                   ?? throw new InvalidOperationException("cannot register models for redis cache");

        return services
            .AddScoped((_) => provider.RedisCollection<DeskCacheModel>())
            .AddScoped((_) => provider.RedisCollection<CustomerCacheModel>())
            .AddScoped((_) => provider.RedisCollection<EmployeeCacheModel>())
            .AddScoped((_) => provider.RedisCollection<EmployeeRoleCacheModel>());
    }
}