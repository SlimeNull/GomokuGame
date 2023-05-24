using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Input;
using WpfGomokuGameClient.Services;
using WpfGomokuGameClient.ViewModels;

namespace WpfGomokuGameClient.Views
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage(
            RegisterViewModel viewModel,
            GomokuService gomokuService)
        {
            ViewModel = viewModel;
            GomokuService = gomokuService;
            DataContext = this;
            InitializeComponent();
        }

        public RegisterViewModel ViewModel { get; }
        public GomokuService GomokuService { get; }

        [RelayCommand]
        public async Task Continue()
        {
            try
            {
                if (!Uri.TryCreate(ViewModel.ServerAddress, UriKind.Absolute, out var address))
                {
                    MessageBox.Show($"Invalid URL", "Failed to register", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                GomokuService.UpdateGomokuServer(address);
                await GomokuService.RegisterAsync();

                App.NavigateToPage(typeof(GamePage));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Failed to register", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
