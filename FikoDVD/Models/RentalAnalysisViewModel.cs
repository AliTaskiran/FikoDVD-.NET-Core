using System.Collections.Generic;
using System;

namespace FikoDVD.Models
{
    public class RentalAnalysisViewModel
    {
        public List<DvdRentalStats> MostRentedDvds { get; set; }
        public List<DvdRentalStats> LeastRentedDvds { get; set; }
        public List<DailyRental> DailyRentals { get; set; }
    }

    public class DvdRentalStats
    {
        public string Title { get; set; }
        public int RentalCount { get; set; }
    }

    public class DailyRental
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

}
