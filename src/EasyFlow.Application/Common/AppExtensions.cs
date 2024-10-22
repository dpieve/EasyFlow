using EasyFlow.Application.Settings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Application.Common;

public static class AppExtensions
{
    public static ServiceCollection AddApplication(this ServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Sessions.Create).Assembly));

        services.AddAutoMapper(typeof(MappingProfiles).Assembly);

        services.AddValidatorsFromAssemblyContaining<SettingsValidator>();

        return services;
    }
}