using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class Director
    {
        public Director()
        {
            DvdDirectors = new HashSet<DvdDirector>();
        }

        public int DirectorId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DvdDirector> DvdDirectors { get; set; }
    }
}
