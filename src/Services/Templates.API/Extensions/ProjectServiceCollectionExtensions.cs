namespace API.Extensions
{
    using global::Templates.API.BussinessLogic;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.StackExchangeRedis;
    using Microsoft.Extensions.DependencyInjection;
    using Templates.API;

    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods add project services.
    /// </summary>
    /// <remarks>
    /// AddSingleton - Only one instance is ever created and returned.
    /// AddScoped - A new instance is created and returned for each request/response cycle.
    /// AddTransient - A new instance is created and returned each time.
    /// </remarks>
    public static class ProjectServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services) => services
            .AddSingleton<IMerchantListsHandler, MerchantListsHandler>()
            .AddSingleton<ICouponsHandler, CouponsHandler>()
            .AddSingleton<ISynchronizedHandler, SynchronizedHandler>()
            .AddSingleton<INewsHandler, NewsHandler>()
            .AddSingleton<ITypeNewsHandler, TypeNewsHandler>()
            .AddSingleton<IStatusHandler, StatusHandler>()
            .AddSingleton<ITypePromotionsHandler, TypePromotionsHandler>()
            .AddSingleton<IClickMerchantHandler, ClickMerchantHandler>()
            .AddSingleton<IDashboardsHandler, DashboardsHandler>()
            .AddSingleton<IMenusHandler, MenusHandler>()
            .AddSingleton<IUsersHandler, UsersHandler>()
            .AddSingleton<IRedisService, RedisService>()            
            ;  
    }
}