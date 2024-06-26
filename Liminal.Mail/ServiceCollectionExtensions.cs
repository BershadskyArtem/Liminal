using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Mail;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailer<TMailer>(this IServiceCollection services)
        where TMailer : AbstractMailer
    {
        return services.AddScoped<AbstractMailer, TMailer>();
    }
}