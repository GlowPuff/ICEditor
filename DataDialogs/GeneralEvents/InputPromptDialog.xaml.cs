﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for InputPromptDialog.xaml
	/// </summary>
	public partial class InputPromptDialog : Window
	{
		public InputPromptDialog()
		{
			InitializeComponent();
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void Window_ContentRendered( object sender, EventArgs e )
		{
			textbox.Focus();
			textbox.SelectAll();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void remInputButton_Click( object sender, RoutedEventArgs e )
		{

		}
	}
}
