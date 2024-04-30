using System.ComponentModel.DataAnnotations;

namespace DTS.Web.Areas.User.ViewModels;

public class ChangePasswordVm
{
    [Required]
    public string OldPassword { get; set; }
    
    [Required]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }
}