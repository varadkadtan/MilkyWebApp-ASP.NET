// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Milky.Models;
using Microsoft.AspNetCore.Http;

namespace MilkyWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _sender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender, SignInManager<ApplicationUser> signInManager , IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _sender = sender;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            ViewData["Email"] = email;

            // If email is not confirmed, show the register confirmation page
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Page();
            }


            // If email is confirmed, automatically sign in the user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect the user to the index page or the returnUrl
            return LocalRedirect(returnUrl);

            //// Once you add a real email sender, you should remove this code that lets you confirm the account
            //DisplayConfirmAccountLink = true;
            //if (DisplayConfirmAccountLink)
            //{
            //    var userId = await _userManager.GetUserIdAsync(user);
            //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //    EmailConfirmationUrl = Url.Page(
            //        "/Account/ConfirmEmail",
            //        pageHandler: null,
            //        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
            //        protocol: Request.Scheme);
            //}

            // If the email is not confirmed, show the register confirmation page

            //ViewData["Email"] = email;

            //return Page();

        }
    }
}
