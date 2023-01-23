namespace CVManager.Services;

public class FileManagerConfig
{
    public const string Config = "FileManagerConfig";

    public string Path { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
}