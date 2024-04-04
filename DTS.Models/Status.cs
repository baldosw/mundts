using System.ComponentModel.DataAnnotations;

namespace DTS.Models;

public class Status
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string Title { get; set; }
    
    [Required]
    [StringLength(500, ErrorMessage = "The {0} value cannot exceed {1} characters ")]
    public string? Description { get; set; }
}