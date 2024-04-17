using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DTS.Web.Areas.User.ViewModels;

public class DocumentVm
{
    [ValidateNever]
    public IEnumerable<SelectListItem> RequestTypes { get; set; }
    
    [ValidateNever]
    public IEnumerable<SelectListItem> Departments { get; set; }

    public int Id { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string TrackingCode { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string Title { get; set; }

    [Required]
    [StringLength(400, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
    public string Content { get; set; }
    
    [ValidateNever]
    public string RequestType { get; set; }
   
    public int? RequestTypeId { get; set; }

    [ValidateNever]
    public string  Department { get; set; }

     
    public int? DepartmentId { get; set; }

    public string Remarks { get; set; }

    public int CreatedById { get; set; }

    public DateTime CreatedDate { get; set; }

    public long CreatedTimestamp { get; set; }

    public int DocumentId { get; set; }

    [ValidateNever]
    public string DepartmentName { get; set; }

    [ValidateNever]
    public string RequestTypeTitle { get; set; }

    public int? RouteDepartmentId { get; set; }
 
}