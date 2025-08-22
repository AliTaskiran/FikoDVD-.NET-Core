using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace FikoDVD.Controllers
{
    public class LoginController : Controller
    {
        private readonly AdminManager _adminManager;

        public LoginController()
        {
            _adminManager = new AdminManager(new EfAdminDal());
        }

        private bool VerifyPassword(string storedHash, string password)
        {
            try
            {
                // Stored hash'ten salt'ı ve hash'i ayır
                var parts = storedHash.Split('.');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedHashBytes = Convert.FromBase64String(parts[1]);

                // Girilen şifreyi aynı salt ile hashle
                var hashToCheck = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

                // Hash'leri karşılaştır
                return storedHashBytes.SequenceEqual(hashToCheck);
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Default");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Kullanıcı adı ve şifre gereklidir.");
                return View();
            }

            var admin = _adminManager.TGetList().FirstOrDefault(x => x.UserName == username);

            if (admin != null && VerifyPassword(admin.Password, password))
            {
                var claims = new List<Claim>
               {
                   new Claim(ClaimTypes.Name, admin.UserName),
                   new Claim(ClaimTypes.Role, "Admin")
               };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = System.DateTimeOffset.UtcNow.AddHours(1)
                    });

                return RedirectToAction("Index", "Default");
            }

            ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}