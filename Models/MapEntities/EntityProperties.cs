using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class EntityProperties : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public string name { get; set; }
		public bool isActive { get; set; }
		public string theText { get; set; }
		public ObservableCollection<ButtonAction> buttonActions { get; set; } = new();

		public EntityProperties()
		{
			isActive = true;
		}

		public void CopyFrom( IMapEntity me )
		{
			name = me.name;
			isActive = me.entityProperties.isActive;
			theText = me.entityProperties.theText;
			foreach ( ButtonAction ba in me.entityProperties.buttonActions )
				buttonActions.Add( ba );
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
	}
}
