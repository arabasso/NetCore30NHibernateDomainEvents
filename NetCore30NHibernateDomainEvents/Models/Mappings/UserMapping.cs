using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NetCore30NHibernateDomainEvents.Models.Mappings
{
    class UserMapping
        : ClassMapping<User>
    {
        public UserMapping()
        {
            Table("Users");
            Id(m => m.Id, p => p.Generator(Generators.Identity));
            Property(m => m.Name, p =>
            {
                p.Length(100);
                p.NotNullable(true);
            });
            Property(m => m.Login, p =>
            {
                p.Length(50);
                p.NotNullable(true);
                p.Unique(true);
            });
            Property(m => m.Password, p => p.Length(35));
            Property(m => m.Email, p => p.Length(150));
            Property(m => m.Active);
            ManyToOne(m => m.Group, p =>
            {
                p.NotNullable(true);
                p.ForeignKey("FK_GroupId");
                p.Cascade(Cascade.Persist);
            });
        }
    }
}
