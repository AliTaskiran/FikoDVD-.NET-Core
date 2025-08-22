using System;
using System.Collections.Generic;

namespace FikoDVD.Models
{
    public class RenterInfo
    {
        public string Name { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
    }

    public class DvdListViewModel
    {
        public int DvdId { get; set; }
        public string Title { get; set; }
        public int TotalRentals { get; set; }
        public int RentalCount { get; set; }
        public List<RenterInfo> CurrentRenters { get; set; } = new List<RenterInfo>();
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; } = 2; // Her DVD'den 2 kopya olduğu için
        
    }
}