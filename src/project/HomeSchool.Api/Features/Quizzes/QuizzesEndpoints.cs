// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace HomeSchool.Api.Features.Quizzes;

public static class QuizzesEndpoints
{
    public static IEndpointRouteBuilder MapQuizzes(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes", GetQuizesByFilter);

        return app;
    }

    private static Task GetQuizesByFilter(HttpContext context) => throw new NotImplementedException();
}
