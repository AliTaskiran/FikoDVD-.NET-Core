using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FikoDVD.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;

namespace FikoDVD.Controllers
{
    public class AnalysisController : Controller
    {
        public IActionResult Index()
        {
            var dvdManager = new DvdManager(new EfDvdDal());
            var dvdRentalManager = new DvdRentalManager(new EfDvdRentalDal());
            var clientManager = new ClientManager(new EfClientDal());

            // Verileri çek
            var dvds = dvdManager.TGetList();
            var rentals = dvdRentalManager.TGetList();
            var clients = clientManager.TGetList();

            // DVD'ler için ViewModel oluştur
            var dvdViewModels = dvds.Select(dvd =>
            {
                // Tüm kiralamalar
                var allRentals = rentals.Where(r => r.DvdId == dvd.DvdId).ToList();

                // Aktif kiralamalar
                var currentRentals = rentals.Where(r =>
                    r.DvdId == dvd.DvdId &&
                    r.RentalStatus == true && // Aktif kiralama
                    (r.ReturnDate == null || r.ReturnDate > DateTime.Now)) // Henüz iade edilmemiş
                    .ToList();

                // Kiracı bilgileri
                var renterInfos = currentRentals.Select(rental =>
                {
                    var renter = clients.FirstOrDefault(c => c.ClientId == rental.ClientId);
                    return new RenterInfo
                    {
                        Name = renter != null
                            ? $"{renter.Name} {renter.Surname} ({renter.TelNo})"
                            : "Unknown Renter",
                        RentDate = rental.RentDate,
                        ExpectedReturnDate = rental.ExpectedReturnDate
                    };
                }).ToList();

                // Mevcut kopya sayısı
                var availableCopies = 2 - currentRentals.Count;

                // ViewModel oluştur
                return new DvdListViewModel
                {
                    DvdId = dvd.DvdId,
                    Title = dvd.Title,
                    TotalRentals = allRentals.Count,
                    RentalCount = dvd.RentalCount,
                    CurrentRenters = renterInfos,
                    AvailableCopies = availableCopies,
                    TotalCopies = 2
                };
            }).ToList();

            // View'e gönder
            return View(dvdViewModels);
        }
        public IActionResult RentalAnalysis()
        {
            var dvdManager = new DvdManager(new EfDvdDal());
            var dvdRentalManager = new DvdRentalManager(new EfDvdRentalDal());

            // En çok ve en az kiralanan filmler
            var dvds = dvdManager.TGetList().Select(d => new
            {
                d.DvdId,
                d.Title,
                d.RentalCount
            }).ToList();

            var mostRented = dvds.OrderByDescending(d => d.RentalCount).Take(5).ToList();
            var leastRented = dvds.OrderBy(d => d.RentalCount).Take(5).ToList();

            // Günlere göre kiralama sayısı (son 7 gün)
            var rentalsByDate = dvdRentalManager.TGetList()
                .Where(r => r.RentDate >= DateTime.Now.AddDays(-7))
                .GroupBy(r => r.RentDate.Date)
                .Select(g => new DailyRental
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            var analysisViewModel = new RentalAnalysisViewModel
            {
                MostRentedDvds = mostRented.Select(d => new DvdRentalStats
                {
                    Title = d.Title,
                    RentalCount = d.RentalCount
                }).ToList(),

                LeastRentedDvds = leastRented.Select(d => new DvdRentalStats
                {
                    Title = d.Title,
                    RentalCount = d.RentalCount
                }).ToList(),

                DailyRentals = rentalsByDate
            };

            return View(analysisViewModel);
        }
        public IActionResult RentalStatus()
        {
            var dvdRentalManager = new DvdRentalManager(new EfDvdRentalDal());
            var dvdManager = new DvdManager(new EfDvdDal());
            var clientManager = new ClientManager(new EfClientDal());

            // Günün başlangıcını al (saat 00:00:00)
            var today = DateTime.Now.Date;

            var activeRentals = dvdRentalManager.TGetList()
                .Where(r => r.RentalStatus == true && r.ReturnDate == null)
                .ToList();

            var rentalStatusViewModel = new RentalStatusViewModel
            {
                OverdueRentals = new List<RentalDetailViewModel>(),
                UpcomingReturns = new List<RentalDetailViewModel>()
            };

            foreach (var rental in activeRentals)
            {
                var dvd = dvdManager.TGetById(rental.DvdId);
                var client = clientManager.TGetById(rental.ClientId);

                // Beklenen dönüş tarihini de günün başlangıcına normalize et
                var expectedReturnDate = rental.ExpectedReturnDate.Date;
                var daysUntilReturn = (expectedReturnDate - today).Days;

                var rentalDetail = new RentalDetailViewModel
                {
                    RentalId = rental.RentalId,
                    DvdTitle = dvd?.Title ?? "Unknown DVD",
                    ClientName = $"{client?.Name} {client?.Surname}",
                    ClientPhone = client?.TelNo,
                    RentDate = rental.RentDate,
                    ExpectedReturnDate = rental.ExpectedReturnDate,
                    DaysOverdueOrRemaining = Math.Abs(daysUntilReturn)
                };

                if (today > expectedReturnDate)
                {
                    rentalStatusViewModel.OverdueRentals.Add(rentalDetail);
                }
                else if (daysUntilReturn <= 2)
                {
                    rentalStatusViewModel.UpcomingReturns.Add(rentalDetail);
                }
            }

            rentalStatusViewModel.OverdueRentals = rentalStatusViewModel.OverdueRentals
                .OrderByDescending(r => r.DaysOverdueOrRemaining)
                .ToList();

            rentalStatusViewModel.UpcomingReturns = rentalStatusViewModel.UpcomingReturns
                .OrderBy(r => r.ExpectedReturnDate)
                .ToList();

            return View(rentalStatusViewModel);
        }   
    }
}
