// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Results;
using Liminal.Storage.Models;

public class UploadResult<TAttachment>
    where TAttachment : FileAttachment
{
    public TAttachment Entry { get; set; }

    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }

    public static UploadResult<TAttachment> Failure(string errorMessage) => new UploadResult<TAttachment>
    {
        IsSuccess = false,
        ErrorMessage = errorMessage,
    };

    public static UploadResult<TAttachment> Success(TAttachment entry) => new UploadResult<TAttachment>
    {
        IsSuccess = true,
        Entry = entry,
    };
}
