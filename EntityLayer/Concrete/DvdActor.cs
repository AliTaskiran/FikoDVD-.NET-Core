using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class DvdActor
    {       
        public int DvdId { get; set; }
        public Dvd Dvd { get; set; }

        public int ActorId { get; set; }
        public Actor Actor { get; set; }
    }

}
