using System;
using System.Collections.Generic;
using System.Linq;
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
using WpfGomokuGameClient.Services;
using WpfGomokuGameClient.ViewModels;

namespace WpfGomokuGameClient.Views
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        public GamePage(
            GameViewModel viewModel,
            GomokuService gomokuService,
            NotifyService notifyService)
        {
            ViewModel = viewModel;
            GomokuService = gomokuService;
            NotifyService = notifyService;
            DataContext = this;
            InitializeComponent();

            RunGame();
        }

        public GameViewModel ViewModel { get; }
        public GomokuService GomokuService { get; }
        public NotifyService NotifyService { get; }

        public void RunGame()
        {
            Task.Run(async () =>
            {
                try
                {
                    await GomokuService.GameAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex}", "Game ended", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                    return;
                }
            });
        }
    }
}
