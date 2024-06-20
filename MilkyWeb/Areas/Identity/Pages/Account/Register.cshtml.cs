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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Milky.DataAccess.Data;
using Milky.Models;
using Milky.Utility;
using MilkyWeb.Areas.Identity.Pages.Account;
using Newtonsoft.Json;

namespace MilkyWeb.Areas.Identity.Pages.Account
{
    
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public string TempDataMessage { get; set; }

        
        public RegisterModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            TempDataMessage = string.Empty; // Initialize TempDataMessage
        }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]

        public InputModel Input { get; set; }

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
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

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

            public string? Role { get; set; }

            [ValidateNever]
            public List<SelectListItem> RoleList { get; set; }

            [TempData]
            public InputModel TempDataInput { get; set; }

            //public string? FirstName { get; set; }

            
            //public string? LastName { get; set; }

            
            //public string? City { get; set; }

            
            //public string? Street { get; set; }

            
            //public string? State { get; set; }

            
            //public string? Address { get; set; }

            
            //public string? PostalCode { get; set; }

        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // Check if the user is already authenticated
            if (_signInManager.IsSignedIn(User))
            {
                //get user information
                var user = await _userManager.GetUserAsync(User);

                if(user != null && await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                {
                    // Admin user is authenticated, allow them to access the register page
                    Input = new InputModel
                    {
                        RoleList = _roleManager.Roles.Select(i => new SelectListItem
                        {
                            Text = i.Name,
                            Value = i.Name
                        }).ToList()
                    };

                    return Page();
                }
                else
                {
                    // User is already logged in, redirect to return URL
                    return LocalRedirect(returnUrl);
                }
            }

            //injecting roles to database if no roles are detected during signup
            //if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            //{
            //    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult(); //create new roles using utility class
            //    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
            //    _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
            //    _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
            //}
            //if (!_roleManager.RoleExistsAsync(SD.Role_Employee).GetAwaiter().GetResult())
            //{
            //    _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
            //}

            Input = new InputModel
            {
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }).ToList()
            };

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Access TempData["UserData"] and deserialize the JSON string
            var userDataString = TempData["UserData"] as string;
            if (!string.IsNullOrEmpty(userDataString))
            {
                Input = JsonConvert.DeserializeObject<InputModel>(userDataString);
            }
            else
            {
                Input = new InputModel();
            }
            return Page(); // Return the page if the user is not already logged in
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                if (!string.IsNullOrEmpty(Input.Role))
                {
                    //await _userManager.AddToRoleAsync(user, Input.Role);
                    TempData["UserRole"] = Input.Role;  // Add role information to TempData
                }
                else
                {
                    //await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    //TempData["UserRole"] = SD.Role_Customer;  // Add default role information to TempData
                    TempData["UserRole"] = "Customer";
                }
                TempData.Keep("UserRole");

                TempData["UserData"] = JsonConvert.SerializeObject(Input); //store user input to temp data
                TempData.Keep("UserData");

                // Store InputModel directly in TempDataInput

                _logger.LogInformation($"TempData[\"UserData\"] content: {TempData["UserData"]}");

                // Redirect to the next step
                return RedirectToPage("./NextStep");
            }

            // If there are validation errors, redisplay the form
            return Page();
        }


        //        var result = await _userManager.CreateAsync(user, Input.Password);

        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User created a new account with password.");

        //            if (!string.IsNullOrEmpty(Input.Role))  //backend for role assigning in register page
        //            {
        //                await _userManager.AddToRoleAsync(user, Input.Role);
        //            }
        //            else
        //            {
        //                await _userManager.AddToRoleAsync(user, SD.Role_Customer);
        //            }


        //            var userId = await _userManager.GetUserIdAsync(user);
        //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //            var callbackUrl = Url.Page(
        //                "/Account/ConfirmEmail",
        //                pageHandler: null,
        //                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
        //                protocol: Request.Scheme);

        //            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
        //                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


        //            if (_userManager.Options.SignIn.RequireConfirmedAccount)
        //            {
        //                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
        //            }
        //            else
        //            {
        //                await _signInManager.SignInAsync(user, isPersistent: false);
        //                ////return LocalRedirect(returnUrl);
        //                //// Redirect to the next step
        //                //return RedirectToPage("./NextStep");
        //                TempData["UserData"] = Input;
        //                return RedirectToPage("/Identity/Pages/Account/NextStep");
        //                //return RedirectToPage("/Identity/Pages/Account/NextStep", new { userId = user.Id });

        //            }
        //        }
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return Page();
        //}

        public ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            var userEmailStore = _userStore as IUserEmailStore<ApplicationUser>;

            if (userEmailStore == null)
            {
                throw new InvalidOperationException("User store is not of the expected type.");
            }

            return userEmailStore;
        }
    }
}
