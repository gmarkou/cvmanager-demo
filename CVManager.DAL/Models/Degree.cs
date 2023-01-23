using System.ComponentModel.DataAnnotations;

namespace CVManager.DAL.Models;

public class Degree
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Degree title is required.")]
    [MaxLength(100, ErrorMessage = "Maximum length for the degree title is 100 characters.")]

    public string Title { get; set; } = string.Empty;
}
