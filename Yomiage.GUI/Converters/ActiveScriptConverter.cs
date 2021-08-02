using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Yomiage.GUI.ViewModels;

namespace Yomiage.GUI.Converters
{
    public class ActiveScriptConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is MainTextViewModel)
				return value;

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is MainTextViewModel)
				return value;

			return Binding.DoNothing;
		}
	}
}
