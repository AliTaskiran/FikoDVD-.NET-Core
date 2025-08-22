using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class DvdRentalManager : IGenericService<DvdRental>
    {
        IDvdRentalDal _dvdRentalDal;

        public DvdRentalManager(IDvdRentalDal dvdRentalDal)
        {
            _dvdRentalDal = dvdRentalDal;
        }

        public void TAdd(DvdRental t)
        {
            _dvdRentalDal.Insert(t);
        }

        public void TDelete(DvdRental t)
        {
            _dvdRentalDal.Delete(t);
        }

        public DvdRental TGetById(int id)
        {
            return _dvdRentalDal.GetById(id);
        }

        public List<DvdRental> TGetList()
        {
            return _dvdRentalDal.GetList();
        }

        public void TUpdate(DvdRental t)
        {
            _dvdRentalDal.Update(t);
        }

        // Aktif kiralama sayısını al
        public int GetActiveRentalsCount(int dvdId)
        {
            return _dvdRentalDal.GetList(x => x.DvdId == dvdId && x.RentalStatus).Count;
        }

        // Stok durumunu kontrol et
        public bool IsStockAvailable(int dvdId, int stock)
        {
            var activeRentals = GetActiveRentalsCount(dvdId);
            return activeRentals <= stock;
        }
    }
}
