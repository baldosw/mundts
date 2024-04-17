// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using DTS.DataAccess;
using DTS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DTS.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly Employee _employee;
        private readonly ApplicationDbContext _dbContext;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _dbContext = dbContext;

        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }
        
        public List<SelectListItem> InputDepartments { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
           

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public int Id { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
            public string MiddleName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
            public string Position { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
            public string ContactNumber { get; set; }

            [Required]
            [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email address")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Required]
            [StringLength(400, MinimumLength = 2, ErrorMessage = "The {0} value cannot exceed {1} characters and should not be less than {2} characters ")]
            [DataType(DataType.Text)]
            public string Address { get; set; }
 
            [Required]
            [DataType(DataType.PostalCode)]
            public int DepartmentId { get; set; }

           
            [ValidateNever]
            public IEnumerable<SelectListItem> Departments { get; set; }  
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            InputDepartments = await _dbContext.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            InputDepartments = await _dbContext.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToListAsync();
            
            
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                Employee employee = new Employee();
                employee.FirstName = Input.FirstName;
                employee.MiddleName = Input.MiddleName;
                employee.LastName = Input.LastName;
                employee.Address = Input.Address;
                employee.ContactNumber = Input.ContactNumber;
                employee.Position = Input.Position;
                employee.Email = Input.Email;
                employee.DepartmentId = Input.DepartmentId;
                employee.UserId = user.Id;

                await _dbContext.Employees.AddAsync(employee);
                await _dbContext.SaveChangesAsync();

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                     
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        pageHandler: null,
                        values: new {  userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme,
                        pageName: "ConfirmEmail");

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    _userManager.Options.SignIn.RequireConfirmedAccount = true;
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return Redirect($"/registerconfirmation?email={Input.Email}&returnUrl={returnUrl}");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
