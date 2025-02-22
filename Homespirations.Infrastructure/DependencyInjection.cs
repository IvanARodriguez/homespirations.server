using Amazon.S3;
using Homespirations.Core.Interfaces;
using Homespirations.Infrastructure.Repositories;
using Homespirations.Infrastructure.Services;
using Homespirations.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Homespirations.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddLogging();

            services.AddSingleton(provider =>
           {
               var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
               return loggerFactory.CreateLogger("Infrastructure");
           });

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
             {
                 var logger = serviceProvider.GetRequiredService<ILogger>();

                 var connectionString = configuration.GetConnectionString("DefaultConnection")
                                     ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

                 if (string.IsNullOrEmpty(connectionString))
                 {
                     logger.LogCritical("Database connection string is missing.");
                     throw new Exception("Database connection string is missing.");
                 }

                 logger.LogInformation("Configuring database with connection string: {ConnectionString}", connectionString);

                 options.EnableSensitiveDataLogging();
                 options.UseNpgsql(connectionString);
             });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IImageOptimizer, ImageOptimizer>();
            services.AddScoped<ICloudStorage, CloudStorage>();
            services.AddSingleton<IAmazonS3, AmazonS3Client>();

            return services;

        }

    }
}