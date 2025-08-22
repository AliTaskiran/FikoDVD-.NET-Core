using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FikoDVD.Controllers
{
    [Authorize]
    public class DirectorController : Controller
    {
        DirectorManager directorManager = new DirectorManager(new EfDirectorDal());
        public IActionResult Index()
        {
            var values = directorManager.TGetList();
            return View(values);
        }
        [HttpGet]
        public IActionResult AddDirector()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddDirector(Director director)
        {
            directorManager.TAdd(director);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteDirector(int id)
        {

            var director = directorManager.TGetById(id);

            if (director != null)
            {

                directorManager.TDelete(director);


                using (var context = new Context())
                {
                    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Dvds', RESEED, {0})", context.Dvds.Max(d => d.DvdId));
                }
            }


            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult EditDirector(int id)
        {
            var values = directorManager.TGetById(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult EditDirector(Director director)
        {
            directorManager.TUpdate(director);
            return RedirectToAction("Index");
        }
    }
}
