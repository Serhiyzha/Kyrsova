using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RestaurantDomain.Exceptions;
using RestaurantDomain.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace RestaurantDomain.Services
{
    public class RestaurantDataContainer
    {
        public List<Dish> Dishes { get; set; } = new();
        public List<Courier> Couriers { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<TableReservation> Reservations { get; set; } = new();
    }

    public class RestaurantService
    {
        private readonly GenericRepository<Dish> _dishRepo = new();
        private readonly GenericRepository<Courier> _courierRepo = new();
        private readonly GenericRepository<Order> _orderRepo = new();
        private readonly GenericRepository<TableReservation> _reservationRepo = new();

        public GenericRepository<Dish> Dishes => _dishRepo;
        public GenericRepository<Courier> Couriers => _courierRepo;
        public GenericRepository<Order> Orders => _orderRepo;
        public GenericRepository<TableReservation> Reservations => _reservationRepo;

        public List<Dish> GetVegetarianMenu()
        {
            return _dishRepo.GetAll()
                .Where(d => d.IsVegetarian)
                .OrderBy(d => d.Price)
                .ToList();
        }

        public void PlaceOrder(Order order)
        {
            if (order.Dishes.Count == 0)
                throw new InvalidOperationException("Неможливо оформити порожнє замовлення.");

            var freeCourier = _courierRepo.GetAll().FirstOrDefault(c => c.IsFree);
            if (freeCourier == null)
                throw new CourierUnavailableException("На жаль, зараз немає жодного вільного кур'єра.");

            freeCourier.IsFree = false;
            order.AssignedCourier = freeCourier;
            _orderRepo.Add(order);
        }

        public void ReserveTable(TableReservation reservation)
        {
            bool isOccupied = _reservationRepo.GetAll().Any(r =>
                r.TableNumber == reservation.TableNumber &&
                Math.Abs((r.ReservationTime - reservation.ReservationTime).TotalHours) < 2);

            if (isOccupied)
                throw new InvalidOperationException($"Столик №{reservation.TableNumber} вже заброньовано на цей час.");

            _reservationRepo.Add(reservation);
        }

        public void SaveToFile(string filePath)
        {
            var container = new RestaurantDataContainer
            {
                Dishes = _dishRepo.GetAll(),
                Couriers = _courierRepo.GetAll(),
                Orders = _orderRepo.GetAll(),
                Reservations = _reservationRepo.GetAll()
            };

            var options = new JsonSerializerOptions  
            { 
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(container, options);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            var container = JsonSerializer.Deserialize<RestaurantDataContainer>(json);
            if (container == null) return;

            _dishRepo.Items = container.Dishes;
            _courierRepo.Items = container.Couriers;
            _orderRepo.Items = container.Orders;
            _reservationRepo.Items = container.Reservations;
        }
    }
}