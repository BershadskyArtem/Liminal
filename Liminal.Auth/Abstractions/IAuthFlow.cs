namespace Liminal.Auth.Abstractions;

/// <summary>
/// Marker interface that marks auth flow implementation (Password, MagicLink, etc.)
/// </summary>
public interface IAuthFlow
{
    public string Name { get; }
}