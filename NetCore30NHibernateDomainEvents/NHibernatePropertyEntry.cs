namespace NetCore30NHibernateDomainEvents
{
    public class NHibernatePropertyEntry
        : PropertyEntry
    {
        private readonly object[] _originalValues;
        private readonly object[] _currentValues;
        private readonly int _index;

        public NHibernatePropertyEntry(
            string name,
            bool isModified,
            object [] originalValues,
            object [] currentValues,
            int index)
            : base(name, isModified)
        {
            _originalValues = originalValues;
            _currentValues = currentValues;
            _index = index;
        }

        public override object CurrentValue
        {
            get => _currentValues[_index];
            set => _currentValues[_index] = value;
        }

        public override object OriginalValue
        {
            get => _originalValues?[_index];
            set
            {
                if (_originalValues != null)
                {
                    _originalValues[_index] = value;
                }
            }
        }
    }
}