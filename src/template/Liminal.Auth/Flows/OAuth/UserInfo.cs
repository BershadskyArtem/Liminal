// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth;

public class UserInfo
{
    public string Email { get; private set; }

    public string UserName { get; private set; }

    public bool IsVerified { get; private set; }

    private UserInfo(string email, string userName, bool isVerified)
    {
        Email = email;
        UserName = userName;
        IsVerified = isVerified;
    }

    public static UserInfo Create(string email, string userName, bool isVerified)
        => new(email, userName, isVerified);
}
