using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.EntityLayer.Concrete
{
    public partial class Actor
    {
        public Actor()
        {
            DvdActors = new HashSet<DvdActor>();
        }

        public int ActorId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DvdActor> DvdActors { get; set; }
    }
}
