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
    public class CategoryController : Controller
    {
        CategoryManager categoryManager = new CategoryManager(new EfCategoryDal());
        public IActionResult Index()
        {
            var values = categoryManager.TGetList();
            return View(values);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            categoryManager.TAdd(category);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteCategory(int id)
        {

            var category = categoryManager.TGetById(id);

            if (category != null)
            {

                categoryManager.TDelete(category);


                using (var context = new Context())
                {
                    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Dvds', RESEED, {0})", context.Dvds.Max(d => d.DvdId));
                }
            }


            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var values = categoryManager.TGetById(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            categoryManager.TUpdate(category);
            return RedirectToAction("Index");
        }
    }
}
