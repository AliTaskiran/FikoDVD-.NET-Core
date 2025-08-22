using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace FikoDVD.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly ClientManager clientManager = new ClientManager(new EfClientDal());

        // Hash için yardımcı metod
        private string HashTcNo(string tcNo)
        {
            // Rastgele salt oluştur
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // PBKDF2 algoritması ile TC No'yu hashle
            string hashedTcNo = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: tcNo,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Salt'ı hash ile birlikte sakla
            return $"{Convert.ToBase64String(salt)}.{hashedTcNo}";
        }

        // TC No doğrulama metodu
        private bool VerifyTcNo(string storedHash, string tcNo)
        {
            try
            {
                var parts = storedHash.Split('.');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedHashBytes = Convert.FromBase64String(parts[1]);

                var hashToCheck = KeyDerivation.Pbkdf2(
                    password: tcNo,
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

        public IActionResult Index(string searchTerm)
        {
            var values = clientManager.TGetList();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                values = values.Where(d =>
                    d.Name.ToLower().Contains(searchTerm) ||
                    d.Surname.ToLower().Contains(searchTerm) ||
                    d.TelNo.ToLower().Contains(searchTerm)
                ).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
           
            return View(values);
        }

        [HttpGet]
        public IActionResult AddClient()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddClient(Client client)
        {
            if (string.IsNullOrEmpty(client.TcNo))
            {
                ModelState.AddModelError("TcNo", "TC No is required.");
                return View(client);
            }

            // TC No dublicate kontrolü
            var existingClient = clientManager.TGetList()
                .FirstOrDefault(c => VerifyTcNo(c.TcNo, client.TcNo));

            if (existingClient != null)
            {
                ModelState.AddModelError("TcNo", "This TC No is already registered.");
                return View(client);
            }

            // TC No'yu hashle
            client.TcNo = HashTcNo(client.TcNo);

            clientManager.TAdd(client);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteClient(int id)
        {
            var client = clientManager.TGetById(id);
            if (client != null)
            {
                clientManager.TDelete(client);
                using (var context = new Context())
                {
                    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Clients', RESEED, {0})", context.Clients.Max(c => c.ClientId));
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditClient(int id)
        {
            var values = clientManager.TGetById(id);
            return View(values);
        }

        [HttpPost]
        public IActionResult EditClient(Client client)
        {
            var existingClient = clientManager.TGetById(client.ClientId);
            if (existingClient == null)
            {
                return NotFound();
            }

            if (client.TcNo != existingClient.TcNo)
            {
                // Yeni TC No girilmişse hashle
                client.TcNo = HashTcNo(client.TcNo);
            }
            else
            {
                // TC No değişmediyse eski hash'i koru
                client.TcNo = existingClient.TcNo;
            }

            clientManager.TUpdate(client);
            return RedirectToAction("Index");
        }
    }
}