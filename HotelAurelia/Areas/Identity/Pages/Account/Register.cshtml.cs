// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using HotelAurelia.Areas.Identity.Data;
using HotelAurelia.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
namespace HotelAurelia.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            
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

            [Required]
            public string Nom { get; set; }

            [Required]
            public string Prenom { get; set; }

            [Required]
            public string Tel { get; set; }

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
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // DEBUG: Log the start of the method
            Console.WriteLine("DEBUG: OnPostAsync started");
            Console.WriteLine($"DEBUG: ModelState.IsValid = {ModelState.IsValid}");

            if (ModelState.IsValid)
            {
                Console.WriteLine("DEBUG: ModelState is valid, creating user...");

                // DEBUG: Check Input values
                Console.WriteLine($"DEBUG: Input values - Prenom: '{Input.Prenom}', Nom: '{Input.Nom}', Email: '{Input.Email}', Tel: '{Input.Tel}'");

                var user = new User
                {
                    UserName = $"{Input.Prenom}_{Input.Nom}",
                    Tel = Input.Tel,
                    Email = Input.Email,
                    Nom = Input.Nom,
                    Prenom = Input.Prenom,
                    Role = "Client",
                    EmailConfirmed = true
                };

                Console.WriteLine("DEBUG: User object created, calling CreateAsync...");

                var result = await _userManager.CreateAsync(user, Input.Password);

                Console.WriteLine($"DEBUG: CreateAsync result - Succeeded: {result.Succeeded}");

                if (result.Succeeded)
                {
                    Console.WriteLine("DEBUG: User created successfully!");

                    // Default to "Client" role 
                    await _userManager.AddToRoleAsync(user, "Client");
                    Console.WriteLine("DEBUG: 'Client' role added to user");

                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    Console.WriteLine("DEBUG: User signed in successfully");

                    // DEBUG: Check where we're redirecting
                    Console.WriteLine("DEBUG: Redirecting to ~/Views/Home/Index");

                    // FIX: Your redirect might be incorrect - try these alternatives:
                    // Option 1: Redirect to action
                    // return RedirectToAction("Index", "Home");

                    // Option 2: Return to specific path
                    // return LocalRedirect("/");

                    // Option 3: Use proper redirect
                    return LocalRedirect(returnUrl ?? "/");
                }
                else
                {
                    Console.WriteLine("DEBUG: User creation FAILED!");
                    Console.WriteLine($"DEBUG: Number of errors: {result.Errors.Count()}");

                    foreach (var error in result.Errors)
                    {
                        // DEBUG: Log each error in detail
                        Console.WriteLine($"DEBUG ERROR: Code: {error.Code}, Description: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("DEBUG: ModelState is INVALID!");
                Console.WriteLine($"DEBUG: ModelState error count: {ModelState.ErrorCount}");

                // DEBUG: List all validation errors
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        Console.WriteLine($"DEBUG VALIDATION ERROR for '{key}':");
                        foreach (var error in state.Errors)
                        {
                            Console.WriteLine($"  - {error.ErrorMessage}");
                        }
                    }
                }
            }

            Console.WriteLine("DEBUG: Returning Page() - Something failed");
            Console.WriteLine($"DEBUG: ReturnUrl parameter value: {returnUrl}");

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
