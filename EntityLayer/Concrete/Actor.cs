using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Actor
    {
        [Key]
        public int ActorId { get; set; }
        public string Name { get; set; }
        public ICollection<DvdActor> DvdActors { get; set; }
    }
}
