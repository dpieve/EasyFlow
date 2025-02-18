namespace EasyFlow.Domain.Entities;

public static class Paths
{
    public static readonly string DbName = "EasyFlow.ds";
    public static readonly string BasePath = GetBasePath();
    public static readonly string DbFullPath = Path.Combine(BasePath, DbName);

    private static string GetBasePath()
    {
        string basePath = "";

        if (OperatingSystem.IsWindows())
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        else if (OperatingSystem.IsLinux())
        {
            basePath = Environment.GetEnvironmentVariable("HOME")!;
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported operating system.");
        }

        return basePath;
    }
}