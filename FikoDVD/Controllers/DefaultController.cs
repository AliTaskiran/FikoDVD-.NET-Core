using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using FikoDVD.Models;

namespace FikoDVD.Controllers
{
    [Authorize]
    public class DefaultController : Controller
    {

        public IActionResult Index()
        {

            return View();
        }

        public PartialViewResult HeaderPartial()
        {
            return PartialView();
        }
        // Controller: DvdRentalController.cs
        public IActionResult DVDRent(string searchTerm)
        {
            DvdManager dvdManager = new DvdManager(new EfDvdDal());
            var values = dvdManager.TGetList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Convert search term to lowercase for case-insensitive search
                searchTerm = searchTerm.ToLower();

                // Filter DVDs based on multiple criteria
                values = values.Where(d =>
                    d.Title.ToLower().Contains(searchTerm) ||
                    d.Description.ToLower().Contains(searchTerm) ||
                    d.Country.ToLower().Contains(searchTerm) ||
                    d.ReleaseYear.ToString().Contains(searchTerm)
                ).ToList();
            }

            ViewBag.SearchTerm = searchTerm; // Keep the search term in the search box
            return View(values);
        }

    }
}
