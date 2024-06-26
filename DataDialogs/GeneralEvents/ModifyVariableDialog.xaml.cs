﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyVariableDialog.xaml
	/// </summary>
	public partial class ModifyVariableDialog : Window, IEventActionDialog
	{
		public static MainWindow mainWindow { get { return Utils.mainWindow; } }
		public IEventAction eventAction { get; set; }
		public Trigger selectedTrigger { get; set; }
		public ObservableCollection<TriggerModifier> selectedTriggers { get; set; } = new();

		public ModifyVariableDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ModifyVariable( dname, et );
			DataContext = this;

			//validate triggers
			bool found = false;
			List<string> strings = new();
			for ( int i = (eventAction as ModifyVariable).triggerList.Count - 1; i >= 0; i-- )
			{
				if ( !Utils.ValidateTrigger( (eventAction as ModifyVariable).triggerList[i].triggerGUID ) )
				{
					found = true;
					strings.Add( $"{(eventAction as ModifyVariable).triggerList[i].triggerName}" );
					//(eventAction as ModifyVariable).triggerList[i].triggerGUID = Guid.Empty;
					//(eventAction as ModifyVariable).triggerList[i].triggerName = "None";
				}
			}
			if ( found )
			{
				MessageBox.Show( $"This Event Action is referencing one or more Triggers that no longer exist in the Mission.\n\n{string.Join( "\n", strings )}", "Missing Reference(s) Found" );
			}
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void addTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedTrigger != null && !(eventAction as ModifyVariable).triggerList.Any( x => x.triggerGUID == selectedTrigger.GUID ) )
			{
				(eventAction as ModifyVariable).triggerList.Add( new TriggerModifier( selectedTrigger ) );
				selectedTriggers.Add( new TriggerModifier( selectedTrigger ) );
			}
		}

		private void remTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ModifyVariable).triggerList.Remove( ((sender as FrameworkElement).DataContext as TriggerModifier) );
			selectedTriggers.Remove( (sender as FrameworkElement).DataContext as TriggerModifier );
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null && !(eventAction as ModifyVariable).triggerList.Any( x => x.triggerGUID == t.GUID ) )
			{
				(eventAction as ModifyVariable).triggerList.Add( new TriggerModifier( t ) );
				selectedTriggers.Add( new TriggerModifier( t ) );
				tlist.ItemsSource = Utils.mainWindow.allMissionTriggers;
				tlist.SelectedValue = t;
			}
		}
	}
}
