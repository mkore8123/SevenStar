using Common.Enums;

namespace SevenStar.Data.Register;

public static class AssemblyMapping
{
    public static readonly string BaseAssemblyName = "SevenStar.Shared.Domain";

    public static readonly Dictionary<DataSource, string> PlatformAssemblies = new()
    {
        { DataSource.MySql, "SevenStar.Data.Platform.MySql" },
        { DataSource.PostgreSql, "SevenStar.Data.Platform.PostgreSql" }
        // 根據實際情況添加其他映射
    };

    public static readonly Dictionary<DataSource, string> BackendAssemblies = new()
    {
        { DataSource.MySql, "SevenStar.Data.Backend.MySql" },
        { DataSource.PostgreSql, "SevenStar.Data.Backend.PostgreSql" }
        // 根據實際情況添加其他映射
    };

    public static readonly Dictionary<DataSource, string> CompanyAssemblies = new()
    {
        { DataSource.MySql, "SevenStar.Data.Company.MySql" },
        { DataSource.PostgreSql, "SevenStar.Data.Company.PostgreSql" }
        // 根據實際情況添加其他映射
    };
}
