using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore30NHibernateDomainEvents.Models;
using NHibernate;

namespace NetCore30NHibernateDomainEvents
{
    class Program
    {
        static async Task Main(
            string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (!string.IsNullOrEmpty(environment))
            {
                Console.WriteLine("Environment: {0}", environment);
            }

            var builder = new ConfigurationBuilder()
                .AddCommandLine(args)
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();

            if (environment == "Development")
            {
                var path = Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}..{0}", Path.DirectorySeparatorChar));

                builder.AddJsonFile(Path.Combine(path, "appsettings.json"), true);
                builder.AddJsonFile(Path.Combine(path, $"appsettings.{environment}.json"), true);
            }

            else
            {
                builder.AddJsonFile("appsettings.json", false);
            }

            var configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(c => configuration);
            services.AddDataDomain(configuration.GetConnectionString("DefaultConnection"));
            services.AddDomainEvents();

            await using var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var session = scope.ServiceProvider.GetRequiredService<ISession>();

            using var trans = session.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var user = new User("Raphael Basso", "arabasso", new Group("Users"))
                {
                    Email = "arabasso@yahoo.com.br"
                };

                await session.SaveAsync(user);

                await trans.CommitAsync();
            }

            catch (Exception e)
            {
                await trans.RollbackAsync();

                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
