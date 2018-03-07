using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SaleAndRentingPortalSql.Data;
using SaleAndRentingPortalSql.Models;
using SaleAndRentingPortalSql.Models.AccountViewModels;
using SaleAndRentingPortalSql.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        private readonly ApplicationDbContext dbContext;





        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            dbContext = context;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email.ToLower(), model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {


                    ApplicationUser user = await _userManager.FindByEmailAsync(model.Email.ToLower());

                    if (!user.IsActive)
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError("Email", "Din bruger er blevet deaktiveret");
                        return View(model);

                    }

                    if (!user.EmailConfirmed)
                    {
                        await _signInManager.SignOutAsync();
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        EmailSender.SendEmail("Email bekræftelse", "Du skal bekræfte din email for at kunne bruge vores service " + @"<a href ='" + callbackUrl + @"'>Tryk her for at bekræfte din mail</a>", "Bekræft venligst din email", model.Email);
                        ModelState.AddModelError("Email", "Din Email er ikke blevet bekræftet");
                        return View(model);

                    }
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                //}
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ugyldigt log ind forsøg");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {

                using (HttpClient client = new HttpClient())
                {

                    var value = new Dictionary<string, string>
                    {
                        { "secret", Staticscs.GoogleRecaptchKey}  ,
                        {"response", model.RecaptchaResponse},
                    };

                    var content = new FormUrlEncodedContent(value);

                    var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

                    var jsonresponse = JObject.Parse(await response.Content.ReadAsStringAsync());


                    if (!bool.Parse(jsonresponse["success"].ToString()))
                    {

                        ModelState.AddModelError("RecaptchaResponse", "Fejl i recaptcha");

                        return View(model);

                    }
                }

                var user = new ApplicationUser { UserName = model.Email.ToLower(), Email = model.Email.ToLower(), FirstName = model.FirstName, LastName = model.LastName, Zipcode = model.ZipCode, Address = model.Address, PhoneNumber = model.PhoneNumber };

                var city = dbContext.ZipCodes.Where(i => i.ZipCode == model.ZipCode).FirstOrDefault();
                if (city == null)
                {
                    ModelState.AddModelError("ZipCode", "Det indtastede postnummer findes ikke");
                    return View(model);
                }


                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Users");
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    EmailSender.SendEmail("Email bekræftelse", "Du skal bekræfte din email for at kunne bruge vores service " + @"<a href ='" + callbackUrl + @"'>Tryk her for at bekræfte din mail</a>", "Bekræft venligst din email", model.Email);


                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToAction("Success");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                EmailSender.SendEmail("Nulstil kodeord", $"Tryk <a href='{callbackUrl}'>her</a> for at nulstille dit kodeord", "Nulstilling af kodeord", model.Email);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public string GetCity(int zipCode)
        {
            var city = dbContext.ZipCodes.Where(i => i.ZipCode == zipCode).FirstOrDefault();

            if (city == null) return null;
            return city.City;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var users = dbContext.Users.Where(i => i.IsActive);
            List<EditUserViewModel> eusers = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                eusers.Add(new EditUserViewModel(user));
            }

            return View(eusers);

        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult DUsers()
        {
            var users = dbContext.Users.Where(i => !i.IsActive);
            List<EditUserViewModel> eusers = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                eusers.Add(new EditUserViewModel(user));
            }

            return View("USERS", eusers);

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edituser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);


            var euser = new EditUserViewModel(user);
            var roles = await _userManager.GetRolesAsync(user);

            euser.Role = roles.FirstOrDefault();

            var allroles = dbContext.Roles.Where(i => i.Name != null);
            euser.AllRoles = new List<SelectListItem>();
            foreach (var r in allroles)
            {
                euser.AllRoles.Add(new SelectListItem() { Text = r.Name, Value = r.Name });
            }


            return View(euser);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edituser(EditUserViewModel euser)
        {
            var user = await _userManager.FindByIdAsync(euser.Id);

            user = euser.MakeApplicationUser(user);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (!role.Equals(euser.Role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);

                await _userManager.AddToRoleAsync(user, euser.Role);
            }

            if (!string.IsNullOrEmpty(euser.NewPassword))
            {

                await _userManager.RemovePasswordAsync(user);

                await _userManager.AddPasswordAsync(user, euser.NewPassword);
            }



            await _userManager.UpdateAsync(user);

            await _userManager.UpdateSecurityStampAsync(user);



            return RedirectToAction("Users");




        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(new EditUserViewModel(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(EditUserViewModel euser)
        {
            var user = await _userManager.FindByIdAsync(euser.Id);
            user.IsActive = false;
            await _userManager.UpdateSecurityStampAsync(user);
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Users");
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(new EditUserViewModel(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(EditUserViewModel euser)
        {
            var user = await _userManager.FindByIdAsync(euser.Id);
            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Users");
        }




        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        //{
        //    // Ensure the user has gone through the username & password screen first
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication user.");
        //    }

        //    var model = new LoginWith2faViewModel { RememberMe = rememberMe };
        //    ViewData["ReturnUrl"] = returnUrl;

        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        //    var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else if (result.IsLockedOut)
        //    {
        //        _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
        //        return RedirectToAction(nameof(Lockout));
        //    }
        //    else
        //    {
        //        _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
        //        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
        //        return View();
        //    }
        //}


        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication user.");
        //    }

        //    var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

        //    var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
        //        return RedirectToAction(nameof(Lockout));
        //    }
        //    else
        //    {
        //        _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
        //        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
        //        return View();
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public IActionResult ExternalLogin(string provider, string returnUrl = null)
        //{
        //    // Request a redirect to the external login provider.
        //    var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return Challenge(properties, provider);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        //{
        //    if (remoteError != null)
        //    {
        //        ErrorMessage = $"Error from external provider: {remoteError}";
        //        return RedirectToAction(nameof(Login));
        //    }
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return RedirectToAction(nameof(Login));
        //    }

        //    // Sign in the user with this external login provider if the user already has a login.
        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        return RedirectToAction(nameof(Lockout));
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then ask the user to create an account.
        //        ViewData["ReturnUrl"] = returnUrl;
        //        ViewData["LoginProvider"] = info.LoginProvider;
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //        return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await _signInManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            throw new ApplicationException("Error loading external login information during confirmation.");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await _userManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await _userManager.AddLoginAsync(user, info);
        //            if (result.Succeeded)
        //            {
        //                await _signInManager.SignInAsync(user, isPersistent: false);
        //                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View(nameof(ExternalLogin), model);
        //}


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }



        #endregion
    }
}
