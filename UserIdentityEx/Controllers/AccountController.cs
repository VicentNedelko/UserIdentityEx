using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserIdentityEx.Models;
using UserIdentityEx.ViewModels;

namespace UserIdentityEx.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _usermanager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _usermanager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Edit(User user)
        {
            return View(user.Id);
        }
        [HttpGet]
        public async Task<IActionResult> ShowList()
        {
            return View(await _usermanager.Users.ToListAsync());
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Age = model.Age,
                    Email = model.Email,
                    UserName = model.Name,
                    PasswordHash = model.Password,
                    Code = model.Code,
                };
                var result = await _usermanager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
