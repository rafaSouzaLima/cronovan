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
using webapp.Models;

namespace webapp.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null) {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        if (User.IsInRole("Administrador")) {
            returnUrl = Url.Page("/Gerentes/Index", new { area = "Administrador" });
        } else if (User.IsInRole("Gerente")) {
            returnUrl = Url.Page("/Estudantes/Index", new { area = "Gerente"});
        } else if (User.IsInRole("Estudante")) {
            returnUrl = Url.Page("/Cronograma/Index", new { area = "Estudante"});
        } else if (User.IsInRole("Responsavel")) {
            returnUrl = Url.Page("/Index", new { area = "Responsavel"});
        } else if (User.IsInRole("Motorista")) {
            returnUrl = Url.Page("/ListaPresenca/Index", new { area = "Motorista"});
        }

        _logger.LogInformation("User logged in.");
        return LocalRedirect(returnUrl);
    }
}
