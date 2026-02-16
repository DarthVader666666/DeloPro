using Delopro.Bll.Interfaces;

namespace Delopro.Bll.Services
{
    public class LocalDriveService : IDriveService
    {
        public void CreateFile(string? filePath)
        {
        }

        public void CreateFolder(string folderPath)
        {
        }

        public void Delete(string? path, bool isFolder = false)
        {
        }

        public Task DownloadFolderContentsAsync(string? folderId, string? localPath)
        {
            return Task.CompletedTask;
        }

        public void Rename(string? path, string? newName, bool isFolder = false)
        {
        }

        public void RestoreAllDocuments()
        {
        }
    }
}
