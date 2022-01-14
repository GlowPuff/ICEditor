using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for DefaultWindowStyle.xaml
	/// </summary>
	public partial class DefaultWindowStyle : UserControl
	{
		public DefaultWindowStyle()
		{
			InitializeComponent();
		}

		public object AdditionalContent
		{
			get { return (object)GetValue( AdditionalContentProperty ); }
			set { SetValue( AdditionalContentProperty, value ); }
		}

		public object header
		{
			get { return (object)GetValue( headerProperty ); }
			set { SetValue( headerProperty, value ); }
		}

		public static readonly DependencyProperty AdditionalContentProperty =
				DependencyProperty.Register( "AdditionalContent", typeof( object ), typeof( DefaultWindowStyle ),
					new PropertyMetadata( null ) );

		public static readonly DependencyProperty headerProperty =
				DependencyProperty.Register( "header", typeof( string ), typeof( DefaultWindowStyle ),
					new PropertyMetadata( null ) );
	}
}
