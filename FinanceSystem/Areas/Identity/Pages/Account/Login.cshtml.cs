// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using FinanceSystem.Data;
using FinanceSystem.Models;
using FinanceSystem.Areas.Identity.Data;

namespace FinanceSystem.Areas.Identity.Pages.Account
{
    public class Login : PageModel
    {
        private readonly SignInManager<FinanceSystemUser> _signInManager;
        private readonly ILogger<Login> _loggerLogin;
        private readonly UserManager<FinanceSystemUser> _userManager;
        private readonly IUserStore<FinanceSystemUser> _userStore;
        private readonly IUserEmailStore<FinanceSystemUser> _emailStore;
        private readonly ILogger<Login> _loggerRegister;
        private readonly IEmailSender _emailSender;
        private readonly FinanceSystemDbContext _context;
        public Login(SignInManager<FinanceSystemUser> signInManager, ILogger<Login> loggerLogin, 
            UserManager<FinanceSystemUser> userManager,
            IUserStore<FinanceSystemUser> userStore,            
            ILogger<Login> loggerRegister,
            IEmailSender emailSender,
             FinanceSystemDbContext context)
        {
            _signInManager = signInManager;
            _loggerLogin = loggerLogin;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _loggerRegister = loggerRegister;
            _emailSender = emailSender;
            _context = context;
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            //register
            [Required]
            [EmailAddress]
            [Display(Name = "Email Register")]
            public string EmailRegister { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password Register")]
            public string PasswordRegister { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("PasswordRegister", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!string.IsNullOrEmpty(Input.EmailRegister) && !string.IsNullOrEmpty(Input.PasswordRegister) && !string.IsNullOrEmpty(Input.ConfirmPassword))
            {
                ModelState.Remove(nameof(InputModel.Email));
                ModelState.Remove(nameof(InputModel.Password));
                if (ModelState.IsValid)
                {
                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, Input.EmailRegister, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.EmailRegister, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.PasswordRegister);

                    if (result.Succeeded)
                    {
                        
                        _loggerRegister.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        UserInfor userInfor = new UserInfor();
                        userInfor.Id = userId;
                        _context.UserInfors.Add(userInfor);
                        
                        Wallet wallet = new Wallet();        
                        wallet.UserId = userId;
                        _context.Wallets.Add(wallet);
                        _context.SaveChanges();

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.EmailRegister, returnUrl = returnUrl });
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
            }
            else if (!string.IsNullOrEmpty(Input.Email) && !string.IsNullOrEmpty(Input.Password))
            {
                ModelState.Remove("Input.EmailRegister");
                ModelState.Remove("Input.PasswordRegister");
                ModelState.Remove("Input.ConfirmPassword");

                if (ModelState.IsValid)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                       
                        _loggerLogin.LogInformation("User logged in.");
                        
                        return LocalRedirect(returnUrl);

                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _loggerLogin.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        private FinanceSystemUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<FinanceSystemUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(FinanceSystemUser)}'. " +
                    $"Ensure that '{nameof(FinanceSystemUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<FinanceSystemUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<FinanceSystemUser>)_userStore;
        }
    }
}
