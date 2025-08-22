using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class DvdDirector
    {
        public int DvdId { get; set; }
        public int DirectorId { get; set; }

        public virtual Director Director { get; set; }
        public virtual Dvd Dvd { get; set; }
    }
}
