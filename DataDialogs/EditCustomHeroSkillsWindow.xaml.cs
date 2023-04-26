using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EditCustomHeroSkillsWindow.xaml
	/// </summary>
	public partial class EditCustomHeroSkillsWindow : Window, INotifyPropertyChanged
	{
		CampaignSkill _selectedSkill;
		CustomToon customToon;

		public event PropertyChangedEventHandler PropertyChanged;

		public CampaignSkill selectedSkill { get => _selectedSkill; set { _selectedSkill = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public EditCustomHeroSkillsWindow( CustomToon toon )
		{
			InitializeComponent();

			DataContext = customToon = toon;
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void removeSkillButton_Click( object sender, RoutedEventArgs e )
		{
			customToon.heroSkills.Remove( (sender as Control).DataContext as CampaignSkill );
		}

		private void addSkillButton_Click( object sender, RoutedEventArgs e )
		{
			customToon.heroSkills.Add( customToon.CreateSkill( skillName.Text, int.Parse( skillCost.Text ) ) );
			skillCost.Text = skillName.Text = string.Empty;
		}

		private void upBtn_Click( object sender, RoutedEventArgs e )
		{
			CampaignSkill ea = (sender as Control).DataContext as CampaignSkill;
			int idx = customToon.heroSkills.IndexOf( ea );
			customToon.heroSkills.Move( idx, Math.Max( 0, idx - 1 ) );
		}

		private void downBtn_Click( object sender, RoutedEventArgs e )
		{
			CampaignSkill ea = (sender as Control).DataContext as CampaignSkill;
			int idx = customToon.heroSkills.IndexOf( ea );
			customToon.heroSkills.Move( idx, Math.Min( customToon.heroSkills.Count - 1, idx + 1 ) );
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void skillName_TextChanged( object sender, TextChangedEventArgs e )
		{
			addSkillButton.IsEnabled = !string.IsNullOrEmpty( skillName.Text.Trim() ) && !string.IsNullOrEmpty( skillCost.Text.Trim() );
		}

		private void skillCost_TextChanged( object sender, TextChangedEventArgs e )
		{
			addSkillButton.IsEnabled = !string.IsNullOrEmpty( skillName.Text.Trim() ) && !string.IsNullOrEmpty( skillCost.Text.Trim() );
		}

		private void Window_ContentRendered( object sender, EventArgs e )
		{
			skillName.Focus();
		}
	}
}
