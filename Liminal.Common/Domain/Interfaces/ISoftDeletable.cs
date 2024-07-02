namespace Liminal.Common.Domain.Interfaces;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
}