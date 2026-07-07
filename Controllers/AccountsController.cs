using EshopperMCV.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EshopperMCV.Controllers
{
    public class AccountsController : Controller
    {
        private EShopperDBContext dbContext = new EShopperDBContext();

        //   LOGIN  
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string hashPassword = GetMD5(model.UserPassword);

            // 1. Kiểm tra user + password
            var user = dbContext.Users.FirstOrDefault(u =>
                u.UserName.ToLower() == model.UserName.ToLower() &&
                u.UserPassword == hashPassword
            );

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(model);
            }

            // 2. Lấy role của user
            var userRole = dbContext.UserRolesMappings
                .Include(x => x.RoleMaster)
                .FirstOrDefault(x => x.UserID == user.ID);

            if (userRole == null)
            {
                ModelState.AddModelError("", "User has no role assigned");
                return View(model);
            }

            string roleName = userRole.RoleMaster.RollName;

            // 3. Tạo ticket + cookie
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                user.UserName,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                roleName
            );

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            HttpCookie authCookie = new HttpCookie(
                FormsAuthentication.FormsCookieName,
                encryptedTicket
            );

            Response.Cookies.Add(authCookie);

            // 4. Redirect theo role
            if (roleName == "Admin")
            {
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Product");
        }

        // SIGNUP
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.UserPassword = GetMD5(model.UserPassword);
            dbContext.Users.Add(model);
            dbContext.SaveChanges();

            // GÁN ROLE MẶC ĐỊNH = Customer
            var role = dbContext.RoleMasters.First(r => r.RollName == "Customer");

            dbContext.UserRolesMappings.Add(new UserRolesMapping
            {
                UserID = model.ID,
                RoleID = role.ID
            });

            dbContext.SaveChanges();

            return RedirectToAction("Login");
        }

        //  LOGOUT  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Accounts");
        }


        //   HASH  
        public static string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = md5.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2")); // IN HOA
                }
                return sb.ToString();
            }
        }
    }
}
