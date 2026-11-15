using System;

namespace RestaurantDomain.Models
{
    public class Dish : EntityBase, IComparable<Dish>
    {
        private string _name = string.Empty;
        private decimal _price;
        private bool _isVegetarian;
        private bool _isAvailable;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва страви не може бути порожньою.");
                _name = value.Trim();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Ціна страви повинна бути більшою за нуль.");
                _price = value;
            }
        }

        public bool IsVegetarian
        {
            get => _isVegetarian;
            set => _isVegetarian = value;
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => _isAvailable = value;
        }

        public Dish() { }

        public Dish(string name, decimal price, bool isVegetarian, bool isAvailable = true)
        {
            Name = name;
            Price = price;
            IsVegetarian = isVegetarian;
            IsAvailable = isAvailable;
        }

        public int CompareTo(Dish? other)
        {
            if (other is null) return 1;
            return Price.CompareTo(other.Price);
        }

        public override string GetSummary()
        {
            string veg = IsVegetarian ? "[Вегетаріанське]" : "";
            string status = IsAvailable ? "В наявності" : "Немає в наявності";
            return $"{Name} {veg} — {Price:C} ({status})";
        }
    }
}