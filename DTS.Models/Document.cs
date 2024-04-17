using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DTS.Models;

public class Document
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DisplayName("Tracking Code")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string TrackingCode { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string Title { get; set; }

    [Required]
    [StringLength(400, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string Content { get; set; }
 
    [Display(Name = "Created By")]
    public int CreatedBy { get; set; }
    
    [Display(Name = "Modified By")]
    public int ModifiedBy { get; set; }
 
    [ValidateNever]
    public RequestType RequestType { get; set; }

    public int RequestTypeId { get; set; }

    public string? Remarks { get; set; }

    [ValidateNever]
    public Department? Department { get; set; }
    
    //The source of the document
    public int? DepartmentId { get; set; }

    //Department to the forwarded department
    public int? RouteDepartmentId { get; set; }

    public int? StatusId { get; set; }

    [ValidateNever]
    public Status Status { get; set; }
     
    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }
 
}