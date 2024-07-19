// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Abstractions;

public interface IFileStore
{
    public IDisk Disk(string diskName);

    public IDisk this[string diskName]
    {
        get;
    }
}
