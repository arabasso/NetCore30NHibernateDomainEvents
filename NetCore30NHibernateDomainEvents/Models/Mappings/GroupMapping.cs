using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NetCore30NHibernateDomainEvents.Models.Mappings
{
    class GroupMapping
        : ClassMapping<Group>
    {
        public GroupMapping()
        {
            Table("Groups");
            Id(m => m.Id, p => p.Generator(Generators.Identity));
            Property(m => m.Name, p =>
            {
                p.Length(50);
                p.NotNullable(true);
                p.Unique(true);
            });
            Bag(m => m.Users, p =>
            {
                p.Inverse(true);
                p.Key(k => k.Column("Group"));
            }, mm => mm.OneToMany());
        }
    }
}
