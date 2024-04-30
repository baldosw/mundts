using System.Security.Claims;
using DTS.DataAccess;
using DTS.Web.Areas.User.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTS.Web.Controllers;

[Area("User")]
[Authorize]
public class ChangePasswordController : Controller
{
   
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _dbContext;


    
    public ChangePasswordController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["PasswordChangedSuccess"] = false;
        ViewData["WithErrors"] = false;
        
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee =  await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();

        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        ViewData["employeeId"] = employee.Id;
         
        if (HttpContext.Items.ContainsKey("Received"))
        {
            var receivedMiddlewareValue = HttpContext.Items["Received"];
            var forwardedMiddlewareValue = HttpContext.Items["Forwarded"];
            var completedMiddlewareValue = HttpContext.Items["Completed"];
            var incomingMiddlewareValue = HttpContext.Items["Incoming"];
            
            ViewData["Received"] = receivedMiddlewareValue;
            ViewData["Forwarded"] = forwardedMiddlewareValue;
            ViewData["Completed"] = completedMiddlewareValue;
            ViewData["Incoming"] = incomingMiddlewareValue;
        }
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(ChangePasswordVm changePasswordVm)
    {
        ViewData["PasswordChangedSuccess"] = false;
        ViewData["WithErrors"] = false;
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }
 
        var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordVm.OldPassword, changePasswordVm.ConfirmNewPassword);
        if (!changePasswordResult.Succeeded)
        {
            ViewData["WithErrors"] = true;
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
 
        ViewData["PasswordChangedSuccess"] = true;
        await _signInManager.RefreshSignInAsync(user);
        return View();
    }
    
    
}