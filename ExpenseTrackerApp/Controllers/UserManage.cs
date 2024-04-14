using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class UserManage : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserManage(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            SignUpviewModel signUpviewModel = new SignUpviewModel();
            return View(signUpviewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpviewModel signUpviewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = signUpviewModel.Email, Email = signUpviewModel.Email, ApplicationUserName = signUpviewModel.Name, registeredSince = DateTime.Today };
                var result = await _userManager.CreateAsync(user, signUpviewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            SignInviewModel signInviewModel = new SignInviewModel();
            return View(signInviewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInviewModel signInviewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(signInviewModel.Email, signInviewModel.Password, signInviewModel.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Fehler");
            }
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Home", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SettingsProfile()
        {
            var currentUser = (ClaimsIdentity)User.Identity;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = _context.applicationUsers.FirstOrDefault(x => x.Id == currentUserId);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> SettingsProfile(ApplicationUser user)
        {

            if (user.Id == null || user.Id == string.Empty)
            {
                return BadRequest($"{user.UserName} in invalid shape");
            }

            var db_user = await _context.applicationUsers.FirstOrDefaultAsync(e => e.Id == user.Id);

            if (db_user == null)
            {
                return NotFound("ship");
            }

            db_user.ApplicationUserName = user.ApplicationUserName;
            db_user.Email = user.Email;
            db_user.registeredSince = user.registeredSince;

            await _context.SaveChangesAsync();

            return View(db_user);
        }

        public IActionResult Profile()
        {
            var currentUser = (ClaimsIdentity)User.Identity;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;


            var user = _context.applicationUsers.FirstOrDefault(u => u.Id == currentUserId);
            if(user.ApplicationUserName == "Max")
            {
                return View(user);
            }
            return View();
        }
    }
}
