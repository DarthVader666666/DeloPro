using Microsoft.Extensions.Configuration;

namespace Delopro.Bll
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;

        public static string? EnvironmentName;
        public static string? WebRootPath;
        public static string? DocsPath;
        public static string? DocsFolderName;
        public static string? DocsFolderId;
        public static string? ChapterImagesPath;
        public static string? AvatarsPath;
        public static string? IconsPath;

        public static void Initialize(IConfiguration configuration, string webRootPath, string environmentName)
        {
            EnvironmentName = environmentName;
            Configuration = configuration;
            DocsFolderName = configuration["DocsFolderName"];
            WebRootPath = webRootPath;

            DocsPath = Path.Combine(webRootPath, DocsFolderName ?? string.Empty);
            DocsFolderId = Configuration["GoogleDrive:FolderId"];

            var imagesRootPath = IsDevelopment ? Path.GetFullPath(configuration["DevRootPath"] ?? string.Empty) : WebRootPath;
            ChapterImagesPath = Path.Combine(imagesRootPath, "chapters");
            AvatarsPath = Path.Combine(imagesRootPath, "avatars");
            IconsPath = Path.Combine(imagesRootPath, "icons");
        }

        public static bool IsDevelopment => EnvironmentName == "Development";
    }
}