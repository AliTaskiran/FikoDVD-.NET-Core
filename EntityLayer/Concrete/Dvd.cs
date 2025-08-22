using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Dvd
    {
        [Key]
        public int DvdId { get; set; }
        public int ReleaseYear { get; set; }
        public int RentalCount { get; set; }
        public string Country { get; set; }

        [Range(0, 2, ErrorMessage = "Stock value cannot exceed 2.")]
        public int Stock { get; set; }

        public string Image { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public ICollection<DvdCategory> DvdCategories { get; set; }
        public ICollection<DvdActor> DvdActors { get; set; }
        public ICollection<DvdDirector> DvdDirectors { get; set; }
        public ICollection<DvdRental> DvdRentals { get; set; }
    }
}
