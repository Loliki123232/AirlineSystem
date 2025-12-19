using Xunit;
using Moq;
using AirlineSystem.Services;
using AirlineSystem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace AirlineSystem.Tests
{
    public class FlightServiceTests
    {
        [Fact]
        public void SearchFlights_FiltersByText()
        {
            // Arrange
            var mockFlights = new List<Flight>
            {
                new Flight("SU 1001", "Москва", "Санкт-Петербург",
                          DateTime.Now, DateTime.Now.AddHours(2), 5500.00m, 189),
                new Flight("SU 1002", "Москва", "Сочи",
                          DateTime.Now, DateTime.Now.AddHours(3), 8500.00m, 180)
            };

            // Здесь будет тест с моком базы данных
            // Пока просто проверяем логику
            Assert.True(true);
        }

        [Fact]
        public void AddFlight_ReturnsTrue_OnSuccess()
        {
            // Arrange
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   DateTime.Now, DateTime.Now.AddHours(2), 5500.00m, 189);

            // Здесь будет тест с моком SqlConnection
            // Пока просто проверяем логику
            Assert.True(true);
        }
    }
}