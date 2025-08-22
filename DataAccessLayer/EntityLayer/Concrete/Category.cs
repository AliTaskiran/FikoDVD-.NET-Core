using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class Category
    {
        public Category()
        {
            DvdCategories = new HashSet<DvdCategory>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DvdCategory> DvdCategories { get; set; }
    }
}
