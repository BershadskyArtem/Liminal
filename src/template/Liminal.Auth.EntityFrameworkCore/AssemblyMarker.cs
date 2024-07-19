// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore;
using System.Reflection;

public static class AssemblyMarker
{
    public static Assembly Current => typeof(AssemblyMarker).Assembly;
}
