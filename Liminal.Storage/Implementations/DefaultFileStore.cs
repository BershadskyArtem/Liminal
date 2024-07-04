using Liminal.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Storage.Implementations;

public class DefaultFileStore(IServiceProvider services) : IFileStore
{
    public IDisk Disk(string diskName)
    {
        return services.GetRequiredKeyedService<IDisk>(diskName);
    }

    public IDisk this[string diskName] => Disk(diskName);
}