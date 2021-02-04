using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using UserIdentityEx.BLL;
using UserIdentityEx.Models;
using UserIdentityEx.ViewModels;

namespace UserIdentityEx.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _usermanager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _memoryCache;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache)
        {
            _usermanager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _memoryCache = memoryCache;
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
                if(user != null)
                {
                    IdentityResult result = await _usermanager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ShowList");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
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

            //UserBLL userBLL = new UserBLL();
            //userBLL.PrintUserProp(userView);

            var userDB = await _usermanager.FindByIdAsync(userView.userId);
            if (userDB != null)
            {
                userDB.UserName = userView.Name;
                userDB.Email = userView.Email;
                userDB.Age = userView.Age;
                userDB.Code = userView.Code;
                await _usermanager.UpdateAsync(userDB);
                _memoryCache.Set(userDB.Id, userDB, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                return RedirectToAction("ShowList");
            }
            return View("Error", "Error! User NOT Found.");
        }

        [HttpGet]
        [ResponseCache(Location =ResponseCacheLocation.Any, Duration =300)]
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
        //[Authorize]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            //if (User.Identity.IsAuthenticated)
            //{
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

                    if (await _usermanager.FindByEmailAsync(model.Email) != null)
                    {
                        return View("Error", "Error! Email is also registered.");
                    }

                    var result = await _usermanager.CreateAsync(user, model.Password);
                    _memoryCache.Set(user.Id, user, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("ShowList", "Account");
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
            //}
            //else
            //{
                //return Content("User doesn't Authentificated. Login first.");
            //}
        }
    }
}
