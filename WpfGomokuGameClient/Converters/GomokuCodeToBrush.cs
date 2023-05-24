using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfGomokuGameClient.Converters
{
    public class GomokuCodeToBrushConverter : IValueConverter
    {
        private readonly SolidColorBrush[] brushes =
            new SolidColorBrush[]
            {
                Brushes.Transparent,
                Brushes.Black,
                Brushes.White,
                Brushes.Red,
                Brushes.Green,
                Brushes.Blue,
                Brushes.AliceBlue,
                Brushes.AntiqueWhite,
                Brushes.Aqua,
                Brushes.Aquamarine,
                Brushes.Azure,
                Brushes.Beige,
                Brushes.Bisque,
                Brushes.BlanchedAlmond,
                Brushes.BlueViolet,
                Brushes.Brown,
                Brushes.BurlyWood,
                Brushes.CadetBlue,
                Brushes.Chartreuse,
                Brushes.Chocolate,
                Brushes.Coral,
                Brushes.Crimson, 
                Brushes.Maroon,
                Brushes.Silver,
                Brushes.LightBlue,
                Brushes.SkyBlue,
                Brushes.SpringGreen
            };

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int code || code < 0 || code > 26)
                return null;
            if (!targetType.IsAssignableFrom(typeof(SolidColorBrush)))
                return null;

            return brushes[code];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
