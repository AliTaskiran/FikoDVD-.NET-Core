using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class DvdDirector
    {
        public int DvdId { get; set; }
        public Dvd Dvd { get; set; }

        public int DirectorId { get; set; }
        public Director Director { get; set; }
    }

}
