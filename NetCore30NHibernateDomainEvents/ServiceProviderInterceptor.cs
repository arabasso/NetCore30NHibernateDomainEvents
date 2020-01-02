using System;
using NHibernate;

namespace NetCore30NHibernateDomainEvents
{
    public class ServiceProviderInterceptor
        : EmptyInterceptor
    {
        public IServiceProvider ServiceProvider { get; }

        public ServiceProviderInterceptor(
            IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}