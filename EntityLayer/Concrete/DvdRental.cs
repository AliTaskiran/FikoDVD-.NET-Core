using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class DvdRental
    {
        [Key]
        public int RentalId { get; set; }
        public int DvdId { get; set; }
        public Dvd Dvd { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } 
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime RentDate { get; set; }
        public decimal RentalFee { get; set; }
        public decimal LateFee { get; set; }
        public decimal TotalFee { get; set; }   
        public bool RentalStatus { get; set; }
    }

}
