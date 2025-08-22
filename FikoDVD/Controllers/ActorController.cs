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
    public class ActorController : Controller
    {
        ActorManager actorManager = new ActorManager(new EfActorDal());
        public IActionResult Index()
        {
            var values = actorManager.TGetList();
            return View(values);
        }

        [HttpGet]
        public IActionResult AddActor()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddActor(Actor actor)
        {
            actorManager.TAdd(actor);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteActor(int id)
        {

            var dvd = actorManager.TGetById(id);

            if (dvd != null)
            {

                actorManager.TDelete(dvd);


                using (var context = new Context())
                {
                    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Dvds', RESEED, {0})", context.Dvds.Max(d => d.DvdId));
                }
            }


            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult EditActor(int id)
        {
            var values = actorManager.TGetById(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult EditActor(Actor actor)
        {
            actorManager.TUpdate(actor);
            return RedirectToAction("Index");
        }
    }

}
