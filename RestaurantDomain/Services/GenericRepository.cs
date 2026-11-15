using System;
using System.Collections.Generic;
using System.Linq;
using RestaurantDomain.Interfaces;
using RestaurantDomain.Models;

namespace RestaurantDomain.Services
{
    public class GenericRepository<T> : IRepository<T> where T : EntityBase
    {
        private List<T> _items = new();

        public List<T> Items
        {
            get => _items;
            set => _items = value ?? new List<T>();
        }

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        public bool Remove(Guid id)
        {
            var item = GetById(id);
            if (item == null) return false;
            return _items.Remove(item);
        }

        public T? GetById(Guid id) => _items.FirstOrDefault(x => x.Id == id);

        public List<T> GetAll() => new List<T>(_items);
    }
}