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
    public class DvdManager : IGenericService<Dvd>
    {
        IDvdDal _dvdDal;

        public DvdManager(IDvdDal dvdDal)
        {
            _dvdDal = dvdDal;
        }

        public void TAdd(Dvd t)
        {
           
            _dvdDal.Insert(t);
        }

        public void TDelete(Dvd t)
        {
           _dvdDal.Delete(t);
        }

        public Dvd TGetById(int id)
        {
            
            return _dvdDal.GetById(id); 
        }

        public List<Dvd> TGetList()
        {
           return _dvdDal.GetList();    
        }

        public void TUpdate(Dvd t)
        {
            _dvdDal.Update(t);  
        }
    }
}
