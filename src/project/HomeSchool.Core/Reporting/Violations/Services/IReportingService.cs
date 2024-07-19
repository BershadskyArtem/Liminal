// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ErrorOr;
using HomeSchool.Core.Reporting.Violations.Enums;

namespace HomeSchool.Core.Reporting.Violations.Services;

public interface IReportingService
{
    public Task<ErrorOr<Guid>> ReportUserAsync(Guid userId, string text, ReportSeverity severity, Guid? authorUserId);

    public Task<ErrorOr<Guid>> ReportIssueAsync(string text, ReportSeverity severity, Guid? authorUserId);

    public Task<ErrorOr<bool>> SetResolveAsync(Guid reportId, Guid resolverUserId, string resolveMessage, bool resolve, bool sendEmail = false);

    public Task<ErrorOr<bool>> DeleteAsync(Guid reportId);
}
