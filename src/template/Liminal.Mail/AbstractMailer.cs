// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Mail;

public abstract class AbstractMailer
{
    public abstract Task<bool> SendEmailAsync(string email, string content);
}
