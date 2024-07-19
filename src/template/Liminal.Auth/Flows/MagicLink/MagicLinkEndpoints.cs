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

namespace Liminal.Auth.Flows.MagicLink;

public static class MagicLinkEndpoints
{
    public class SendMagicLinkRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;
    }

    public class SendMagicLinkRequestValidator : AbstractValidator<SendMagicLinkRequest>
    {
        public SendMagicLinkRequestValidator() => this.RuleFor(req => req.Email)
                .NotEmpty()
                .EmailAddress();
    }

    public class SendMagicLinkResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;
    }

    public static IEndpointRouteBuilder MapSendLink<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapPost("api/auth/magic", SendMagic<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Summary = "Send magic link";
                options.Description = "Sends magic link to the specified email";

                return options;
            })
            .WithTags("Magic link");
        return app;
    }

    public static async Task<Results<
            Ok<SendMagicLinkResponse>,
            BadRequest<SendMagicLinkResponse>,
            BadRequest<List<FluentValidation.Results.ValidationFailure>>>>
        SendMagic<TUser>(
        [FromBody] SendMagicLinkRequest req,
        [FromServices] IValidator<SendMagicLinkRequest> validator,
        [FromServices] MagicLinkFlow<TUser> flow)
        where TUser : AbstractUser
    {
        var valid = await validator.ValidateAsync(req);

        if (!valid.IsValid)
        {
            return TypedResults.BadRequest(valid.Errors);
        }

        var result = await flow.SendLink(req.Email);

        if (result)
        {
            return TypedResults.Ok(new SendMagicLinkResponse
            {
                IsSuccess = true,
                Message = "Sent",
            });
        }

        return TypedResults.BadRequest(new SendMagicLinkResponse
        {
            IsSuccess = false,
            Message = "Unable to sent email to specified email address",
        });
    }

    public static IEndpointRouteBuilder MapActivateMagic<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapGet("api/auth/magic", AuthenticateMagic<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Summary = "Magic callback";
                options.Description = "Authenticates user using magic link in route";

                return options;
            })
            .WithTags("Magic link");

        return app;
    }

    public static async Task<Results<Ok<GenerateTokenResult>, BadRequest<GenerateTokenResult>, UnauthorizedHttpResult>>
        AuthenticateMagic<TUser>(
        [FromQuery] string code,
        [FromServices] MagicLinkFlow<TUser> flow,
        HttpContext context)
        where TUser : AbstractUser
    {
        var result = await flow.ActivateAsync(code);

        if (!result.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var signInResult = await context.SignInAsyncLiminal(result.Principal, JwtDefaults.Scheme);

        if (!signInResult.IsSuccess)
        {
            return TypedResults.BadRequest(signInResult);
        }

        return TypedResults.Ok(signInResult);
    }

    public static IEndpointRouteBuilder MapMagic<TUser>(this IEndpointRouteBuilder builder)
        where TUser : AbstractUser => builder
            .MapActivateMagic<TUser>()
            .MapSendLink<TUser>();
}
