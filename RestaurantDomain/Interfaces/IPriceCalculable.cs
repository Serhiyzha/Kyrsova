using System;
using System.Collections.Generic;

namespace RestaurantDomain.Interfaces
{
    public interface IPriceCalculable
    {
        decimal CalculateTotalCost();
    }
}