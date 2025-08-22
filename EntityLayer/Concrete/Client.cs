using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        public string TelNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TcNo { get; set; }
        public int RentalCount { get; set; }
        public ICollection<DvdRental> DvdRentals { get; set; }
    }
}
