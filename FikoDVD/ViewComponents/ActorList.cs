using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace FikoDVD.ViewComponents
{
    public class ActorList: ViewComponent
    {
        ActorManager actorManager = new ActorManager(new EfActorDal());  
        public IViewComponentResult Invoke()
        {
            var values = actorManager.TGetList();
            return View(values);
        }
        

    }

}
