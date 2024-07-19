// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.S3;

public static class LiminalStorageBuilderExtensions
{
    public static LiminalStorageBuilder UseS3Disk(
        this LiminalStorageBuilder builder,
        string name,
        Action<S3DiskOptions> configure)
    {
        var options = new S3DiskOptions();

        configure(options);

        return builder.AddDisk(name, options, (sp, driveName) => new DefaultS3Disk(driveName, sp));
    }
}
