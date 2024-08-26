namespace EasyFlow.Domain.Entities;
public static class Paths
{
    public static readonly string DbName = "EasyFlow.ds";
    public static readonly string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // TODO: change the base path
    public static readonly string DbFullPath = Path.Combine(BasePath, DbName);
}
