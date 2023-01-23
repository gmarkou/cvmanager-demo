namespace CVManager.DTO;

public class CvDetailDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public DegreeDto? Degree { get; set; }
    public int DegreeId { get; set; }
    public long? FileSize { get; set; }
    public string? FileExtension { get; set; }

}