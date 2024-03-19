using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectASP.Data;
using ProiectASP.Models;
using System.Data;

namespace ProiectASP.Controllers
{
    public class UsersController : Controller
    {

        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;



        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = db.Users.ToList();

            ViewBag.Users = users;

            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var role = db.UserRoles.Where(u => u.UserId == id).First();
            if (role.RoleId == "2c5e174e-3b0e-446f-86af-483d56fd7212")
            {
                ViewBag.Role = "User";
            }
            if (role.RoleId == "2c5e174e-3b0e-446f-86af-483d56fd7211")
            {
                ViewBag.Role = "Editor";
            }
            if (role.RoleId == "2c5e174e-3b0e-446f-86af-483d56fd7210")
            {
                ViewBag.Role = "Admin";
            }
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser requestUser)
        {
            ApplicationUser user = db.Users.Find(id);
            if (ModelState.IsValid)
            {
                user.UserName = requestUser.UserName;
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return View(requestUser);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            if (db.Articles.Where(art => art.UserId == id).Count() == 0)
            {
                db.Users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return RedirectToAction("Index");
            }

        }

        public IActionResult ChangeRole(string idUser, string roleId)
        {
            var user = db.UserRoles.Where(u => u.UserId == idUser).First();
            db.UserRoles.Remove(user);
            db.SaveChanges();
            db.UserRoles.Add(
                new IdentityUserRole<string>
                {
                    RoleId = roleId,
                    UserId = idUser
                });
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
