using Microsoft.Build.Framework;

namespace DTS.Web.Areas.User.ViewModels;

public class DocumentStatusVm
{
    public int EmployeeId { get; set; }

    public int DocumentId { get; set; }
    
    public string Remarks { get; set; }

    [Required]
    public int RouteDepartmentId { get; set; }

}