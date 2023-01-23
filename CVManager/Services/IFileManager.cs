namespace CVManager.Services;


public interface IFileManager
{
    public class File
    {
        public string Extension { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }

    public class UploadedFile
    {
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }

    Task<UploadedFile> UploadFile(IFormFile formFile);
    IFileManager.File MoveFile(int fileId, UploadedFile uploadedFile);

    void RemoveFile(int fileId, string extension);

    string? GetFileName(int fileId, string extension);
}