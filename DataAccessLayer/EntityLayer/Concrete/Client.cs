using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class Client
    {
        public int ClientId { get; set; }
        public string TelNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TcNo { get; set; }
        public int RentalCount { get; set; }
    }
}
