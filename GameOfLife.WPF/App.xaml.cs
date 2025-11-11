using GameOfLife.Core.Services;
using GameOfLife.WPF.ViewModels;
using GameOfLife.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace GameOfLife.WPF
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<SimulationService>();
                    services.AddSingleton<FileService>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();

                    services.AddTransient<InitializationViewModel>();
                    services.AddTransient<InitializationWindow>();
                }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                await _host.StartAsync();

                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                Current.MainWindow = mainWindow;

                var initViewModel = _host.Services.GetRequiredService<InitializationViewModel>();
                var initWindow = _host.Services.GetRequiredService<InitializationWindow>();
                initWindow.DataContext = initViewModel;

                var dialogResult = initWindow.ShowDialog();

                if (dialogResult == true)
                {
                    var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();

                    mainViewModel.Initialize(
                        initViewModel.GetWidth(),
                        initViewModel.GetHeight(),
                        initViewModel.GetRuleString(),
                        initViewModel.GetAliveColor(),
                        initViewModel.GetDeadColor(),
                        initViewModel.GetShape());

                    mainWindow.WindowState = WindowState.Maximized;
                    mainWindow.Show();


                }
                else
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }
    }
}