using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Identity;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Requirements;
using Liminal.Storage.Abstractions;
using Liminal.Storage.AspNetCore;
using Liminal.Storage.EntityFrameworkCore.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeSchool.Api.Features.Attachments;

public static class AttachmentsEndpoints
{
    public static IEndpointRouteBuilder MapAttachments(this IEndpointRouteBuilder app)
    {
        app
            .MapGet("api/attachments/{id}", GetAttachmentById)
            .WithOpenApi(options =>
            {
                options.Summary = "Retrieves attachment info";
                options.Description = "Retrieves attachment info using it's id.";
                
                return options;
            })
            .WithTags("attachments");

        app
            .MapGet("api/attachments/{id}/file", GetAttachmentFileById)
            .WithOpenApi(options =>
            {
                options.Summary = "Retrieves attachment file stream";
                options.Description = "Retrieves attachment file using it's id.";
                
                return options;
            })
            .WithTags("attachments");
        
        app.MapDelete("api/attachments/{id}", DeleteAttachment)
            .WithOpenApi(options =>
            {
                options.Summary = "Deletes attachment file";
                options.Description = "Deletes attachment file using it's id.";
                
                return options;
            })
            .WithTags("attachments");

        
        // OpenApi break this endpoint
        // https://stackoverflow.com/questions/76117745/when-uploading-iformfile-file-with-minimalapi-in-swagger-the-file-is-null
        app.MapPost("api/attachments", PostAttachment)
            .DisableAntiforgery()
            .RequireAuthorization(PolicyDefaults.ConfirmedAccount)
            .Accepts<IFormFile>("multipart/form-data");
        
        return app;
    }

    private static async Task<Results<NotFound, NoContent, ProblemHttpResult, UnauthorizedHttpResult>> DeleteAttachment(
        [FromRoute] Guid id,
        [FromServices] IAttachmentDbContext<ApplicationAttachment> db,
        [FromServices] IAuthContext<ApplicationUser> auth, 
        [FromServices] IFileStore fileStore,
        HttpContext context)
    { 
        var attachment = await db.Set().FirstOrDefaultAsync(a => a.Id == id);

        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        var currentUser = await auth.Current();

        if (currentUser is null)
        {
            return TypedResults.Unauthorized();
        }
        
        if (attachment.UserId != auth.UserId && 
            RoleDefaults.Admin != currentUser.Role && 
            RoleDefaults.SuperAdmin != currentUser.Role)
        {
            return TypedResults.Unauthorized();
        }
        
        await fileStore[attachment.DiskName].DeleteByIdAsync(attachment.ExternalId);

        db.Set().Remove(attachment);
        var savedCount = await db.SaveChangesAsync();

        if (savedCount < 1)
        {
            return TypedResults.Problem(statusCode: 500);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<BadRequest, UnauthorizedHttpResult, Created>> PostAttachment(
        IFormFile file,
        [FromServices] IAttachmentDbContext<ApplicationAttachment> db,
        [FromServices] IAuthContext<ApplicationUser> auth, 
        [FromServices] IFileStore fileStore,
        HttpContext context)
    {
        if (!auth.IsConfirmed || auth.UserId is null)
        {
            return TypedResults.Unauthorized();
        }
        
        var fileInfo = file.ToFileInfo();
        
        var appAttachment = new ApplicationAttachment()
        {
            DiskName = "S3",
            Id = Guid.NewGuid(),
            FileName = fileInfo.FileName,
            Extension = fileInfo.Extension,
            MimeType = fileInfo.MimeType,
            Size = fileInfo.Size,
            ExternalId = Guid.NewGuid().ToString(),
            UserId = auth.UserId,
        };
        
        appAttachment.SetTransient(false);

        await db.Set().AddAsync(appAttachment);

        var savedCount = await db.SaveChangesAsync();

        if (savedCount < 1)
        {
            return TypedResults.BadRequest();
        }

        await fileStore
             .Disk("S3")
             .UploadAsync(fileInfo.ToStream(), appAttachment.Extension, appAttachment.MimeType);

        return TypedResults.Created($"api/attachments/{appAttachment.Id}");
    }

    private static async Task<Results<NotFound, FileStreamHttpResult>> GetAttachmentFileById(
        [FromRoute] Guid id,
        [FromServices] IAttachmentDbContext<ApplicationAttachment> db,
        [FromServices] IFileStore fileStore,
        HttpContext context)
    {
        var attachment = await db.Set().FirstOrDefaultAsync(a => a.Id == id);

        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        var fileStream = await fileStore[attachment.DiskName]
            .GetFileByIdAsync(attachment.ExternalId);

        return TypedResults.File(fileStream, attachment.MimeType); 
    }

    public class AttachmentInfoResponse
    {
        public Guid Id { get; set; }
        public long Size { get; set; }
        public string FileName { get; set; } = default!;
        public string Extension { get; set; } = default!;
        public string MimeType { get; set; } = default!;
    }

    private static async Task<Results<NotFound, Ok<AttachmentInfoResponse>>> GetAttachmentById(
        [FromRoute] Guid id,
        [FromServices] IAttachmentDbContext<ApplicationAttachment> db,
        HttpContext context)
    {
        var attachment = await db.Set().FirstOrDefaultAsync(a => a.Id == id);

        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        var response = new AttachmentInfoResponse()
        {
            Id = id,
            Size = attachment.Size,
            FileName = attachment.FileName,
            Extension = attachment.Extension,
            MimeType = attachment.MimeType ?? attachment.Extension
        };

        return TypedResults.Ok(response);
    }
}