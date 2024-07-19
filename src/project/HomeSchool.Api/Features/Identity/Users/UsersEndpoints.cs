// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace HomeSchool.Api.Features.Identity.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app) => app;

    public static IEndpointRouteBuilder MapVerboseMe(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/user/me", GetMeVerbose);

        return app;
    }

    private static Task GetMeVerbose(HttpContext context) => throw new NotImplementedException();
}
