namespace Delopro.Server.Models
{
    public class DocumentUploadForm
    {
        public List<IFormFile>? Files { get; set; }  
        public string? FolderName { get; set; }
    }
}
