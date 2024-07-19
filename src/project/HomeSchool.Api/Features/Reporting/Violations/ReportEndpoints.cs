// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HomeSchool.Api.Features.Reporting.Violations;
using System.Text.Json.Serialization;
using ErrorOr;
using FluentValidation;
using HomeSchool.Core.Data;
using HomeSchool.Core.Reporting.Violations.Domain;
using HomeSchool.Core.Reporting.Violations.Enums;
using HomeSchool.Core.Reporting.Violations.Services;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;
using Liminal.Auth.Requirements;
using Liminal.Common.EntityFrameworkCore;
using Liminal.Common.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Created = Microsoft.AspNetCore.Http.HttpResults.Created;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReport<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        // TODO: Add admin requirements.
        app.MapGet("api/reports/{id}", MapGetById<TUser>)
            .RequireAuthorization(PolicyDefaults.AdminName)
            .WithOpenApi(options =>
            {
                options.Summary = "Get report by id";
                options.Description = "Returns report using specified id";

                return options;
            })
            .WithTags("Reporting");

        app.MapGet("api/reports", MapGetByFilter<TUser>)
            .RequireAuthorization(PolicyDefaults.AdminName)
            .WithOpenApi(options =>
            {
                options.Summary = "Get report using filter";
                options.Description = "Returns reports using filter";

                return options;
            })
            .WithTags("Reporting");

        app.MapPost("api/reports/", MapCreateReport<TUser>)
            .WithOpenApi(options =>
            {
                options.Summary = "Create report";
                options.Description = "Creates report for user or issue";

                return options;
            })
            .WithTags("Reporting");

        app.MapDelete("api/reports/{id}", MapDeleteReport<TUser>)
            .RequireAuthorization(PolicyDefaults.AdminName)
            .RequireAuthorization()
            .WithOpenApi(options =>
            {
                options.Summary = "Delete a specified report";
                options.Description = "Deletes a report with specified id";

                return options;
            })
            .WithTags("Reporting");

        app.MapPatch("api/reports/{id}", MapUpdateReport<TUser>)
            .RequireAuthorization(PolicyDefaults.AdminName)
            .RequireAuthorization()
            .WithOpenApi(options =>
            {
                options.Summary = "Update a specified report";
                options.Description = "Updates a report with specified id";

                return options;
            })
            .WithTags("Reporting");

        return app;
    }

    public class CreateReportRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("severity")]
        public ReportSeverity Severity { get; set; }

        [JsonPropertyName("target_id")]
        public Guid? TargetId { get; set; }
    }

    public class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
    {
        public CreateReportRequestValidator() => this.RuleFor(r => r.Text)
                .NotEmpty();
    }

    // Create
    public static async Task<Results<Created, BadRequest<List<Error>>>> MapCreateReport<TUser>(
        [FromBody] CreateReportRequest req,
        [FromServices] IAuthContext<TUser> authContext,
        [FromServices] IReportingService reportingService)
        where TUser : AbstractUser
    {
        var userId = authContext.UserId;

        ErrorOr<Guid> result;

        if (req.TargetId is null)
        {
            result = await reportingService.ReportIssueAsync(req.Text, req.Severity, userId);
        }
        else
        {
            result = await reportingService.ReportUserAsync(req.TargetId.Value, req.Text, req.Severity, userId);
        }

        var response = result.Match<Results<Created, BadRequest<List<Error>>>>(
            (id) => TypedResults.Created(id.ToString()),
            err => TypedResults.BadRequest(err));

        return response;
    }

    public class UpdateReportRequest
    {
        public bool Resolve { get; set; }

        public string ResolveMessage { get; set; } = default!;

        public bool SendNotification { get; set; }
    }

    public class UpdateReportRequestValidator : AbstractValidator<UpdateReportRequest>
    {
        public UpdateReportRequestValidator() => this.RuleFor(r => r.ResolveMessage)
                .NotEmpty();
    }

    // Update 
    public static async Task<Results<Ok, NotFound, UnauthorizedHttpResult>> MapUpdateReport<TUser>(
        [FromRoute] Guid id,
        [FromBody] UpdateReportRequest req,
        [FromServices] IAuthContext<TUser> authContext,
        [FromServices] IReportingService reportingService)
        where TUser : AbstractUser
    {
        var userId = authContext.UserId;
        ArgumentNullException.ThrowIfNull(userId);

        var result =
            await reportingService.SetResolveAsync(
                id,
                userId.Value,
                req.ResolveMessage,
                req.Resolve,
                req.SendNotification);

        var response = result.Match<Results<Ok, NotFound, UnauthorizedHttpResult>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.NotFound());

        return response;
    }

    // Delete
    public static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> MapDeleteReport<TUser>(
        [FromRoute] Guid id,
        [FromServices] IAuthContext<TUser> authContext,
        [FromServices] IReportingService reportingService)
        where TUser : AbstractUser
    {
        var userId = authContext.UserId;
        ArgumentNullException.ThrowIfNull(userId);

        var result = await reportingService.DeleteAsync(id);

        var response = result.Match<Results<NoContent, NotFound, UnauthorizedHttpResult>>(
            (_) => TypedResults.NoContent(),
            _ => TypedResults.NotFound());

        return response;
    }

    // Get
    public class GetReportResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("severity")]
        public ReportSeverity Severity { get; set; }

        [JsonPropertyName("author_id")]
        public Guid? AuthorId { get; set; }

        [JsonPropertyName("is_resolved")]
        public bool IsResolved { get; set; }

        [JsonPropertyName("resolved_at")]
        public DateTimeOffset? ResolvedAt { get; set; }

        [JsonPropertyName("resolve_message")]
        public string? ResolveMessage { get; set; }

        [JsonPropertyName("resolved_by")]
        public Guid? ResolvedBy { get; set; }

        [JsonPropertyName("target_id")]
        public Guid? TargetId { get; set; }

        public static GetReportResponse FromModel(Report report) => new GetReportResponse
        {
            Id = report.Id,
            AuthorId = report.AuthorId,
            CreatedAt = report.CreatedAt,
            IsResolved = report.IsResolved,
            Severity = report.Severity,
            TargetId = report.TargetUserId,
            Text = report.Text,
            ResolvedAt = report.ResolvedAt,
            ResolvedBy = report.ResolvedBy,
            ResolveMessage = report.ResolveMessage,
        };
    }

    public static async Task<Results<Ok<GetReportResponse>, NotFound, UnauthorizedHttpResult>> MapGetById<TUser>(
        [FromRoute] Guid id,
        [FromServices] IAuthContext<TUser> authContext,
        [FromServices] ApplicationDbContext context)
        where TUser : AbstractUser
    {
        var userId = authContext.UserId;
        ArgumentNullException.ThrowIfNull(userId);

        var result = await context
            .Reports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(GetReportResponse.FromModel(result));
    }

    public class GetReportsByFilterRequest : PagedRequest
    {
        [JsonPropertyName("target_id")]
        public Guid? TargetId { get; set; }

        [JsonPropertyName("author_id")]
        public Guid? AuthorId { get; set; }

        [JsonPropertyName("start")]
        public DateTimeOffset? Start { get; set; }

        [JsonPropertyName("finish")]
        public DateTimeOffset? Finish { get; set; }

        [JsonPropertyName("is_resolved")]
        public bool? IsResolved { get; set; }

        [JsonPropertyName("minimum_severity")]
        public ReportSeverity? MinimumSeverity { get; set; }

        [JsonPropertyName("maximum_severity")]
        public ReportSeverity? MaximumSeverity { get; set; }

        [JsonPropertyName("resolved_by")]
        public Guid? ResolvedBy { get; set; }
    }

    public static async Task<Results<Ok<List<GetReportResponse>>, NotFound, UnauthorizedHttpResult>> MapGetByFilter<TUser>(
        [AsParameters] GetReportsByFilterRequest req,
        [FromServices] IAuthContext<TUser> authContext,
        [FromServices] ApplicationDbContext context)
        where TUser : AbstractUser
    {
        var userId = authContext.UserId;
        ArgumentNullException.ThrowIfNull(userId);

        var query = context
            .Reports
            .AsNoTracking();

        if (req.TargetId is not null)
        {
            query = query.Where(r => r.TargetUserId == req.TargetId);
        }

        if (req.AuthorId is not null)
        {
            query = query.Where(r => r.AuthorId == req.AuthorId);
        }

        if (req.Start is not null)
        {
            query = query.Where(r => r.CreatedAt >= req.Start);
        }

        if (req.Finish is not null)
        {
            query = query.Where(r => r.ResolvedAt <= req.Finish);
        }

        if (req.IsResolved is not null)
        {
            query = query.Where(r => r.IsResolved == req.IsResolved);
        }

        if (req.MinimumSeverity is not null)
        {
            query = query.Where(r => r.Severity >= req.MinimumSeverity);
        }

        if (req.MaximumSeverity is not null)
        {
            query = query.Where(r => r.Severity <= req.MaximumSeverity);
        }

        if (req.ResolvedBy is not null)
        {
            query = query.Where(r => r.ResolvedBy == req.ResolvedBy);
        }

        var result = await query.Paged(req).ToListAsync();

        if (!result.Any())
        {
            return TypedResults.NotFound();
        }

        var response = result
            .Select(GetReportResponse.FromModel).ToList();

        return TypedResults.Ok(response);
    }
}
