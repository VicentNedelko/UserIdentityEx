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
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            return View("Delete", user.Id);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _usermanager.FindByIdAsync(id);
            var viewUser = new UserViewModel
            {
                userId = user.Id,
                Email = user.Email,
                Name = user.UserName,
                Age = user.Age,
                Code = user.Code,
                Password = "",
                PasswordConfirm = "",
            };
            return View("Edit", viewUser);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userView)
        {
            var userDB = await _usermanager.FindByIdAsync(userView.userId);
            userDB.Email = userView.Email;
            userDB.Age = userView.Age;
            userDB.Code = userView.Code;
            await _usermanager.UpdateAsync(userDB);
            //userDB.PasswordHash = userView.Password;
            return RedirectToAction("/Account/ShowList");

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
