using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class DvdActor
    {
        public int DvdId { get; set; }
        public int ActorId { get; set; }

        public virtual Actor Actor { get; set; }
        public virtual Dvd Dvd { get; set; }
    }
}
