// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.AspNetCore;
using Microsoft.AspNetCore.Http;

public static class FormFileExtensions
{
    public static FormFileInfo ToFileInfo(this IFormFile file) => new(file);
}
