// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Common.Domain.Interfaces;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }

    public DateTimeOffset DeletedAt { get; set; }
}
