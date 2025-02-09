using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SyncSyntax.Models.ViewModels;

namespace SyncSyntax.Controllers
{
    public class AuthController:Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public  IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInModel)
        {
           
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(signInModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    return View(signInModel);
                }
                var signInResult = await _signInManager.PasswordSignInAsync(user, signInModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    return View(signInModel);
                }

                return RedirectToAction("Index","Post");
            }
            return View(signInModel);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Post");
        }

    }
}
