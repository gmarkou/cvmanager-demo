

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace CVManager.Services;

public class FileManager: IFileManager
{
    private readonly string _rootDirectory;
    private readonly string _requestPath;
    private static readonly List<string> SupportedExtensions = new List<string>
    {
        ".pdf", ".doc", ".docx", ".odt", ".png", ".jpg", ".jpeg", ".svg"
    };

    public FileManager(IConfiguration configuration)
    {
        var config = new FileManagerConfig();
        configuration.GetSection(FileManagerConfig.Config).Bind(config);
        _rootDirectory = Path.Join(configuration.GetValue<string>(WebHostDefaults.ContentRootKey), config.Path);
        _requestPath = config.RequestPath;
        if (!_requestPath.EndsWith('/'))
        {
            _requestPath += '/';
        }
        EnsureDirectory(_rootDirectory);
    }
    private void EnsureDirectory(string directory)
    {
        Directory.CreateDirectory(directory);
    }

    public async Task<IFileManager.UploadedFile> UploadFile(IFormFile formFile)
    {
        var extension = Path.GetExtension(formFile.FileName).ToLower();
        if (SupportedExtensions.All(s => s != extension))
        {
            extension = ".blob";
        }

        var filename = Guid.NewGuid();

        var dir = Path.Join(_rootDirectory, "uploads");
        EnsureDirectory(dir);
        if (Path.EndsInDirectorySeparator(dir) == false)
        {
            dir += Path.DirectorySeparatorChar;
        }

        var newFileName = dir + filename + extension;
        
        await using (var stream = new FileStream(newFileName, FileMode.Create))
        {
            await formFile.CopyToAsync(stream);
        }

        return new IFileManager.UploadedFile
        {
            Extension = extension,
            ContentType = formFile.ContentType,
            FileName = newFileName,
            FileSize = formFile.Length
        };
    }

    public IFileManager.File MoveFile(int fileId, IFileManager.UploadedFile uploadedFile)
    {
        var extension = uploadedFile.Extension;
        var relativeDir = (fileId / 100).ToString();
        var dir = Path.Join(_rootDirectory, relativeDir);
        EnsureDirectory(dir);
        if (Path.EndsInDirectorySeparator(dir) == false)
        {
            dir += Path.DirectorySeparatorChar;
        }
        var newFileName = dir + (fileId % 100).ToString() + extension;
        File.Move(uploadedFile.FileName, newFileName);
        
        return new IFileManager.File
        {
            Extension = extension,
            ContentType = uploadedFile.ContentType,
            FileSize = uploadedFile.FileSize,
            Url = _requestPath + relativeDir + '/' + (fileId % 100).ToString() + extension
        };
    }

    public void RemoveFile(int fileId, string extension)
    {
        var relativeDir = (fileId / 100).ToString();
        var dir = Path.Join(_rootDirectory, relativeDir);
        EnsureDirectory(dir);
        if (Path.EndsInDirectorySeparator(dir) == false)
        {
            dir += Path.DirectorySeparatorChar;
        }
        File.Delete(dir + (fileId % 100).ToString() + extension);
    }

    public string? GetFileName(int fileId, string extension)
    {
        var relativeDir = (fileId / 100).ToString();
        var dir = Path.Join(_rootDirectory, relativeDir);
        EnsureDirectory(dir);
        if (Path.EndsInDirectorySeparator(dir) == false)
        {
            dir += Path.DirectorySeparatorChar;
        }

        return dir + (fileId % 100).ToString() + extension;
    }


}