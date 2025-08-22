using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;

namespace FikoDVD.Controllers
{
    [Authorize]
    public class DvdRentalController : Controller
    {
        DvdRentalManager dvdRentalManager = new DvdRentalManager(new EfDvdRentalDal());
        DvdManager dvdManager = new DvdManager(new EfDvdDal());
        ClientManager clientManager = new ClientManager(new EfClientDal());

        [HttpGet]
        public IActionResult Index()
        {
            // Tüm DvdRental kayıtlarını al
            var rentals = dvdRentalManager.TGetList();

            // Late Fee'yi dinamik olarak hesapla
            foreach (var rental in rentals)
            {
                if (rental.ReturnDate.HasValue && rental.ReturnDate > rental.ExpectedReturnDate)
                {
                    var lateDays = (rental.ReturnDate.Value - rental.ExpectedReturnDate).Days;
                    rental.LateFee = lateDays * 10; // Günlük gecikme ücreti: 5 birim
                    rental.TotalFee = rental.RentalFee + rental.LateFee;
                }
                else
                {
                    rental.LateFee = 0;
                    rental.TotalFee = rental.RentalFee;
                }
            }

            return View(rentals);
        }


        [HttpGet]
        public IActionResult AddDvdRental(int dvdId)
        {
            // DVD bilgilerini al
            var dvd = dvdManager.TGetById(dvdId);
            if (dvd == null)
            {
                return NotFound("DVD not found");
            }

            // DVD bilgilerini ViewBag ile gönder
            ViewBag.DvdId = dvd.DvdId;
            ViewBag.Title = dvd.Title;
            ViewBag.Description = dvd.Description;
            ViewBag.Stock = dvd.Stock;
            ViewBag.ReleaseYear = dvd.ReleaseYear;
            ViewBag.Country = dvd.Country;

            return View();
        }



            [HttpPost]
            public IActionResult AddDvdRental(DvdRental dvdRental, string TcNo, string Name, string Surname, string TelNo)
            {
            

                // 1. Müşteri bilgilerini kontrol edin veya yeni müşteri ekleyin
                var client = clientManager.TGetList().FirstOrDefault(c => c.TcNo == TcNo);
                if (client == null)
                {
                    client = new Client
                    {
                        TcNo = TcNo,
                        Name = Name,
                        Surname = Surname,
                        TelNo = TelNo,
                        RentalCount = 0
                    };
                    clientManager.TAdd(client);
                }
            else
            {
                // Eğer müşteri varsa telefon numarasını kontrol et
                if (client.TelNo != TelNo)
                {
                    ModelState.AddModelError("TelNo", "The phone number does not match the existing records.");
                    return View(dvdRental); // Hata mesajı ile geri dön
                }
                if (client.Name != Name)
                {
                    ModelState.AddModelError("TelNo", "The name does not match the existing records.");
                    return View(dvdRental); // Hata mesajı ile geri dön
                }
                if (client.Surname != Surname)
                {
                    ModelState.AddModelError("TelNo", "The surname does not match the existing records.");
                    return View(dvdRental); // Hata mesajı ile geri dön
                }
                if (client.TcNo != TcNo)
                {
                    ModelState.AddModelError("TelNo", "The surname does not match the existing records.");
                    return View(dvdRental); // Hata mesajı ile geri dön
                }
            }

            var activeRentals = dvdRentalManager.TGetList()
                       .Where(r => r.ClientId == client.ClientId && r.RentalStatus == true) // Aktif kiralamalar
                       .Count();

            if (activeRentals >= 3)
            {
                ModelState.AddModelError("", "You cannot rent more than 3 movies at the same time.");
                return View(dvdRental); // Hata mesajı ile kiralama formuna geri dön
            }
            // 2. Aynı müşteri aynı DVD'yi zaten kiralamış mı?
            var existingRental = dvdRentalManager.TGetList()
                    .FirstOrDefault(r => r.DvdId == dvdRental.DvdId && r.ClientId == client.ClientId && r.RentalStatus == true);

                if (existingRental != null)
                {
                    ModelState.AddModelError("", "You cannot rent the same DVD twice.");
                    return View(dvdRental);
                }

                // 3. DVD bilgilerini kontrol edin
                var dvd = dvdManager.TGetById(dvdRental.DvdId);
                if (dvd == null)
                {
                    ModelState.AddModelError("", "DVD not found.");
                    return View(dvdRental);
                }
                if (dvdRental.ExpectedReturnDate < dvdRental.RentDate)
                {
                    ModelState.AddModelError("ExpectedReturnDate", "Expected Return Date cannot be earlier than Rent Date.");
                    return View(dvdRental);
                }
                if (dvd.Stock <= 0)
                {
                    ModelState.AddModelError("", "This DVD is out of stock. Please try again later.");
                    return View(dvdRental);
                }

                // 4. Kiralama bilgilerini doldurun
                dvdRental.ClientId = client.ClientId;
                dvdRental.RentDate = DateTime.Now; // Kiralama tarihini otomatik olarak bugüne ayarla
                dvdRental.RentalFee =( ((dvdRental.ExpectedReturnDate - dvdRental.RentDate).Days)+1) * 25;
                dvdRental.LateFee = 0;
                dvdRental.TotalFee = dvdRental.RentalFee;
                dvdRental.RentalStatus = true;

                try
                {
                    // Kiralamayı kaydet
                    dvdRentalManager.TAdd(dvdRental);

                    // 5. DVD stok bilgisini güncelle
                    dvd.Stock -= 1;
                    dvd.RentalCount += 1;
                    dvdManager.TUpdate(dvd);

                    // 6. Müşteri kiralama sayısını güncelle
                    client.RentalCount += 1;
                    clientManager.TUpdate(client);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the rental: " + ex.Message);
                    return View(dvdRental);
                }

                return RedirectToAction("Index");
            }



        public IActionResult DeleteDvdRental(int id)
        {
            var dvdRental = dvdRentalManager.TGetById(id);

            if (dvdRental != null)
            {
                var dvd = dvdManager.TGetById(dvdRental.DvdId);
                if (dvd != null)
                {
                    dvd.Stock += 1; // Stok bilgisini artır
                    dvdManager.TUpdate(dvd);
                }

                var client = clientManager.TGetById(dvdRental.ClientId);
                if (client != null)
                {
                    client.RentalCount -= 1; // Müşteri kiralama sayısını azalt
                    clientManager.TUpdate(client);
                }

                dvdRentalManager.TDelete(dvdRental);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditDvdRental(int id)
        {
            var values = dvdRentalManager.TGetById(id);
            if (values == null)
            {
                return NotFound();
            }
            return View(values);
        }

        [HttpPost]
        public IActionResult EditDvdRental(DvdRental dvdRental)
        {
            var existingRental = dvdRentalManager.TGetById(dvdRental.RentalId);
            if (existingRental == null)
            {
                return NotFound();
            }

            // Check if ReturnDate is empty string from form and set to null
            if (Request.Form["ReturnDate"].ToString() == "")
            {
                dvdRental.ReturnDate = null;
                dvdRental.RentalStatus = true; // ReturnDate yoksa Active
            }
            else
            {
                dvdRental.RentalStatus = false; // ReturnDate varsa Completed
            }

            // ReturnDate varsa onu kullan, yoksa ExpectedReturnDate üzerinden hesaplama yap
            DateTime feeCalculationDate = dvdRental.ReturnDate.HasValue
                ? dvdRental.ReturnDate.Value
                : dvdRental.ExpectedReturnDate;

            if (feeCalculationDate < dvdRental.RentDate)
            {
                ModelState.AddModelError("", "Return Date or Expected Return Date cannot be earlier than Rent Date.");
                return View(dvdRental);
            }

            // RentalFee hesaplama
            var rentalDays = (feeCalculationDate - dvdRental.RentDate).Days;
            dvdRental.RentalFee = rentalDays * 25; // Günlük ücret: 25 birim

            // LateFee hesaplama: ReturnDate mevcutsa ve ExpectedReturnDate'i geçtiyse
            if (dvdRental.ReturnDate.HasValue && dvdRental.ReturnDate > dvdRental.ExpectedReturnDate)
            {
                var lateDays = (dvdRental.ReturnDate.Value - dvdRental.ExpectedReturnDate).Days;
                dvdRental.LateFee = lateDays * 10; // Günlük gecikme ücreti: 10 birim
            }
            else
            {
                dvdRental.LateFee = 0;
            }

            dvdRental.TotalFee = dvdRental.RentalFee + dvdRental.LateFee;

            // Mevcut kaydı güncelle
            existingRental.ExpectedReturnDate = dvdRental.ExpectedReturnDate;
            existingRental.ReturnDate = dvdRental.ReturnDate;
            existingRental.RentalFee = dvdRental.RentalFee;
            existingRental.LateFee = dvdRental.LateFee;
            existingRental.TotalFee = dvdRental.TotalFee;
            existingRental.RentalStatus = dvdRental.RentalStatus;

            dvdRentalManager.TUpdate(existingRental);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ConfirmRental(int id)
        {
            var rental = dvdRentalManager.TGetById(id);
            if (rental == null)
            {
                return NotFound();
            }

            // ReturnDate yoksa şu anki tarihi ata
            if (!rental.ReturnDate.HasValue)
            {
                rental.ReturnDate = DateTime.Now;
            }

            // Kiralama süresini hesapla (sıfır gün olmaması için +1 ekliyoruz)
            var rentalDays = (rental.ReturnDate.Value.Date - rental.RentDate.Date).Days;
            rental.RentalFee = rentalDays * 25; // Günlük 25 birim

            // Eğer ReturnDate, ExpectedReturnDate'i geçtiyse gecikme ücreti hesapla
            if (rental.ReturnDate > rental.ExpectedReturnDate)
            {
                var lateDays = (rental.ReturnDate.Value - rental.ExpectedReturnDate).Days;
                rental.LateFee = lateDays * 10; // Günlük gecikme ücreti: 10 birim
            }
            else
            {
                rental.LateFee = 0;
            }

            rental.TotalFee = rental.RentalFee + rental.LateFee;
            rental.RentalStatus = false; // Durumu "Completed" olarak ayarla

            dvdRentalManager.TUpdate(rental);

            return RedirectToAction("Index");
        }

    }
}
