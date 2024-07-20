namespace HomeSchool.Api.Features.Quizzes;

public static class QuizzesEndpoints
{
    public static IEndpointRouteBuilder MapQuizzes(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes", GetQuizesByFilter);
        
        
        return app;
    }

    private static Task GetQuizesByFilter(HttpContext context)
    {
        throw new NotImplementedException();
    }
}