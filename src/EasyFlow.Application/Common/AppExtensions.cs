using EasyFlow.Application.Tags;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Application.Common;

public static class AppExtensions
{
    public static ServiceCollection AddApplication(this ServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTagCommand).Assembly));
        return services;
    }
}