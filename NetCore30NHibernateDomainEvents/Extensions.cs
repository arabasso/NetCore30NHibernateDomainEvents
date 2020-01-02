using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NetCore30NHibernateDomainEvents.Events;
using NetCore30NHibernateDomainEvents.Events.Handles;
using NetCore30NHibernateDomainEvents.Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;

namespace NetCore30NHibernateDomainEvents
{
    public static class Extensions
    {
        public static IServiceProvider GetServiceProvider(
            this AbstractEvent @event)
        {
            return ((ServiceProviderInterceptor)@event.Session.Interceptor).ServiceProvider;
        }

        public static T GetService<T>(
            this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public static IEnumerable<T> GetServices<T>(
            this IServiceProvider serviceProvider)
        {
            var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(typeof(T));

            return (IEnumerable<T>)serviceProvider.GetService(genericEnumerable);
        }

        public static IEnumerable<object> GetServices(
            this IServiceProvider serviceProvider,
            Type serviceType)
        {
            var enumerable = typeof(IEnumerable<>).MakeGenericType(serviceType);

            return (IEnumerable<object>)serviceProvider.GetService(enumerable);
        }

        public static ServiceCollection AddDataDomain(
            this ServiceCollection services,
            string connectionString,
            bool create = true)
        {
            services.AddScoped<ServiceProviderInterceptor>();

            services.AddSingleton(c =>
            {
                var cfg = new Configuration()
                    .AddMapping()
                    .AddDomainEventListener()
                    .UseMysql(connectionString);

                if (create)
                {
                    cfg.DataBaseIntegration(db => db.SchemaAction = SchemaAutoAction.Create);
                }

                return cfg;
            });
            services.AddSingleton(c => c.GetRequiredService<Configuration>().BuildSessionFactory());
            services.AddScoped(c => c.GetRequiredService<ISessionFactory>()
                .WithOptions()
                .Interceptor(new ServiceProviderInterceptor(c))
                .OpenSession());

            services.AddScoped<DomainEventHandleService>();

            return services;
        }

        public static ServiceCollection AddDomainEvents(
            this ServiceCollection services)
        {
            services.AddTransient<IDomainEventHandle<BeforeAddedEntityDomainEvent<User>>, BeforeAddedUserModelDomainEventHandle>();
            services.AddTransient<IDomainEventHandleAsync<BeforeAddedEntityDomainEvent<User>>, BeforeAddedUserModelDomainEventHandle>();

            return services;
        }

        public static IEnumerable<T> ReverseConcat<T>(
            this IEnumerable<T> source, params T[] items)
        {
            return items.Concat(source);
        }

        public static Configuration AddDomainEventListener(
            this Configuration configuration)
        {
            var listener = new DomainEventHandleListener();

            configuration.SetListener(ListenerType.PreInsert, listener);
            configuration.SetListener(ListenerType.PostInsert, listener);
            configuration.SetListener(ListenerType.PreUpdate, listener);
            configuration.SetListener(ListenerType.PostUpdate, listener);
            configuration.SetListener(ListenerType.PreDelete, listener);
            configuration.SetListener(ListenerType.PostDelete, listener);

            return configuration;
        }

        public static Configuration AddMapping(
            this Configuration configuration)
        {
            var modelMapper = new ModelMapper();

            modelMapper.AddMapping<Models.Mappings.UserMapping>();
            modelMapper.AddMapping<Models.Mappings.GroupMapping>();

            configuration.AddMapping(modelMapper.CompileMappingForAllExplicitlyAddedEntities());

            return configuration;
        }

        public static Configuration UseMysql(
            this Configuration configuration,
            string connectionString)
        {
            configuration.DataBaseIntegration(db =>
            {
                db.Driver<NHibernate.Driver.MySqlDataDriver>();
                db.Dialect<NHibernate.Dialect.MySQL5InnoDBDialect>();
                db.ConnectionString = connectionString;
                db.AutoCommentSql = true;
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            });

            return configuration;
        }
    }
}