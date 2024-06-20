using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Milky.DataAccess.Data;
using Milky.Models.ViewModels;
using MilkyWeb.Areas.Identity.Pages.Account;
using Milky.Utility;
using System.Text;
using System.Text.Encodings.Web;
using Newtonsoft.Json;
using Azure.Identity;


namespace MilkyWeb.Areas.Identity.Pages.Account
{
    public class NextStepModel : PageModel
    {

        //public RegisterModel.InputModel Input { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        private readonly ApplicationDbContext _context;

        public ApplicationUser ApplicationUser { get; set; }

        public RegisterVM RegisterVM { get; set; }

        public string UserRole { get; set; }

        public InputModel Input1 { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<NextStepModel> _logger;
        private readonly IEmailSender _emailSender;

        public class InputModel
        {
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

            [Required]
            public string? FirstName { get; set; }

            [Required]
            public string? LastName { get; set; }

            [Required]
            public string? City { get; set; }

            [Required]
            public string? Street { get; set; }

            [Required]
            public string? State { get; set; }

            [Required]
            public string? Address { get; set; }

            [Required]
            public string? PostalCode { get; set; }

            [Required]

            public string? PhoneNumber { get; set; }

            public string? Role { get; set; }

        }

        public NextStepModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<NextStepModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;

            RegisterVM = new RegisterVM()
            {
                CityList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Kollam", Text = "Kollam" },
                    new SelectListItem { Value = "Punalur", Text = "Punalur" },
                    new SelectListItem { Value = "Chavara", Text = "Chavara" },
                    new SelectListItem { Value = "Mayyanad", Text = "Mayyanad" },
                    new SelectListItem { Value = "Paravur", Text = "Paravur" },
                    new SelectListItem { Value = "Kundara", Text = "Kundara" },
                    new SelectListItem { Value = "Kottarakkara", Text = "Kottarakkara" },
                    new SelectListItem { Value = "Ochira", Text = "Ochira" },
                    new SelectListItem { Value = "Chathannoor", Text = "Chathannoor" },
                    new SelectListItem { Value = "Karunagappally", Text = "Karunagappally" },
                }
            };
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/");

            var userData = TempData["UserData"] as string;
            TempData.Keep("UserData");

            if (!string.IsNullOrEmpty(userData))
            {
                return Page();
            }
            else
            {
                _logger.LogWarning("TempData['UserData'] is null or empty.");
                return LocalRedirect(returnUrl);
            }

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            _logger.LogInformation($"Input.City: {Input.City}, {Input.FirstName}");
            TempData.Keep("CreatedUser");

            var userData = TempData["UserData"] as string;
            if (!string.IsNullOrEmpty(userData))
            {
                Input1 = JsonConvert.DeserializeObject<NextStepModel.InputModel>(userData);

                // Log the data
                _logger.LogInformation($"User data retrieved: Email - {Input.Email}, Password - {Input.Password}, Role - {Input.Role}, ...");
            }
            else
            {
                // Handle the case where TempData["UserData"] is null or empty
                _logger.LogWarning("TempData['UserData'] is null or empty.");
            }

            UserRole = TempData["UserRole"]?.ToString();

            // Log the City value
            _logger.LogInformation($"Input.City: {Input.City}, {Input.FirstName}");

            if (Input1 != null && !string.IsNullOrEmpty(Input1.Email))
            {

                // Remove validation errors for email and password
                ModelState.Remove("Email");
                ModelState.Remove("Password");
                ModelState.Remove("Input.Email");
                ModelState.Remove("Input.Password");
                ModelState.Remove("Input.PostalCode");
                ModelState.Remove("Input.ConfirmPassword");

                if (!string.IsNullOrEmpty(Input.PostalCode))
                {
                    if (Input.PostalCode.Length != 6)
                    {
                        ModelState.AddModelError(nameof(Input.PostalCode), "Postal Code must be exactly 6 characters.");
                        return Page();
                    }
                    else if (!Input.PostalCode.All(char.IsDigit))
                    {
                        ModelState.AddModelError(nameof(Input.PostalCode), "Postal Code must contain only numeric digits.");
                    }
                }


                
                if (ModelState.IsValid)
                {
                    ApplicationUser = new ApplicationUser();
                    if (!string.IsNullOrEmpty(Input.PhoneNumber))
                    {
                        // Remove non-digit characters
                        string cleanedPhoneNumber = new string(Input.PhoneNumber.Where(char.IsDigit).ToArray());

                        // Ensure only 10 digits are kept
                        if (cleanedPhoneNumber.Length == 10)
                        {
                            // Prepend the country code (91)
                            ApplicationUser.PhoneNumber = "91" + cleanedPhoneNumber;
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(Input.PhoneNumber), "Phone number must contain exactly 10 digits.");
                            return Page();
                        }
                    }

                    //ApplicationUser = new ApplicationUser();
                    ApplicationUser.UserName = Input1.Email;
                    ApplicationUser.Email = Input1.Email;
                    // Save additional details to the database
                    ApplicationUser.Name = $"{Input.FirstName} {Input.LastName}";
                    ApplicationUser.City = Input.City;
                    ApplicationUser.Street = Input.Street;
                    ApplicationUser.State = Input.State;
                    ApplicationUser.Address = Input.Address;
                    ApplicationUser.PostalCode = Input.PostalCode;

                    var result = await _userManager.CreateAsync(ApplicationUser, Input1.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        await _userManager.AddToRoleAsync(ApplicationUser, UserRole);


                        var userId = await _userManager.GetUserIdAsync(ApplicationUser);

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(ApplicationUser);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(ApplicationUser.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                        if (_userManager.Options.SignIn.RequireConfirmedAccount && !User.IsInRole(SD.Role_Admin))
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input1.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            if (User.IsInRole(SD.Role_Admin))
                            {
                                TempData["success"] = "New User Created Successfully";
                            }
                            else
                            {
                                await _signInManager.SignInAsync(ApplicationUser, isPersistent: false);
                            }
                            return LocalRedirect(returnUrl);
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError($"ModelState error: {error.ErrorMessage}");
                    }
                }
                return Page();

            }
            else
            {
                _logger.LogError("ModelState is not valid.");
                // If ModelState is not valid, redisplay the form
                return Page();
            }
        }

    }

}
