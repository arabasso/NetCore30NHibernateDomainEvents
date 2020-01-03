namespace NetCore30NHibernateDomainEvents.Models
{
    public class User
        : IAudit
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
        public virtual bool Active { get; set; }
        public virtual Group Group { get; set; }

        protected User()
        {
        }

        public User(
            string name,
            string login,
            Group @group)
        {
            Name = name;
            Login = login;
            Group = @group;
        }
    }
}
