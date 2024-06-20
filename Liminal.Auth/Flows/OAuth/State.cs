namespace Liminal.Auth.Flows.OAuth;

public class State
{
    public string Provider { get; init; } = default!;
    public string RedirectAfter { get; init; } = default!;
    public string FlowState { get; init; } = default!;
    public string SiteUrl { get; init; } = default!;
    public string RedirectUrl { get; init; } = default!;
}