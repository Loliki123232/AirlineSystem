using AirlineSystem.Models;
using AirlineSystem.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AirlineSystem
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly FlightService _flightService;
        private readonly RevenueCalculator _revenueCalculator;
        private List<Flight> _allFlights = new List<Flight>();
        private Flight? _selectedFlight;

        public Flight? SelectedFlight
        {
            get => _selectedFlight;
            set
            {
                _selectedFlight = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AirlineDB;Integrated Security=True";
            _flightService = new FlightService(connectionString);
            _revenueCalculator = new RevenueCalculator(connectionString);
            
            DataContext = this;
            LoadFlights();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadFlights()
        {
            try
            {
                _allFlights = _flightService.GetAllFlights();
                dgFlights.ItemsSource = _allFlights;
                tbRecordCount.Text = $"Записей: {_allFlights.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки рейсов: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateFlightForm())
                    return;

                var newFlight = CreateFlightFromForm();
                
                if (_flightService.AddFlight(newFlight))
                {
                    MessageBox.Show("Рейс успешно добавлен!", "Успех", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadFlights();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Не удалось добавить рейс", "Ошибка", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFlight == null || SelectedFlight.FlightID == 0)
            {
                MessageBox.Show("Выберите рейс для редактирования", "Внимание", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UpdateFlightFromForm(SelectedFlight);
                
                if (_flightService.UpdateFlight(SelectedFlight))
                {
                    MessageBox.Show("Рейс успешно обновлен!", "Успех", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadFlights();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить рейс", "Ошибка", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFlight == null || SelectedFlight.FlightID == 0)
            {
                MessageBox.Show("Выберите рейс для удаления", "Внимание", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить рейс {SelectedFlight.FlightNumber}?",
                                        "Подтверждение удаления",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (_flightService.DeleteFlight(SelectedFlight.FlightID))
                    {
                        MessageBox.Show("Рейс успешно удален!", "Успех", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadFlights();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить рейс", "Ошибка", 
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void DgFlights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFlights.SelectedItem is Flight flight)
            {
                SelectedFlight = flight;
                UpdateFormFromFlight(flight);
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

        // Вспомогательные методы
        private bool ValidateFlightForm()
        {
            if (string.IsNullOrWhiteSpace(txtFlightNumber.Text))
            {
                MessageBox.Show("Введите номер рейса", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtAvailableSeats.Text, out int seats) || seats < 0)
            {
                MessageBox.Show("Введите корректное количество мест", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private Flight CreateFlightFromForm()
        {
            DateTime departureDate = dpDepartureDate.SelectedDate ?? DateTime.Now;
            TimeSpan departureTime = TimeSpan.TryParse(txtDepartureTime.Text, out var time) ? time : TimeSpan.FromHours(8);
            DateTime departureDateTime = departureDate.Add(departureTime);

            return new Flight
            {
                FlightNumber = txtFlightNumber.Text,
                Status = ((ComboBoxItem)cmbStatus.SelectedItem)?.Content?.ToString() ?? "Scheduled",
                Price = decimal.Parse(txtPrice.Text),
                AvailableSeats = int.Parse(txtAvailableSeats.Text),
                IsTouristSeason = chkTouristSeason.IsChecked ?? false,
                DepartureTime = departureDateTime,
                ArrivalTime = departureDateTime.AddHours(2),
                RouteID = 1, // По умолчанию
                AircraftID = 1 // По умолчанию
            };
        }

        private void UpdateFormFromFlight(Flight flight)
        {
            txtFlightNumber.Text = flight.FlightNumber;
            txtPrice.Text = flight.Price.ToString();
            txtAvailableSeats.Text = flight.AvailableSeats.ToString();
            chkTouristSeason.IsChecked = flight.IsTouristSeason;
            dpDepartureDate.SelectedDate = flight.DepartureTime;
            txtDepartureTime.Text = flight.DepartureTime.ToString("HH:mm");

            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content?.ToString() == flight.Status)
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void UpdateFlightFromForm(Flight flight)
        {
            flight.FlightNumber = txtFlightNumber.Text;
            flight.Price = decimal.Parse(txtPrice.Text);
            flight.AvailableSeats = int.Parse(txtAvailableSeats.Text);
            flight.IsTouristSeason = chkTouristSeason.IsChecked ?? false;
            flight.Status = ((ComboBoxItem)cmbStatus.SelectedItem)?.Content?.ToString() ?? "Scheduled";
        }

        private void FilterFlights()
        {
            string statusFilter = ((ComboBoxItem)cmbStatusFilter.SelectedItem)?.Content?.ToString() ?? "";
            
            var filtered = _flightService.SearchFlights(
                txtSearchFlight.Text,
                statusFilter,
                dpDateFilter.SelectedDate
            );

            dgFlights.ItemsSource = filtered;
            tbRecordCount.Text = $"Записей: {filtered.Count} (отфильтровано из {_allFlights.Count})";
        }

        private void ClearForm()
        {
            SelectedFlight = null;
            txtFlightNumber.Text = "";
            cmbStatus.SelectedIndex = 0;
            txtPrice.Text = "";
            txtAvailableSeats.Text = "";
            chkTouristSeason.IsChecked = false;
            dpDepartureDate.SelectedDate = DateTime.Now;
            txtDepartureTime.Text = "08:00";
        }
    }
}