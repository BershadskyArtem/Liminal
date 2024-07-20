namespace Liminal.Storage.S3;

public static class LiminalStorageBuilderExtensions
{
    public static LiminalStorageBuilder UseS3Disk(
        this LiminalStorageBuilder builder,
        string name,
        Action<S3DiskOptions> configure)
    {
        var options = new S3DiskOptions();

        configure(options);

        return builder.AddDisk(name, options, (sp, driveName) => new DefaultS3Disk(driveName, sp));
    }
}