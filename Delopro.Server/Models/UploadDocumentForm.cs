namespace Delopro.Server.Models
{
    public class UploadDocumentForm
    {
        public List<IFormFile>? Files { get; set; }  
        public string? FolderName { get; set; }
    }
}
