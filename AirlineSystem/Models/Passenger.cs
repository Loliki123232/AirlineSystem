namespace AirlineSystem.Models
{
    public class Passenger
    {
        public int PassengerID { get; set; }
        public string FullName { get; set; } = "";
        public string PassportNumber { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(PassportNumber) &&
                   PassportNumber.Length >= 6;
        }

        public string GetInitials()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                return string.Empty;

            var parts = FullName.Split(' ');
            if (parts.Length >= 2)
                return $"{parts[0][0]}.{parts[1][0]}.";

            return FullName[0].ToString();
        }
    }
}