// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Mail.Implementations;

// ReSharper disable once ClassNeverInstantiated.Global
public class ConsoleMailer : AbstractMailer
{
    public override Task<bool> SendEmailAsync(string email, string content)
    {
        Console.WriteLine($"Sent email to {email}. Date(Local): {DateTimeOffset.Now}. Content: {content}. ");
        return Task.FromResult(true);
    }
}
