using AirlineSystem.Models;
using AirlineSystem.Services;
using System;
using System.Collections.Generic;

namespace AirlineSystem.Tests
{
    public class DbTests
    {
        private readonly string _testConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AirlineDB;Integrated Security=True";

        public void RunDatabaseTests()
        {
            Console.WriteLine("=== ТЕСТЫ БАЗЫ ДАННЫХ ===");

            TestFlightService();
            TestRevenueCalculator();

            Console.WriteLine("=== ТЕСТЫ БАЗЫ ДАННЫХ ЗАВЕРШЕНЫ ===");
        }

        private void TestFlightService()
        {
            try
            {
                var flightService = new FlightService(_testConnectionString);

                // Тест 1: Получение всех рейсов
                var flights = flightService.GetAllFlights();
                Console.WriteLine($"✓ Получено рейсов: {flights.Count}");

                // Тест 2: Поиск рейсов
                var searchResults = flightService.SearchFlights("SU", "Scheduled", null);
                Console.WriteLine($"✓ Найдено рейсов по поиску: {searchResults.Count}");

                Console.WriteLine("✓ Тесты FlightService пройдены");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тесты FlightService провалены: {ex.Message}");
            }
        }

        private void TestRevenueCalculator()
        {
            try
            {
                var calculator = new RevenueCalculator(_testConnectionString);

                // Тест 1: Расчет дохода за день
                var dailyRevenue = calculator.CalculateDailyRevenue(DateTime.Today);
                Console.WriteLine($"✓ Расчет дохода за день: {dailyRevenue.Count} рейсов");

                // Тест 2: Общий доход за период
                var totalRevenue = calculator.CalculateTotalRevenue(
                    DateTime.Today.AddDays(-30),
                    DateTime.Today.AddDays(30));
                Console.WriteLine($"✓ Общий доход за период: {totalRevenue:C}");

                Console.WriteLine("✓ Тесты RevenueCalculator пройдены");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тесты RevenueCalculator провалены: {ex.Message}");
            }
        }
    }
}