using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.BLL.Interfaces;
using OrderService.BLL.Services;
using OrderService.BLL.Mappers;
using OrderService.BLL.Validators;

namespace OrderService.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();

            services.AddAutoMapper(cfg => { }, typeof(OrderAddRequestToOrderMappingProfile).Assembly);

            services.AddScoped<IOrderService, OrderService.BLL.Services.OrderService>();
            return services;
        }
    }
}
