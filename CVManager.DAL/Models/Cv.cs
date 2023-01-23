using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVManager.DAL.Models;

public class Cv
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Last Name is required.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Last Name is 60 characters.")]
    public string LastName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "First Name is required.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the First Name is 60 characters.")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    
    public int? DegreeId { get; set; }
    public Degree? Degree { get; set; }

    public long? FileSize { get; set; }
    public string? FileExtension { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime DateCreated { get; set; }
}