
namespace CVManager.DTO;

public class CvCreateUpdateDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public IEnumerable<DegreeDto>? Degrees { get; set; }
    public int DegreeId { get; set; }
    public IFormFile? File { get; set; }
    public string DeleteFile { get; set; } = string.Empty;
}