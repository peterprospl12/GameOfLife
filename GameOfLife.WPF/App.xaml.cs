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
            try
            {
                await _host.StartAsync();
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
                base.OnStartup(e);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Startup error: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                await _host.StopAsync();
                base.OnExit(e);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Exit error: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _host.Dispose();
            }
        }
    }
}