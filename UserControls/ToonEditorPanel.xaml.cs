using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ToonEditorPanel.xaml
	/// </summary>
	public partial class ToonEditorPanel : UserControl, INotifyPropertyChanged
	{
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		DeploymentCard _selectedCopyFrom;

		public CustomToon customToon { get; set; }
		public DeploymentCard selectedCopyFrom { get => _selectedCopyFrom; set { _selectedCopyFrom = value; PC(); } }
		public bool isStandalone = false;
		public ObservableCollection<DeploymentColor> deploymentColors
		{
			get { return Utils.deploymentColors; }
		}

		public ToonEditorPanel() => InitializeComponent();

		public ToonEditorPanel( CustomToon ct = null )
		{
			InitializeComponent();

			customToon = ct;
			DataContext = customToon;

			propBox.Header = $"General Properties For '{customToon.deploymentCard.name}'";

			thumbListLV.ItemsSource = Utils.thumbnailData.Filter( ThumbType.All );
			thumbListLV.SelectedItem = customToon.thumbnail;

			copyFromCB.ItemsSource = Utils.enemyData.Where( x => !x.id.Contains( "TC" ) );

			tierCB.ItemsSource = new int[] { 1, 2, 3 };
			priorityCB.ItemsSource = new int[] { 1, 2 };

			try
			{
				instructionsBtn.Foreground = customToon.cardInstruction.content.Count == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				bonusBtn.Foreground = customToon.bonusEffect.effects.Count == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				abilityBtn.Foreground = customToon.deploymentCard.abilities.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				surgeBtn.Foreground = customToon.deploymentCard.surges.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				keywordsBtn.Foreground = customToon.deploymentCard.keywords.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				traitsText.Text = "None";
				if ( customToon.deploymentCard.traits.Length > 0 )
				{
					var t = customToon.deploymentCard.traits.Aggregate( ( acc, cur ) => acc + ", " + cur );
					traitsText.Text = t;
				}
				priorityTargetsText.Text = "No Priorities";
				if ( customToon.deploymentCard.preferredTargets?.Length > 0 )
					priorityTargetsText.Text = customToon.deploymentCard.preferredTargets.Select( x => x.ToString() ).Aggregate( ( acc, cur ) => acc.ToString() + ", " + cur.ToString() );
				heroSkillsText.Text = customToon.heroSkills.Count == 0 ? "None" : $"{customToon.heroSkills.Count} Skills";
			}
			catch ( Exception e )
			{
				Utils.ThrowErrorDialog( e, "Error trying to import character." );
			}
		}

		public void SetStandalone()
		{
			isStandalone = true;
			sizeWarningText.Visibility = Visibility.Collapsed;
		}

		private void textbox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void nameTB_TextChanged( object sender, TextChangedEventArgs e )
		{
			propBox.Header = $"General Properties For '{((TextBox)sender).Text}'";
		}

		private void instructionsBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( customToon.cardInstruction == null )
			{
				Utils.ShowError( "Instructions is null, setting a default value.  Try again." );
				customToon.cardInstruction = new() { instID = customToon.cardID, content = new() };
				return;
			}

			string s = "";
			List<string> ilist = new();
			if ( customToon.cardInstruction.content.Count > 0 )
			{
				foreach ( var item in customToon.cardInstruction.content )
				{
					s += item.instruction.Aggregate( ( acc, cur ) => acc + "\n" + cur );
					s += "\n===\n";
				}
				s = s.Substring( 0, s.LastIndexOf( "===" ) ).Trim();
			}

			var dlg = new GenericTextDialog( "EDIT INSTRUCTIONS", s );
			dlg.ShowDialog();
			if ( string.IsNullOrEmpty( dlg.theText.Trim() ) )
				customToon.cardInstruction.content = new();
			else
			{
				customToon.cardInstruction.content = new();
				var groups = dlg.theText.Trim().Split( "\n===\n" );
				foreach ( var item in groups )
				{
					customToon.cardInstruction.content.Add( new() { instruction = item.Trim().Split( "\n" ).ToList() } );
				}
			}

			instructionsBtn.Foreground = customToon.cardInstruction.content.Count == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void bonusBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( customToon.bonusEffect == null )
			{
				Utils.ShowError( "Bonus Effects is null, setting a default value.  Try again." );
				customToon.bonusEffect = new()
				{
					bonusID = customToon.cardID,
					effects = new()
				};
				return;
			}

			var s = string.Join( "\n", customToon.bonusEffect.effects );
			var dlg = new GenericTextDialog( "EDIT BONUSES", s );
			dlg.ShowDialog();

			if ( string.IsNullOrEmpty( dlg.theText.Trim() ) )
				customToon.bonusEffect = new BonusEffect()
				{
					bonusID = customToon.cardID,
					effects = new()
				};
			else
			{
				customToon.bonusEffect = new BonusEffect()
				{
					bonusID = customToon.cardID,
					effects = dlg.theText.Trim().Split( "\n" ).Select( x => x.Trim() ).ToList()
				};
			}

			bonusBtn.Foreground = customToon.bonusEffect.effects.Count == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void abilityBtn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				string s = "";
				if ( customToon.deploymentCard.abilities.Length > 0 )
				{
					s = customToon.deploymentCard.abilities.Select( x => $"{x.name}:{x.text}" ).Aggregate( ( acc, cur ) => acc + "\n" + cur );
				}

				var dlg = new GenericTextDialog( "EDIT ABILITIES", s );
				dlg.ShowDialog();
				if ( string.IsNullOrEmpty( dlg.theText.Trim() ) )
					customToon.deploymentCard.abilities = new GroupAbility[0];
				else
				{
					var array = dlg.theText.Trim().Split( "\n" );//get each line of text
					var list = new List<GroupAbility>();
					foreach ( var item in array )
					{
						var a = item.Trim().Split( ":" );//split into name and text
						list.Add( new() { name = a[0].Trim(), text = a[1].Trim() } );
					}
					customToon.deploymentCard.abilities = list.ToArray();
				}
			}
			catch ( Exception ex )
			{
				MessageBox.Show( $"There was a problem parsing the text.  Did you format the text correctly?\n\nException:\n{ex.Message}", "Parsing Error" );
			}

			abilityBtn.Foreground = customToon.deploymentCard.abilities.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void surgeBtn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				string s = "";
				if ( customToon.deploymentCard.surges.Length > 0 )
					s = string.Join( "\n", customToon.deploymentCard.surges );

				var dlg = new GenericTextDialog( "EDIT SURGES", s );
				dlg.ShowDialog();
				if ( string.IsNullOrEmpty( dlg.theText.Trim() ) )
					customToon.deploymentCard.surges = new string[0];
				else
					customToon.deploymentCard.surges = dlg.theText.Split( "\n" ).Select( x => x.Trim() ).ToArray();
			}
			catch ( Exception ex )
			{
				MessageBox.Show( $"There was a problem parsing the text.  Did you format the text correctly?\n\nException:\n{ex.Message}", "Parsing Error" );
			}

			surgeBtn.Foreground = customToon.deploymentCard.surges.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void keywordsBtn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				string s = "";
				if ( customToon.deploymentCard.keywords.Length > 0 )
					s = string.Join( "\n", customToon.deploymentCard.keywords );

				var dlg = new GenericTextDialog( "EDIT KEYWORDS", s );
				dlg.ShowDialog();
				if ( string.IsNullOrEmpty( dlg.theText.Trim() ) )
					customToon.deploymentCard.keywords = new string[0];
				else
					customToon.deploymentCard.keywords = dlg.theText.Split( "\n" ).Select( x => x.Trim() ).ToArray();
			}
			catch ( Exception ex )
			{
				MessageBox.Show( $"There was a problem parsing the text.  Did you format the text correctly?\n\nException:\n{ex.Message}", "Parsing Error" );
			}

			keywordsBtn.Foreground = customToon.deploymentCard.keywords.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void targetBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( customToon.deploymentCard.preferredTargets == null )
			{
				Utils.ShowError( "Bonus Effects is null, setting a default value.  Try again." );
				customToon.deploymentCard.preferredTargets = new GroupTraits[0];
				return;
			}

			//convert GroupTraits to GroupPriorityTraits from the deployment card
			GroupPriorityTraits traits = new() { useDefaultPriority = false };
			traits.FromArray( customToon.deploymentCard.preferredTargets );
			var dlg = new PriorityTraitsDialog( traits, true );
			dlg.ShowDialog();
			customToon.deploymentCard.preferredTargets = dlg.priorityTraits.GetTraitArray().Select( x => (GroupTraits)Enum.Parse( typeof( GroupTraits ), x ) ).ToArray();
			priorityTargetsText.Text = "No Priorities";
			if ( customToon.deploymentCard.preferredTargets.Length > 0 )
				priorityTargetsText.Text = customToon.deploymentCard.preferredTargets.Select( x => x.ToString() ).Aggregate( ( acc, cur ) => acc.ToString() + ", " + cur.ToString() );
		}

		private void filterAllButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.All );
		}

		private void filterRebelButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.Rebel );
		}

		private void filterImperialButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.Imperial );
		}

		private void filterMercButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.Mercenary );
		}

		private void filterStockImperialButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockImperial );
		}

		private void filterHeroButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockHero );
		}

		private void filterVillainButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockVillain );
		}

		private void filterAllyButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockAlly );
		}

		private void SetThumbSource( ThumbType ttype )
		{
			iconFilterBox.Text = "";

			thumbListLV.SelectionChanged -= thumbListLV_SelectionChanged;
			thumbListLV.ItemsSource = Utils.thumbnailData.Filter( ttype );
			thumbListLV.SelectionChanged += thumbListLV_SelectionChanged;
		}

		public void SetThumbnailImage()
		{
			var item = Utils.thumbnailData.Filter( ThumbType.All ).Where( x => x.ID == customToon.thumbnail.ID ).FirstOrDefault();
			thumbListLV.SelectedItem = item;
			thumbPreview.Source = new BitmapImage( new Uri( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Thumbnails/{customToon.thumbnail.ID.ThumbFolder()}/{customToon.thumbnail.ID}.png" ) );
		}

		private void confirmCopyFromButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedCopyFrom != null )
			{
				var dg = Utils.enemyData.Where( x => x.id == selectedCopyFrom.id ).FirstOr( null );
				customToon.CopyFrom( dg );
				//check if card text is empty now
				instructionsBtn.Foreground = customToon.cardInstruction.content.Count == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				bonusBtn.Foreground = customToon.bonusEffect.effects.Count == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				abilityBtn.Foreground = customToon.deploymentCard.abilities.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				surgeBtn.Foreground = customToon.deploymentCard.surges.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				keywordsBtn.Foreground = customToon.deploymentCard.keywords.Length == 0 ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
				traitsText.Text = "None";
				if ( customToon.deploymentCard.traits.Length > 0 )
				{
					var t = customToon.deploymentCard.traits.Aggregate( ( acc, cur ) => acc + ", " + cur );
					traitsText.Text = t;
				}
				priorityTargetsText.Text = "No Priorities";
				if ( customToon.deploymentCard.preferredTargets.Length > 0 )
					priorityTargetsText.Text = customToon.deploymentCard.preferredTargets.Select( x => x.ToString() ).Aggregate( ( acc, cur ) => acc.ToString() + ", " + cur.ToString() );
			}
		}

		private void editTraitsButton_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new CharacterTraitsDialog( customToon.deploymentCard.traits );
			dlg.ShowDialog();
			customToon.deploymentCard.traits = dlg.GetTraitStringArray();
			traitsText.Text = "None";
			if ( customToon.deploymentCard.traits.Length > 0 )
			{
				var t = customToon.deploymentCard.traits.Aggregate( ( acc, cur ) => acc + ", " + cur );
				traitsText.Text = t;
			}
		}

		private void charHelpButton_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new CharacterHelpWindow( isStandalone );
			dlg.ShowDialog();
		}

		private void iconFilterBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			if ( string.IsNullOrEmpty( iconFilterBox.Text ) )
				return;

			var fthumbs = Utils.thumbnailData.Filter( ThumbType.All ).Where( x => x.Name.ToLower().Contains( iconFilterBox.Text.ToLower() ) );
			if ( fthumbs == null )
				return;

			//set custom filtered CB source
			thumbListLV.SelectionChanged -= thumbListLV_SelectionChanged;
			thumbListLV.ItemsSource = fthumbs;
			thumbListLV.SelectionChanged += thumbListLV_SelectionChanged;

			//select first one found
			if ( fthumbs.FirstOr( null ) != null )
			{
				thumbListLV.SelectionChanged -= thumbListLV_SelectionChanged;
				thumbListLV.SelectedItem = fthumbs.First();
				thumbListLV.SelectionChanged += thumbListLV_SelectionChanged;
				customToon.thumbnail = fthumbs.First();
				customToon.deploymentCard.mugShotPath = $"CardThumbnails/{customToon.thumbnail.ID}";
				SetThumbnailImage();
			}
		}

		private void iconFilterBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				if ( string.IsNullOrEmpty( iconFilterBox.Text ) )
					return;

				var fthumb = Utils.thumbnailData.Filter( ThumbType.All ).Where( x => x.Name.ToLower().Contains( iconFilterBox.Text.ToLower() ) ).FirstOr( null );
				if ( fthumb != null )
				{
					thumbListLV.SelectedItem = fthumb;
					customToon.thumbnail = fthumb;
					customToon.deploymentCard.mugShotPath = $"CardThumbnails/{customToon.thumbnail.ID}";
					SetThumbnailImage();
					iconFilterBox.Text = "";
					SetThumbSource( ThumbType.All );
				}
			}
		}

		private void eliteCheckbox_Click( object sender, RoutedEventArgs e )
		{
			if ( eliteCheckbox.IsChecked == true && customToon.deploymentCard.isElite )
				customToon.deploymentCard.deploymentOutlineColor = "Red";
		}

		private void editHeroSkillsButton_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new EditCustomHeroSkillsWindow( customToon );
			dlg.ShowDialog();
			heroSkillsText.Text = customToon.heroSkills.Count == 0 ? "None" : $"{customToon.heroSkills.Count} Skills";
		}

		private void thumbListLV_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			customToon.thumbnail = thumbListLV.SelectedItem as Thumbnail;
			customToon.deploymentCard.mugShotPath = $"CardThumbnails/{customToon.thumbnail.ID}";
			SetThumbnailImage();
			iconFilterBox.Text = "";
		}
	}
}
