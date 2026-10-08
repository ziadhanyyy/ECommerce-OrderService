using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderService.Data.Interface;
using OrderService.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("MongoDB")!;
            
            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

            services.AddScoped<IMongoDatabase>(provider =>
            {
                IMongoClient client = provider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(configuration["MongoDB:Database"]);
            });

            services.AddScoped<IOrderRepository, OrderRepository>();


            return services;
        }
    }
}
