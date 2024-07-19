// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Abstractions;

public interface IDisk
{
    public string Name { get; }

    public Task UploadAsync(Stream stream, string key, string contentType);

    public Task DeleteByIdAsync(string id);

    public Task<Stream> GetFileByIdAsync(string id);

    public Task<string> GetPublicLinkByIdAsync(string id);

    public Task<string> GetPublicSignedLinkByIdAsync(string id, TimeSpan validFor);
}
