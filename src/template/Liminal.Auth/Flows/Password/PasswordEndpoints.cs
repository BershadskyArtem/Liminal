// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;
using FluentValidation;
using Liminal.Auth.Extensions;
using Liminal.Auth.Jwt;
using Liminal.Auth.Models;
using Liminal.Auth.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Liminal.Auth.Flows.Password;

public static class PasswordEndpoints
{
    public static IEndpointRouteBuilder MapPassword<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app.MapRegister<TUser>();
        app.MapSendConfirmation<TUser>();
        app.MapActivate<TUser>();
        app.MapLogin<TUser>();

        return app;
    }

    public static IEndpointRouteBuilder MapRegister<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapPost("api/auth/password/register", RegisterEndpoint<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Description = "Register user using password and email";
                options.Summary = "Password register";
                return options;
            }).WithTags("Password");

        return app;
    }

    public class RegisterRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            this.RuleFor(req => req.Email)
                .NotEmpty()
                .EmailAddress();

            // https://stackoverflow.com/questions/64205825/regex-with-fluent-validation-in-c-sharp-how-to-not-allow-spaces-and-certain-sp
            this.RuleFor(req => req.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("'{PropertyName}' must contain one or more capital letters.")
                .Matches("[a-z]").WithMessage("'{PropertyName}' must contain one or more lowercase letters.")
                .Matches(@"\d").WithMessage("'{PropertyName}' must contain one or more digits.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("'{ PropertyName}' must contain one or more special characters.")
                .Matches("^[^£# “”]*$").WithMessage("'{PropertyName}' must not contain the following characters £ # “” or spaces.")
                .WithMessage("'{PropertyName}' contains a word that is not allowed.");
        }
    }

    public class RegisterResponse
    {
        [JsonPropertyName("success")]
        public bool IsSuccess { get; set; }
    }

    public static async Task<Results<Ok, BadRequest, BadRequest<List<FluentValidation.Results.ValidationFailure>>>> RegisterEndpoint<TUser>(
        [FromBody] RegisterRequest req,
        [FromServices] IValidator<RegisterRequest> validator,
        [FromServices] PasswordFlow<TUser> flow)
        where TUser : AbstractUser
    {
        var valid = await validator.ValidateAsync(req);

        if (!valid.IsValid)
        {
            return TypedResults.BadRequest(valid.Errors);
        }

        var result = await flow.Register(req.Email, req.Password, () =>
        {
            var user = Activator.CreateInstance<TUser>();
            return user;
        });

        if (result)
        {
            return TypedResults.Ok();
        }

        return TypedResults.BadRequest();
    }

    public static IEndpointRouteBuilder MapSendConfirmation<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app.MapPost("api/auth/password/confirm", SendConfirmationEndpoint<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Description = "Send confirmation email using password flow";
                options.Summary = "Send confirmation email";
                return options;
            })
            .WithTags("Password");

        return app;
    }

    public class SendConfirmationEmailRequest
    {
        public string Email { get; set; } = default!;
    }

    public class SendConfirmationEmailValidator : AbstractValidator<SendConfirmationEmailRequest>
    {
        public SendConfirmationEmailValidator() => this.RuleFor(req => req.Email)
                .NotEmpty()
                .EmailAddress();
    }

    public static async Task<Results<Ok, BadRequest, BadRequest<List<FluentValidation.Results.ValidationFailure>>>>
        SendConfirmationEndpoint<TUser>(
        [FromBody] SendConfirmationEmailRequest req,
        [FromServices] IValidator<SendConfirmationEmailRequest> validator,
        [FromServices] PasswordFlow<TUser> flow)
        where TUser : AbstractUser
    {
        var valid = await validator.ValidateAsync(req);

        if (!valid.IsValid)
        {
            return TypedResults.BadRequest(valid.Errors);
        }

        var result = await flow.SendConfirmationEmail(req.Email);

        if (result)
        {
            return TypedResults.Ok();
        }

        return TypedResults.BadRequest();
    }

    public static IEndpointRouteBuilder MapActivate<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app.MapGet("api/auth/password/confirm", ConfirmAccount<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Description = "Confirm password account using token";
                options.Summary = "Confirm account";
                return options;
            })
            .WithTags("Password");

        return app;
    }

    public static async Task<Results<BadRequest, Ok<GenerateTokenResult>, BadRequest<GenerateTokenResult>>>
        ConfirmAccount<TUser>(
        [FromQuery] string token,
        [FromServices] PasswordFlow<TUser> flow,
        HttpContext context)
        where TUser : AbstractUser
    {
        var result = await flow.ActivateAccount(token, "/");

        if (!result.IsSuccess || result.Principal is null)
        {
            return TypedResults.BadRequest();
        }

        var signInResult = await context.SignInAsyncLiminal(result.Principal, JwtDefaults.Scheme);

        if (signInResult.IsSuccess)
        {
            return TypedResults.Ok(signInResult);
        }

        return TypedResults.BadRequest(signInResult);
    }

    public class LoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            this.RuleFor(req => req.Email)
                .NotEmpty()
                .EmailAddress();

            // Use mini version for faster validation.
            this.RuleFor(req => req.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }

    public static IEndpointRouteBuilder MapLogin<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app.MapPost("api/auth/login", LoginEndpoint<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Description = "Login using email and password and password flow";
                options.Summary = "Login using password";
                return options;
            })
            .WithTags("Password");

        return app;
    }

    public static async Task<Results<
        BadRequest<GenerateTokenResult>,
        BadRequest,
        BadRequest<List<FluentValidation.Results.ValidationFailure>>,
        Ok<GenerateTokenResult>>>
        LoginEndpoint<TUser>(
        [FromBody] LoginRequest req,
        [FromServices] IValidator<LoginRequest> validator,
        [FromServices] PasswordFlow<TUser> flow,
        HttpContext context)
        where TUser : AbstractUser
    {
        var valid = await validator.ValidateAsync(req);

        if (!valid.IsValid)
        {
            return TypedResults.BadRequest(valid.Errors);
        }

        var result = await flow.Login(req.Email, req.Password, "/");

        if (!result.IsSuccess || result.Principal is null)
        {
            return TypedResults.BadRequest();
        }

        var signInResult = await context.SignInAsyncLiminal(result.Principal, JwtDefaults.Scheme);

        if (signInResult.IsSuccess)
        {
            return TypedResults.Ok(signInResult);
        }

        return TypedResults.BadRequest(signInResult);
    }
}
