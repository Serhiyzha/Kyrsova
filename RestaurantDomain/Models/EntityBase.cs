using System;

namespace RestaurantDomain.Models
{
    public abstract class EntityBase
    {
        private Guid _id;

        public Guid Id
        {
            get => _id;
            set => _id = value == Guid.Empty ? Guid.NewGuid() : value;
        }

        protected EntityBase()
        {
            _id = Guid.NewGuid();
        }

        public abstract string GetSummary();
    }
}