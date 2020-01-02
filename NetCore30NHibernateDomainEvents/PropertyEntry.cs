namespace NetCore30NHibernateDomainEvents
{
    public abstract class PropertyEntry
    {
        public string Name { get; }
        public abstract object CurrentValue { get; set; }
        public abstract object OriginalValue { get; set; }
        public bool IsModified { get; set; }

        protected PropertyEntry(
            string name,
            bool isModified)
        {
            Name = name;
            IsModified = isModified;
        }
    }
}