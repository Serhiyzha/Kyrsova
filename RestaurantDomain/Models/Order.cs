using System;
using System.Collections.Generic;
using System.Linq;
using RestaurantDomain.Exceptions;
using RestaurantDomain.Interfaces;

namespace RestaurantDomain.Models
{
    public class Order : EntityBase, IPriceCalculable
    {
        private List<Dish> _dishes = new();
        private decimal _tipPercentage;
        private decimal _deliveryFee;
        private Courier? _assignedCourier;
        private string _deliveryAddress = string.Empty;

        public List<Dish> Dishes
        {
            get => _dishes;
            set => _dishes = value ?? new List<Dish>();
        }

        public decimal TipPercentage
        {
            get => _tipPercentage;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentException("Відсоток чайових має бути від 0 до 100.");
                _tipPercentage = value;
            }
        }

        public decimal DeliveryFee
        {
            get => _deliveryFee;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Вартість доставки не може бути від'ємною.");
                _deliveryFee = value;
            }
        }

        public Courier? AssignedCourier
        {
            get => _assignedCourier;
            set => _assignedCourier = value;
        }

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set => _deliveryAddress = value;
        }

        public Order() { }

        public Order(string deliveryAddress, decimal deliveryFee, decimal tipPercentage)
        {
            if (string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("Адреса доставки обов'язкова.");

            DeliveryAddress = deliveryAddress.Trim();
            DeliveryFee = deliveryFee;
            TipPercentage = tipPercentage;
        }

        public void AddDish(Dish dish)
        {
            if (!dish.IsAvailable)
                throw new DishNotAvailableException($"Страва '{dish.Name}' наразі недоступна для замовлення.");
            _dishes.Add(dish);
        }

        public decimal CalculateTotalCost()
        {
            decimal dishesSum = _dishes.Sum(d => d.Price);
            decimal tips = dishesSum * (_tipPercentage / 100m);
            return dishesSum + tips + _deliveryFee;
        }

        public override string GetSummary()
        {
            string courierInfo = AssignedCourier != null ? AssignedCourier.FullName : "Не призначено";
            return $"Замовлення #{Id.ToString()[..6]} | Адреса: {DeliveryAddress} | Страв: {Dishes.Count} | Кур'єр: {courierInfo} | Разом: {CalculateTotalCost():C}";
        }
    }
}