using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;


namespace FikoDVD.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AdminManager adminManager = new AdminManager(new EfAdminDal());

        // Şifre Hashleme Metodu
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

            // Salt'ı hash ile birlikte sakla
            return $"{Convert.ToBase64String(salt)}.{hashedPassword}";
        }

        // Şifre Doğrulama Metodu
        private bool VerifyPassword(string storedHash, string password)
        {
            try
            {
                var parts = storedHash.Split('.');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedHashBytes = Convert.FromBase64String(parts[1]);

                var hashToCheck = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

                return storedHashBytes.SequenceEqual(hashToCheck);
            }
            catch
            {
                return false;
            }
        }

        public IActionResult Index()
        {
            var admins = adminManager.TGetList();
            return View(admins);
        }

        [HttpGet]
        public IActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAdmin(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and Password are required.");
                return View();
            }

            var existingAdmin = adminManager.TGetList()
                .FirstOrDefault(a => a.UserName == userName);

            if (existingAdmin != null)
            {
                ModelState.AddModelError("UserName", "This username is already registered.");
                return View();
            }

            var hashedPassword = HashPassword(password);

            var admin = new Admin
            {
                UserName = userName,
                Password = hashedPassword
            };

            adminManager.TAdd(admin);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteAdmin(int id)
        {
            var admin = adminManager.TGetById(id);
            if (admin != null)
            {
                adminManager.TDelete(admin);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditAdmin(int id)
        {
            var admin = adminManager.TGetById(id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        [HttpPost]
        public IActionResult EditAdmin(Admin model)  // Admin modelini direkt alıyoruz
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = adminManager.TGetById(model.AdminId);
            if (admin == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.UserName))
            {
                // Kullanıcı adı değişiyorsa, benzersiz olduğunu kontrol et
                var existingAdmin = adminManager.TGetList()
                    .FirstOrDefault(a => a.UserName == model.UserName && a.AdminId != model.AdminId);
                if (existingAdmin != null)
                {
                    ModelState.AddModelError("UserName", "This username is already taken");
                    return View(model);
                }
                admin.UserName = model.UserName;
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                admin.Password = HashPassword(model.Password);
            }

            adminManager.TUpdate(admin);
            return RedirectToAction("Index");
        }
    }
}
