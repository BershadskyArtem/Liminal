// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Abstractions;

/// <summary>
/// Marker interface that marks auth flow implementation (Password, MagicLink, etc.)
/// </summary>
public interface IAuthFlow
{
    public string Name { get; }
}
