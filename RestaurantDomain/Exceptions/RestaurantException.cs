using System;

namespace RestaurantDomain.Exceptions
{
    public class CourierUnavailableException : Exception
    {
        public CourierUnavailableException(string message) : base(message) { }
    }

    public class DishNotAvailableException : Exception
    {
        public DishNotAvailableException(string message) : base(message) { }
    }
}