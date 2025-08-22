using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class ActorManager : IActorService
    {
        IActorDal _actorDal;

        public ActorManager(IActorDal actorDal)
        {
            _actorDal = actorDal;
        }

        public void TAdd(Actor t)
        {
            _actorDal.Insert(t);
        }

        public void TDelete(Actor t)
        {
            _actorDal.Delete(t);
        }

        public Actor TGetById(int id)
        {
           return _actorDal.GetById(id);
        }

        public List<Actor> TGetList()
        {
           return _actorDal.GetList();
        }

        public void TUpdate(Actor t)
        {
           _actorDal.Update(t);
        }
    }
}
