using Xunit;
using AirlineSystem.Models;

namespace AirlineSystem.Tests
{
    public class PassengerTests
    {
        [Fact]
        public void Passenger_WithValidData_IsValid()
        {
            // Arrange
            var passenger = new Passenger
            {
                FullName = "Иванов Иван Иванович",
                PassportNumber = "4001 123456",
                Phone = "+79161234567",
                Email = "ivanov@mail.ru"
            };

            // Act
            bool isValid = passenger.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("", "4001 123456")]
        [InlineData("Иванов Иван", "")]
        [InlineData("Иванов Иван", "123")]
        [InlineData(null, "4001 123456")]
        public void Passenger_WithInvalidData_IsNotValid(string name, string passport)
        {
            // Arrange
            var passenger = new Passenger
            {
                FullName = name,
                PassportNumber = passport
            };

            // Act
            bool isValid = passenger.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("Иванов Иван Иванович", "И.И.")]
        [InlineData("Петров Петр", "П.П.")]
        [InlineData("Сидорова", "С")]
        public void GetInitials_ReturnsCorrectFormat(string fullName, string expectedInitials)
        {
            // Arrange
            var passenger = new Passenger
            {
                FullName = fullName
            };

            // Act
            string initials = passenger.GetInitials();

            // Assert
            Assert.Equal(expectedInitials, initials);
        }
    }
}