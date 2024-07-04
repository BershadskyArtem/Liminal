namespace HomeSchool.Api.Features.Identity.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app)
    {
        return app;
    }

   

    public static IEndpointRouteBuilder MapVerboseMe(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/user/me", GetMeVerbose);
        
        return app;
    }

    private static Task GetMeVerbose(HttpContext context)
    {
        throw new NotImplementedException();
    }
}