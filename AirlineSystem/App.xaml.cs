using System;
using System.Diagnostics;
using System.Windows;

namespace AirlineSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Запускаем TDD тесты
            RunTddTests();

            // Создаем и показываем главное окно
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void RunTddTests()
        {
            try
            {
                Debug.WriteLine("=== ЗАПУСК TDD ТЕСТОВ ===");

                var testRunner = new TestRunner();
                bool allTestsPassed = testRunner.RunAllTests();

                if (allTestsPassed)
                {
                    Debug.WriteLine("=== ВСЕ TDD ТЕСТЫ ПРОЙДЕНЫ УСПЕШНО ===");
                    MessageBox.Show("Все TDD тесты пройдены успешно!", "Тестирование",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Debug.WriteLine("=== НЕКОТОРЫЕ TDD ТЕСТЫ НЕ ПРОШЛИ ===");
                    MessageBox.Show("Некоторые TDD тесты не прошли. Проверьте Output для деталей.",
                        "Тестирование", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при запуске тестов: {ex.Message}");
                MessageBox.Show($"Ошибка при запуске тестов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}