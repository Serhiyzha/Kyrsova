using System;
using System.Globalization;
using System.Text;
using RestaurantDomain.Exceptions;
using RestaurantDomain.Models;
using RestaurantDomain.Services;

namespace RestaurantConsoleApp
{
    internal class Program
    {
        private const string DataFilePath = "restaurant_data.json";
        private static readonly RestaurantService _service = new();

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

            try
            {
                _service.LoadFromFile(DataFilePath);
                SeedInitialDataIfEmpty();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження файлу: {ex.Message}");
            }

            bool isRunning = true;
            while (isRunning)
            {
                PrintMenu();
                Console.Write("Оберіть дію: ");
                string? choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1": ShowAllDishes(); break;
                        case "2": ShowVegetarianDishes(); break;
                        case "3": AddNewDish(); break;
                        case "4": AddNewCourier(); break;
                        case "5": CreateOrder(); break;
                        case "6": ShowAllOrders(); break;
                        case "7": CreateReservation(); break;
                        case "8": ShowReservations(); break;
                        case "0":
                            _service.SaveToFile(DataFilePath);
                            Console.WriteLine("Дані успішно збережено. До побачення!");
                            isRunning = false;
                            break;
                        default:
                            Console.WriteLine("Невідома команда. Спробуйте ще раз.");
                            break;
                    }
                }
                catch (CourierUnavailableException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Помилка доставки]: {ex.Message}");
                    Console.ResetColor();
                }
                catch (DishNotAvailableException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Помилка меню]: {ex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Помилка]: {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("=== РЕСТОРАН ТА ДОСТАВКА ЇЖІ ===");
            Console.WriteLine("1. Переглянути все меню");
            Console.WriteLine("2. Переглянути вегетаріанське меню");
            Console.WriteLine("3. Додати нову страву");
            Console.WriteLine("4. Додати кур'єра");
            Console.WriteLine("5. Оформити замовлення на доставку");
            Console.WriteLine("6. Переглянути всі замовлення");
            Console.WriteLine("7. Забронювати столик");
            Console.WriteLine("8. Переглянути бронювання столиків");
            Console.WriteLine("0. Зберегти дані та вийти");
        }

        private static void ShowAllDishes()
        {
            var dishes = _service.Dishes.GetAll();
            dishes.Sort();
            Console.WriteLine("--- Меню ресторану ---");
            for (int i = 0; i < dishes.Count; i++)
                Console.WriteLine($"{i + 1}. {dishes[i].GetSummary()}");
        }

        private static void ShowVegetarianDishes()
        {
            var vegDishes = _service.GetVegetarianMenu();
            Console.WriteLine("--- Вегетаріанське меню ---");
            if (vegDishes.Count == 0)
            {
                Console.WriteLine("Вегетаріанських страв поки немає.");
                return;
            }
            foreach (var dish in vegDishes)
                Console.WriteLine($"- {dish.GetSummary()}");
        }

        private static void AddNewDish()
        {
            Console.Write("Назва страви: ");
            string name = Console.ReadLine() ?? "";

            decimal price = ReadDecimal("Ціна (грн): ");
            bool isVeg = ReadBool("Вегетаріанська? (т/н): ");
            bool isAvail = ReadBool("Є в наявності зараз? (т/н): ");

            var dish = new Dish(name, price, isVeg, isAvail);
            _service.Dishes.Add(dish);
            Console.WriteLine("Страву успішно додано!");
        }

        private static void AddNewCourier()
        {
            Console.Write("ПІБ кур'єра: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Телефон: ");
            string phone = Console.ReadLine() ?? "";

            _service.Couriers.Add(new Courier(name, phone, true));
            Console.WriteLine("Кур'єра додано успішно!");
        }

        private static void CreateOrder()
        {
            var allDishes = _service.Dishes.GetAll();
            if (allDishes.Count == 0)
            {
                Console.WriteLine("Меню порожнє.");
                return;
            }

            Console.Write("Адреса доставки: ");
            string address = Console.ReadLine() ?? "";
            decimal deliveryFee = ReadDecimal("Вартість доставки (грн): ");
            decimal tips = ReadDecimal("Відсоток чайових (%): ");

            var order = new Order(address, deliveryFee, tips);

            ShowAllDishes();
            Console.WriteLine("Вводьте номери страв по одній (0 — завершити вибір):");
            while (true)
            {
                int index = ReadInt("Номер страви: ");
                if (index == 0) break;

                if (index < 1 || index > allDishes.Count)
                {
                    Console.WriteLine("Невірний номер страви.");
                    continue;
                }

                order.AddDish(allDishes[index - 1]);
                Console.WriteLine($"Додано: {allDishes[index - 1].Name}");
            }

            _service.PlaceOrder(order);
            Console.WriteLine($"Замовлення оформлено! Підсумкова сума: {order.CalculateTotalCost():C}");
        }

        private static void ShowAllOrders()
        {
            var orders = _service.Orders.GetAll();
            Console.WriteLine("--- Активні замовлення ---");
            if (orders.Count == 0) Console.WriteLine("Замовлень немає.");
            foreach (var o in orders)
                Console.WriteLine(o.GetSummary());
        }

        private static void CreateReservation()
        {
            int tableNum = ReadInt("Номер столика: ");
            Console.Write("Ім'я гостя: ");
            string guestName = Console.ReadLine() ?? "";
            int guests = ReadInt("Кількість гостей: ");

            Console.Write("Дата та час (ДД.ММ.РРРР ГГ:ХХ): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                throw new FormatException("Невірний формат дати.");

            var reservation = new TableReservation(tableNum, guestName, guests, date);
            _service.ReserveTable(reservation);
            Console.WriteLine("Столик успішно заброньовано!");
        }

        private static void ShowReservations()
        {
            var list = _service.Reservations.GetAll();
            Console.WriteLine("--- Бронювання столиків ---");
            if (list.Count == 0) Console.WriteLine("Бронювань немає.");
            foreach (var r in list)
                Console.WriteLine(r.GetSummary());
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal val)) return val;
                Console.WriteLine("Помилка вводу. Введіть коректне число.");
            }
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int val)) return val;
                Console.WriteLine("Помилка вводу. Введіть ціле число.");
            }
        }

        private static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim().ToLower();
                if (input is "т" or "так" or "y" or "yes") return true;
                if (input is "н" or "ні" or "n" or "no") return false;
                Console.WriteLine("Введіть 'т' (так) або 'н' (ні).");
            }
        }

        private static void SeedInitialDataIfEmpty()
        {
            if (_service.Dishes.GetAll().Count == 0)
            {
                _service.Dishes.Add(new Dish("Борщ український", 145m, false, true));
                _service.Dishes.Add(new Dish("Салат Грецький", 160m, true, true));
                _service.Dishes.Add(new Dish("Різотто з грибами", 210m, true, true));
                _service.Dishes.Add(new Dish("Стейк Рібай", 480m, false, false));
            }

            if (_service.Couriers.GetAll().Count == 0)
            {
                _service.Couriers.Add(new Courier("Олександр Коваленко", "+380671112233", true));
            }
        }
    }
}