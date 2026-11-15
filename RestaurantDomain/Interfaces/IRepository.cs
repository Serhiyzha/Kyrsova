using System;
using System.Collections.Generic;

namespace RestaurantDomain.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T item);
        bool Remove(Guid id);
        T? GetById(Guid id);
        List<T> GetAll();
    }
}