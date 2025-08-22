using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Linq;

namespace FikoDVD.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AdminManager adminManager = new AdminManager(new EfAdminDal());

        // Hash için yardımcı metod
        private string HashPassword(string password)
        {
            // Rastgele salt oluştur
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // PBKDF2 algoritması ile şifreyi hashle
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Salt'ı hash ile birlikte saklamak için birleştir
            return $"{Convert.ToBase64String(salt)}.{hashedPassword}";
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            // Şifre kontrolü
            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            // Kullanıcı adı kontrolü
            if (adminManager.TGetList().Any(x => x.UserName == username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            try
            {
                // Admin nesnesi oluştur ve şifreyi hashle
                var admin = new Admin
                {
                    UserName = username,
                    Password = HashPassword(password)
                };

                adminManager.TAdd(admin);
                TempData["SuccessMessage"] = "Registration successful. Please login.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View();
            }
        }
    }
}