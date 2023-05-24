using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfGomokuGameClient.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? serverAddress;
    }
}
