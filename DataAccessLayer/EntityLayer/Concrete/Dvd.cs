using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class Dvd
    {
        public Dvd()
        {
            DvdActors = new HashSet<DvdActor>();
            DvdCategories = new HashSet<DvdCategory>();
            DvdDirectors = new HashSet<DvdDirector>();
        }

        public int DvdId { get; set; }
        public int ReleaseYear { get; set; }
        public int RentalCount { get; set; }
        public string Country { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }

        public virtual ICollection<DvdActor> DvdActors { get; set; }
        public virtual ICollection<DvdCategory> DvdCategories { get; set; }
        public virtual ICollection<DvdDirector> DvdDirectors { get; set; }
    }
}
