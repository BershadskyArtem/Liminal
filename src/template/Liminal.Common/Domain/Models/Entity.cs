// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Common.Domain.Models;

public abstract class Entity
{
    public virtual Guid Id { get; set; }
}
