using Application.Helper;
using Application.Helper.Profiles;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationRegistrationservices(this IServiceCollection services)
    {
        // تسجيل AutoMappers
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<RoleProfile>();
        });

        // تسجيل MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}