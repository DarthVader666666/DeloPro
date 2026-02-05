using Microsoft.Extensions.Configuration;

namespace Delopro.Bll
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;
        public static string? WebRootPath;
        public static string? DocsPath;
        public static string? DocsFolderName;
        public static string? DocsFolderId;
        public static string? ChapterImagesPath;
        public static string? AvatarsPath;

        public static void Initialize(IConfiguration configuration, string webRootPath, string environmentName)
        {
            Configuration = configuration;
            DocsFolderName = configuration["DocsFolderName"];
            WebRootPath = webRootPath;
            DocsPath = Path.Combine(webRootPath, DocsFolderName ?? string.Empty);
            DocsFolderId = Configuration["GoogleDrive:FolderId"];
            ChapterImagesPath = environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase)
                ? Path.GetFullPath("../delopro.client/src/assets/chapters/")
                : Path.Combine(WebRootPath, "avatars");
            AvatarsPath = environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase)
                ? Path.GetFullPath("../delopro.client/src/assets/avatars/")
                : Path.Combine(WebRootPath, "avatars");
        }
    }
}