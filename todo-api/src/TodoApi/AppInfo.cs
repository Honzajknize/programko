namespace TodoApi;

/// <summary>
/// Informace o běžící aplikaci.
/// </summary>
public static class AppInfo
{
    /// <summary>
    /// Verze aplikace, čtená z proměnné prostředí APP_VERSION.
    /// </summary>
    public static string Version => Environment.GetEnvironmentVariable("APP_VERSION")!.Trim();
}
