using System.Collections.Generic;
using System;

namespace FikoDVD.Models
{
    public class RentalStatusViewModel
    {
        public List<RentalDetailViewModel> OverdueRentals { get; set; }
        public List<RentalDetailViewModel> UpcomingReturns { get; set; }
    }

    public class RentalDetailViewModel
    {
        public int RentalId { get; set; }
        public string DvdTitle { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public int DaysOverdueOrRemaining { get; set; }
    }
}
