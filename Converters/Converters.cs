using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public class BoolToScrollbarVisibilityConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( (bool)value )
				return ScrollBarVisibility.Auto;
			else
				return ScrollBarVisibility.Hidden;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	public class ActiveSectionColorConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( (bool)value )
				return new SolidColorBrush( Colors.Purple );
			else
				return new SolidColorBrush( Colors.MediumPurple );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	public class RadioBoolToIntConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return value?.Equals( parameter );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return value?.Equals( true ) == true ? parameter : Binding.DoNothing;
		}
	}

	public class MissionSubTypeConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return ((ObservableCollection<MissionSubType>)value).Contains( (MissionSubType)parameter );
		}
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var st = Utils.mainWindow.mission.missionProperties.missionSubType;
			if ( !st.Contains( (MissionSubType)parameter ) )
				st.Add( (MissionSubType)parameter );
			else
				st.Remove( (MissionSubType)parameter );
			return st;
		}
	}

	public class TileSideConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return (string)value == "A" ? false : true;
			;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return (bool)value == true ? "B" : "A";
		}
	}
}
