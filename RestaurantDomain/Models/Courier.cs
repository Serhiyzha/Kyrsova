using System;

namespace RestaurantDomain.Models
{
    public class Courier : EntityBase
    {
        private string _fullName = string.Empty;
        private string _phoneNumber = string.Empty;
        private bool _isFree;

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ім'я кур'єра не може бути порожнім.");
                _fullName = value.Trim();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Телефон не може бути порожнім.");
                _phoneNumber = value.Trim();
            }
        }

        public bool IsFree
        {
            get => _isFree;
            set => _isFree = value;
        }

        public Courier() { }

        public Courier(string fullName, string phoneNumber, bool isFree = true)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            IsFree = isFree;
        }

        public override string GetSummary()
        {
            string status = IsFree ? "Вільний" : "Зайнятий";
            return $"Кур'єр: {FullName} (Тел: {PhoneNumber}) — {status}";
        }
    }
}