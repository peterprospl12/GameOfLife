using System;
using System.Windows;
using GameOfLife.Core.Services;
using GameOfLife.WPF.ViewModels;
using GameOfLife.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameOfLife.WPF
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddSingleton<SimulationService>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                await _host.StartAsync();
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"An unrecoverable error occurred during startup: {exception.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5)); // Allow 5 seconds for graceful shutdown
            }
            base.OnExit(e);
        }
    }
}