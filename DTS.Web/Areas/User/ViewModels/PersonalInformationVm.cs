using DTS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DTS.Web.Areas.User.ViewModels;

public class PersonalInformationVm
{

    [ValidateNever]
    public IEnumerable<SelectListItem> Departments { get; set; }
    
    [ValidateNever]
    public Employee Employee { get; set; }
    
    
}