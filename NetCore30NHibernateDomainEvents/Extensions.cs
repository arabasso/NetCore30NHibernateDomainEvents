using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NetCore30NHibernateDomainEvents.Events;
using NetCore30NHibernateDomainEvents.Events.Handles;
using NetCore30NHibernateDomainEvents.Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
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

        public static Configuration AddListener<T>(
            this Configuration configuration,
            ListenerType type,
            T listener)
            where T : class
        {
            configuration.AppendListeners(type, new [] { listener });

            return configuration;
        }

        public static Configuration AddDomainEventListener(
            this Configuration configuration)
        {
            var listener = new DomainEventHandleListener();

            configuration.AddListener<IPreInsertEventListener>(ListenerType.PreInsert, listener);
            configuration.AddListener<IPostInsertEventListener>(ListenerType.PostInsert, listener);
            configuration.AddListener<IPreUpdateEventListener>(ListenerType.PreUpdate, listener);
            configuration.AddListener<IPostUpdateEventListener>(ListenerType.PostUpdate, listener);
            configuration.AddListener<IPreDeleteEventListener>(ListenerType.PreDelete, listener);
            configuration.AddListener<IPostDeleteEventListener>(ListenerType.PostDelete, listener);

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

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PostInsertEvent @event)
        {
            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) => new NHibernatePropertyEntry(name, false, null, @event.State, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PostUpdateEvent @event)
        {
            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) =>
                {
                    var isModified = @event.Persister.PropertyTypes[i].IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                    return new NHibernatePropertyEntry(name, isModified, @event.OldState, @event.State, i);
                });
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PostDeleteEvent @event)
        {
            return @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.DeletedState, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PreDeleteEvent @event)
        {
            return @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) => new NHibernatePropertyEntry(name, false, null, @event.DeletedState, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PreInsertEvent @event)
        {
            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) => new NHibernatePropertyEntry(name, false, null, @event.State, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PreUpdateEvent @event)
        {
            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) =>
                {
                    var isModified = @event.Persister.PropertyTypes[i].IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                    return new NHibernatePropertyEntry(name, isModified, @event.OldState, @event.State, i);
                });
        }
    }
}