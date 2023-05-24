using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfGomokuGameClient.Services;
using WpfGomokuGameClient.Utilities;
using WpfGomokuGameClient.ViewModels;
using WpfGomokuGameClient.Views;

namespace WpfGomokuGameClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider Services { get; } =
            ConfigureServices();

        private static ServiceProvider ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            // 解决循环依赖的懒加载服务
            services
                .AddSingleton(typeof(LazyService<>));

            // Logic
            services
                .AddSingleton<GomokuService>()
                .AddSingleton<NotifyService>();

            // UI
            services
                .AddSingleton<MainWindow>()
                .AddSingleton<RegisterPage>()
                .AddSingleton<GamePage>()
                .AddSingleton<RegisterViewModel>()
                .AddSingleton<GameViewModel>();

            return services.BuildServiceProvider();
        }

        public static TService GetService<TService>() where TService : class
        {
            return Services.GetRequiredService<TService>();
        }

        public static void NavigateToPage(Type pageType)
        {
            if (Application.Current.MainWindow is not MainWindow mainWindow)
                throw new InvalidOperationException("Application is not started yet");

            mainWindow.Navigate(Services.GetService(pageType));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Application.Current.MainWindow is not MainWindow mainWindow)
                mainWindow = GetService<MainWindow>();

            mainWindow.Show();
            mainWindow.Navigate(GetService<RegisterPage>());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (Application.Current.MainWindow is MainWindow mainWindow)
                mainWindow.Close();
        }
    }
}
