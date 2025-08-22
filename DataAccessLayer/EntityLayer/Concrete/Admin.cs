using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public int TcNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
