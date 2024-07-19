// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Mail;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailer<TMailer>(this IServiceCollection services)
        where TMailer : AbstractMailer => services.AddScoped<AbstractMailer, TMailer>();
}
