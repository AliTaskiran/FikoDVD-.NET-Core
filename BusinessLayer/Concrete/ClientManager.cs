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
   public class ClientManager : IGenericService<Client>
    {
        IClientDal _clientDal;

        public ClientManager(IClientDal clientDal)
        {
            _clientDal = clientDal;
        }

        public void TAdd(Client t)
        {

            _clientDal.Insert(t);
        }

        public void TDelete(Client t)
        {
            _clientDal.Delete(t);
        }

        public Client TGetById(int id)
        {
           return _clientDal.GetById(id);   
        }

        public List<Client> TGetList()
        {
          return _clientDal.GetList();
        }

        public void TUpdate(Client t)
        {
            _clientDal.Update(t);
        }
    }
}
