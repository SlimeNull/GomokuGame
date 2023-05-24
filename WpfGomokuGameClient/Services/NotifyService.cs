using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfGomokuGameClient.Services
{
    public partial class NotifyService : ObservableObject
    {
        [ObservableProperty]
        private string? text;
    }
}
