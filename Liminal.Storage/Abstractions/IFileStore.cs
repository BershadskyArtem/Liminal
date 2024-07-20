namespace Liminal.Storage.Abstractions;

public interface IFileStore
{
    public IDisk Disk(string diskName);

    public IDisk this[string diskName]
    {
        get;
    }
}