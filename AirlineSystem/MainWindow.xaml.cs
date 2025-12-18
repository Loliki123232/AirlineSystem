using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AirlineSystem
{
    public partial class MainWindow : Window
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AirlineDB;Integrated Security=True";
        private List<Flight> flights = new List<Flight>();
        private Flight selectedFlight = new Flight();

        public MainWindow()
        {
            InitializeComponent();
            LoadFlights();
            DataContext = this;
        }

        public Flight SelectedFlight
        {
            get { return selectedFlight; }
            set { selectedFlight = value; }
        }

        private void LoadFlights()
        {
            flights.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                f.FlightID,
                f.FlightNumber,
                r.DeparturePoint,
                r.DestinationPoint,
                f.DepartureTime,
                f.ArrivalTime,
                f.Status,
                f.Price,
                f.AvailableSeats,
                f.IsTouristSeason
            FROM Flights f
            INNER JOIN Routes r ON f.RouteID = r.RouteID
            ORDER BY f.DepartureTime";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();

                connection.Open();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    flights.Add(new Flight
                    {
                        FlightID = Convert.ToInt32(row["FlightID"]),
                        FlightNumber = row["FlightNumber"].ToString(),
                        DeparturePoint = row["DeparturePoint"].ToString(),
                        DestinationPoint = row["DestinationPoint"].ToString(),
                        DepartureTime = Convert.ToDateTime(row["DepartureTime"]),
                        ArrivalTime = Convert.ToDateTime(row["ArrivalTime"]),
                        Status = row["Status"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        AvailableSeats = Convert.ToInt32(row["AvailableSeats"]),
                        IsTouristSeason = Convert.ToBoolean(row["IsTouristSeason"])
                    });
                }
            }

            dgFlights.ItemsSource = flights;
            tbRecordCount.Text = $"Записей: {flights.Count}";
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO Flights (FlightNumber, RouteID, AircraftID, DepartureTime, ArrivalTime, Status, Price, AvailableSeats, IsTouristSeason)
                        VALUES (@FlightNumber, 1, 1, @DepartureTime, @ArrivalTime, @Status, @Price, @AvailableSeats, @IsTouristSeason)";

                    SqlCommand command = new SqlCommand(query, connection);

                    // Собираем дату и время
                    DateTime departureDate = dpDepartureDate.SelectedDate ?? DateTime.Now;
                    TimeSpan departureTime = TimeSpan.Parse(txtDepartureTime.Text);
                    DateTime departureDateTime = departureDate.Add(departureTime);

                    command.Parameters.AddWithValue("@FlightNumber", txtFlightNumber.Text);
                    command.Parameters.AddWithValue("@DepartureTime", departureDateTime);
                    command.Parameters.AddWithValue("@ArrivalTime", departureDateTime.AddHours(2));
                    command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
                    command.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));
                    command.Parameters.AddWithValue("@AvailableSeats", int.Parse(txtAvailableSeats.Text));
                    command.Parameters.AddWithValue("@IsTouristSeason", chkTouristSeason.IsChecked ?? false);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Рейс успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadFlights();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFlight == null || selectedFlight.FlightID == 0)
            {
                MessageBox.Show("Выберите рейс для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        UPDATE Flights 
                        SET FlightNumber = @FlightNumber,
                            Status = @Status,
                            Price = @Price,
                            AvailableSeats = @AvailableSeats,
                            IsTouristSeason = @IsTouristSeason
                        WHERE FlightID = @FlightID";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@FlightID", selectedFlight.FlightID);
                    command.Parameters.AddWithValue("@FlightNumber", txtFlightNumber.Text);
                    command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
                    command.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));
                    command.Parameters.AddWithValue("@AvailableSeats", int.Parse(txtAvailableSeats.Text));
                    command.Parameters.AddWithValue("@IsTouristSeason", chkTouristSeason.IsChecked ?? false);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Рейс успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadFlights();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFlight == null || selectedFlight.FlightID == 0)
            {
                MessageBox.Show("Выберите рейс для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить рейс {selectedFlight.FlightNumber}?",
                                        "Подтверждение удаления",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Проверяем, есть ли связанные бронирования
                        string checkQuery = "SELECT COUNT(*) FROM Bookings WHERE FlightID = @FlightID";
                        SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                        checkCommand.Parameters.AddWithValue("@FlightID", selectedFlight.FlightID);

                        connection.Open();
                        int bookingsCount = (int)checkCommand.ExecuteScalar();

                        if (bookingsCount > 0)
                        {
                            MessageBox.Show("Невозможно удалить рейс, так как есть связанные бронирования. Отмените статус рейса вместо удаления.",
                                          "Ошибка",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                            return;
                        }

                        // Удаляем рейс
                        string deleteQuery = "DELETE FROM Flights WHERE FlightID = @FlightID";
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                        deleteCommand.Parameters.AddWithValue("@FlightID", selectedFlight.FlightID);

                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Рейс успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadFlights();
                            ClearForm();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedFlight = new Flight();
            txtFlightNumber.Text = "";
            cmbStatus.SelectedIndex = 0;
            txtPrice.Text = "";
            txtAvailableSeats.Text = "";
            chkTouristSeason.IsChecked = false;
            dpDepartureDate.SelectedDate = DateTime.Now;
            txtDepartureTime.Text = "08:00";
        }

        private void DgFlights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFlights.SelectedItem != null)
            {
                selectedFlight = (Flight)dgFlights.SelectedItem;

                // Обновляем форму
                txtFlightNumber.Text = selectedFlight.FlightNumber;
                txtPrice.Text = selectedFlight.Price.ToString();
                txtAvailableSeats.Text = selectedFlight.AvailableSeats.ToString();
                chkTouristSeason.IsChecked = selectedFlight.IsTouristSeason;

                // Устанавливаем статус в комбобоксе
                foreach (ComboBoxItem item in cmbStatus.Items)
                {
                    if (item.Content.ToString() == selectedFlight.Status)
                    {
                        cmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void TxtSearchFlight_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterFlights();
        }

        private void CmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterFlights();
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearchFlight.Text = "";
            cmbStatusFilter.SelectedIndex = 0;
            dpDateFilter.SelectedDate = null;
            LoadFlights();
        }

        private void FilterFlights()
        {
            var filtered = flights.AsEnumerable();

            // Фильтр по тексту поиска
            if (!string.IsNullOrWhiteSpace(txtSearchFlight.Text))
            {
                string searchText = txtSearchFlight.Text.ToLower();
                filtered = filtered.Where(f =>
                    f.FlightNumber.ToLower().Contains(searchText) ||
                    f.DeparturePoint.ToLower().Contains(searchText) ||
                    f.DestinationPoint.ToLower().Contains(searchText));
            }

            // Фильтр по статусу
            if (cmbStatusFilter.SelectedIndex > 0)
            {
                string selectedStatus = ((ComboBoxItem)cmbStatusFilter.SelectedItem).Content.ToString();
                filtered = filtered.Where(f => f.Status == selectedStatus);
            }

            // Фильтр по дате
            if (dpDateFilter.SelectedDate.HasValue)
            {
                DateTime selectedDate = dpDateFilter.SelectedDate.Value.Date;
                filtered = filtered.Where(f => f.DepartureTime.Date == selectedDate);
            }

            dgFlights.ItemsSource = filtered.ToList();
            tbRecordCount.Text = $"Записей: {filtered.Count()} (отфильтровано из {flights.Count})";
        }
    }

    // Класс модели для рейса
    public class Flight
    {
        public int FlightID { get; set; }
        public string FlightNumber { get; set; }
        public string DeparturePoint { get; set; }
        public string DestinationPoint { get; set; }
        public string RouteInfo => $"{DeparturePoint} → {DestinationPoint}";
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public bool IsTouristSeason { get; set; }
    }
}