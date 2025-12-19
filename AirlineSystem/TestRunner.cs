using AirlineSystem.Models;
using AirlineSystem.Services;
using System;
using System.Collections.Generic;

namespace AirlineSystem
{
    public class TestRunner
    {
        private readonly List<string> _testResults = new List<string>();

        public bool RunAllTests()
        {
            _testResults.Clear();

            RunFlightTests();
            RunPassengerTests();
            RunBookingTests();
            RunBusinessLogicTests();

            // Выводим результаты в Output
            foreach (var result in _testResults)
            {
                Console.WriteLine(result);
                System.Diagnostics.Debug.WriteLine(result);
            }

            return !_testResults.Exists(r => r.Contains("ПРОВАЛ"));
        }

        private void RunFlightTests()
        {
            AddResult("=== ТЕСТЫ ДЛЯ РЕЙСОВ ===");

            try
            {
                // Тест 1: Создание рейса
                var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                    DateTime.Now.AddHours(1), DateTime.Now.AddHours(3), 5500.00m, 189);

                TestAssertEqual("Создание рейса", "SU 1001", flight.FlightNumber);
                TestAssertEqual("Маршрут", "Москва → Санкт-Петербург", flight.RouteInfo);
                TestAssertTrue("Можно бронировать", flight.CanBeBooked());

                // Тест 2: Бронирование мест
                int initialSeats = flight.AvailableSeats;
                flight.BookSeat();
                TestAssertEqual("Бронирование места", initialSeats - 1, flight.AvailableSeats);

                // Тест 3: Отмена места
                flight.CancelSeat();
                TestAssertEqual("Отмена места", initialSeats, flight.AvailableSeats);

                // Тест 4: Расчет дохода
                decimal revenue = flight.CalculateRevenue(50);
                TestAssertEqual("Расчет дохода", 5500.00m * 50, revenue);

                // Тест 5: Статус рейса
                flight.Status = "Delayed";
                TestAssertTrue("Рейс задержан", flight.IsDelayed());

                AddResult("✓ Все тесты рейсов пройдены");
            }
            catch (Exception ex)
            {
                AddResult($"✗ Тесты рейсов провалены: {ex.Message}");
            }
        }

        private void RunPassengerTests()
        {
            AddResult("\n=== ТЕСТЫ ДЛЯ ПАССАЖИРОВ ===");

            try
            {
                // Тест 1: Валидный пассажир
                var passenger = new Passenger
                {
                    FullName = "Иванов Иван Иванович",
                    PassportNumber = "4001 123456"
                };

                TestAssertTrue("Валидный пассажир", passenger.IsValid());
                TestAssertEqual("Инициалы", "И.И.", passenger.GetInitials());

                // Тест 2: Невалидный пассажир
                var invalidPassenger = new Passenger
                {
                    FullName = "",
                    PassportNumber = "123"
                };

                TestAssertFalse("Невалидный пассажир", invalidPassenger.IsValid());

                AddResult("✓ Все тесты пассажиров пройдены");
            }
            catch (Exception ex)
            {
                AddResult($"✗ Тесты пассажиров провалены: {ex.Message}");
            }
        }

        private void RunBookingTests()
        {
            AddResult("\n=== ТЕСТЫ ДЛЯ БРОНИРОВАНИЙ ===");

            try
            {
                // Тест 1: Создание бронирования
                var booking = new Booking(1, 1, "12A", 5500.00m);

                TestAssertEqual("Статус бронирования", "Confirmed", booking.Status);
                TestAssertTrue("Можно отменить", booking.CanBeCancelled());

                // Тест 2: Отмена бронирования
                booking.Cancel();
                TestAssertEqual("Статус после отмены", "Cancelled", booking.Status);
                TestAssertFalse("Нельзя отменить отмененное", booking.CanBeCancelled());

                AddResult("✓ Все тесты бронирований пройдены");
            }
            catch (Exception ex)
            {
                AddResult($"✗ Тесты бронирований провалены: {ex.Message}");
            }
        }

        private void RunBusinessLogicTests()
        {
            AddResult("\n=== ТЕСТЫ БИЗНЕС-ЛОГИКИ ===");

            try
            {
                // Тест 1: Туристический сезон
                var touristFlight = new Flight("SU 2001", "Москва", "Сочи",
                    DateTime.Now.AddHours(1), DateTime.Now.AddHours(4), 8500.00m, 180);
                touristFlight.IsTouristSeason = true;
                TestAssertTrue("Туристический сезон", touristFlight.IsTouristSeason);

                // Тест 2: Завершенный рейс
                var completedFlight = new Flight("SU 3001", "Москва", "Краснодар",
                    DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1), 7500.00m, 175);
                completedFlight.Status = "Completed";
                TestAssertTrue("Рейс завершен", completedFlight.IsCompleted());

                // Тест 3: Отмененный рейс нельзя бронировать
                var cancelledFlight = new Flight("SU 4001", "Москва", "Екатеринбург",
                    DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), 6500.00m, 200);
                cancelledFlight.Status = "Cancelled";
                TestAssertFalse("Отмененный рейс нельзя бронировать", cancelledFlight.CanBeBooked());

                // Тест 4: Рейс без свободных мест нельзя бронировать
                var fullFlight = new Flight("SU 5001", "Москва", "Новосибирск",
                    DateTime.Now.AddHours(3), DateTime.Now.AddHours(6), 9000.00m, 0);
                TestAssertFalse("Рейс без мест нельзя бронировать", fullFlight.CanBeBooked());

                AddResult("✓ Все тесты бизнес-логики пройдены");
            }
            catch (Exception ex)
            {
                AddResult($"✗ Тесты бизнес-логики провалены: {ex.Message}");
            }
        }

        private void TestAssertEqual<T>(string testName, T expected, T actual)
        {
            if (expected.Equals(actual))
            {
                AddResult($"✓ {testName}: ОК");
            }
            else
            {
                throw new Exception($"{testName}: ожидалось {expected}, получено {actual}");
            }
        }

        private void TestAssertTrue(string testName, bool condition)
        {
            if (condition)
            {
                AddResult($"✓ {testName}: ОК");
            }
            else
            {
                throw new Exception($"{testName}: ожидалось true");
            }
        }

        private void TestAssertFalse(string testName, bool condition)
        {
            if (!condition)
            {
                AddResult($"✓ {testName}: ОК");
            }
            else
            {
                throw new Exception($"{testName}: ожидалось false");
            }
        }

        private void AddResult(string message)
        {
            _testResults.Add(message);
        }
    }
}