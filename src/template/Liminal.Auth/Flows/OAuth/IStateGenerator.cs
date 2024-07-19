// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth;

public interface IStateGenerator
{
    public string GenerateState(string provider, string redirectAfter, Guid? linkingTargetId = null);

    public Task<State> ParseState(string state);
}
