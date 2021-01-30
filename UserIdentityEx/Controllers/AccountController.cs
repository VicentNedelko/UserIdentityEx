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
            UserViewModel viewUser = new UserViewModel
            {
                userId = user.Id,
                Name = user.UserName,
                Code = user.Code,
                Age = user.Age,
                Email = user.Email,
            };
            return View("Delete", viewUser);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] string Confirm, string id)
        {
            if(Confirm == "Yes")
            {
                var user = await _usermanager.FindByIdAsync(id);
                await _usermanager.DeleteAsync(user);
            }
            return RedirectToAction("ShowList");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePass(string userId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            UserViewModel viewUser = new UserViewModel
            {
                userId = user.Id,
                Email = user.Email,
                Name = user.UserName,
                Age = user.Age,
                Code = user.Code,
                Password = user.PasswordHash,
            };
            return View("ChangePass", viewUser);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePass([FromForm] string OldPass, [FromForm] string NewPass, string userId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            await _usermanager.ChangePasswordAsync(user, OldPass, NewPass);
            return RedirectToAction("ShowList");
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
        public async Task<IActionResult> Edit(string id, UserViewModel userView)
        {
            userView.userId = id;
            var userDB = await _usermanager.FindByIdAsync(userView.userId);
            userDB.UserName = userView.Name;
            userDB.Email = userView.Email;
            userDB.Age = userView.Age;
            userDB.Code = userView.Code;
            await _usermanager.UpdateAsync(userDB);
            return RedirectToAction("ShowList");
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
