using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FikoDVD.Controllers
{
    [Authorize]
    public class DvdController : Controller
    {

        DvdManager dvdManager = new DvdManager(new EfDvdDal());
        ActorManager actorManager = new ActorManager(new EfActorDal());
        DirectorManager directorManager = new DirectorManager(new EfDirectorDal());
        CategoryManager categoryManager = new CategoryManager(new EfCategoryDal());

        public IActionResult Index(string searchTerm)
        {
            var values = dvdManager.TGetList();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                values = values.Where(d =>
                    d.Title.ToLower().Contains(searchTerm) ||
                    d.Description.ToLower().Contains(searchTerm) ||
                    d.Country.ToLower().Contains(searchTerm) ||
                    d.ReleaseYear.ToString().Contains(searchTerm)
                ).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            return View(values);
        }

        [HttpGet]
        public IActionResult AddDvd()
        {
            var actors = actorManager.TGetList();
            var categories = categoryManager.TGetList();
            var directors = directorManager.TGetList();
            PopulateViewBags();

            if (actors == null || !actors.Any())
            {
                throw new Exception("Actors list is empty or null.");
            }

            if (categories == null || !categories.Any())
            {
                throw new Exception("Categories list is empty or null.");
            }

            if (directors == null || !directors.Any())
            {
                throw new Exception("Directors list is empty or null.");
            }

            ViewBag.Actors = actors;
            ViewBag.Categories = categories;
            ViewBag.Directors = directors;


            return View();
        }

        [HttpPost]
        public IActionResult AddDvd(Dvd dvd, int[] selectedActors, int[] selectedCategories, int[] selectedDirectors)
        {

            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                return View(dvd);
            }

            try
            {
                if (selectedActors != null && selectedActors.Any())
                {
                    dvd.DvdActors = selectedActors.Select(actorId => new DvdActor
                    {
                        ActorId = actorId
                    }).ToList();
                }

                if (selectedCategories != null && selectedCategories.Any())
                {
                    dvd.DvdCategories = selectedCategories.Select(categoryId => new DvdCategory
                    {
                        CategoryId = categoryId
                    }).ToList();
                }

                if (selectedDirectors != null && selectedDirectors.Any())
                {
                    dvd.DvdDirectors = selectedDirectors.Select(directorId => new DvdDirector
                    {
                        DirectorId = directorId
                    }).ToList();
                }
                

                dvdManager.TAdd(dvd);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                PopulateViewBags();
                return View(dvd);
            }
        }

        private void PopulateViewBags()
        {
            ViewBag.Countries = new List<string>{
                            "United States",
                            "United Kingdom",
                            "Canada",
                            "Germany",
                            "France",
                            "Australia",
                            "Japan",
                            "China",
                            "India",
                            "Turkey"
                        };
            ViewBag.Actors = actorManager.TGetList();
            ViewBag.Categories = categoryManager.TGetList();
            ViewBag.Directors = directorManager.TGetList();
        }
        public IActionResult DeleteDvd(int id)
        {

            var dvd = dvdManager.TGetById(id);

            if (dvd != null)
            {

                dvdManager.TDelete(dvd);


                using (var context = new Context())
                {
                    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Dvds', RESEED, {0})", context.Dvds.Max(d => d.DvdId));
                }
            }


            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult EditDvd(int id)
        {
            var values = dvdManager.TGetById(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult EditDvd(Dvd dvd)
        {
            dvdManager.TUpdate(dvd);
            return RedirectToAction("Index");
        }
    }

   
    }

