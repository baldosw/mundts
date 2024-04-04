using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DTS.Models;

public class Department
{
    public int Id { get; set; }

    [DisplayName(displayName:"Office Name")]
    [Required]
    [StringLength(300, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string Name { get; set; }

    [DisplayName(displayName:"Office Short Name")]
    [StringLength(maximumLength: 50, ErrorMessage = "The {0} value cannot exceed {1} characters    ")]
    public string? ShortName { get; set; }

    [DisplayName(displayName:"Office Description")]
    [StringLength(maximumLength: 400, ErrorMessage = "The {0} value cannot exceed {1} characters    ")]
    public string? Description { get; set; }
}