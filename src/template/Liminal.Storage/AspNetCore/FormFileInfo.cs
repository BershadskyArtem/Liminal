// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.AspNetCore;
using Microsoft.AspNetCore.Http;

public class FormFileInfo(IFormFile file)
{
    public string FileName { get; set; } = Path.GetFileNameWithoutExtension(file.FileName);

    public string Extension { get; set; } = Path.GetExtension(file.FileName);

    public string MimeType { get; set; } = file.ContentType;

    public long Size { get; set; } = file.Length;

    public Stream ToStream() => file.OpenReadStream();
}
