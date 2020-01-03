using System.Collections.Generic;

namespace NetCore30NHibernateDomainEvents.Models
{
    public class Group
        : IAudit
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<User> Users { get; protected set; }

        protected Group()
        {
        }

        public Group(
            string name)
        {
            Name = name;
        }
    }
}
