//using System.Runtime.InteropServices;
//using System.Windows;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	///enums
	public enum CustomInstructionType { Top, Bottom, Replace }
	//public enum EventActionType { General, Deployment, GroupManipulation, MapAndTokens }
	public enum EventActionSubtype { }
	public enum ThreatModifierType { None, Fixed, Multiple }
	public enum YesNoAll { Yes, No, All }
	public enum PriorityTargetType { Rebel, Hero, Ally, Other }
	public enum Expansion { Core }
	public enum EntityType { Tile, Console, Crate, DeploymentPoint, Token, Highlight, Door }
	public enum TokenShape { Circle, Square, Rectangle }
	public enum EventActionType { G1, G2, G3, G4, G5, G6, D1, D2, D3, D4, D5, GM1, GM2, GM3, M1, M2, M3, M4 }
	public enum ThreatAction { Add, Remove }
	public enum SourceType { InitialReserved, Manual, Hand }
	public enum DeploymentSpot { Active, Specific }
	public enum GroupType { All, Specific }
	public enum MarkerType { Neutral, Rebel, Imperial }

	public class ProjectItem
	{
		public string Title { get; set; }
		public string Date { get; set; }
		public string Description { get; set; }
		public string fileName { get; set; }
		public string relativePath { get; set; }
		public string fileVersion { get; set; }
	}

	public class DeploymentColor
	{
		public string name { get; set; }
		public Color color { get; set; }

		public DeploymentColor( string n, Color c )
		{
			name = n;
			color = c;
		}
	}

	public class Question
	{
		public string buttonText { get; set; }
		public Guid triggerGUID { get; set; }
	}

	public class DeploymentModifier
	{
		public string name { get; set; }
		public Guid GUID { get; set; }
		public bool isActive { get; set; }
		public string deploymentColor { get; set; }
	}

	public class ButtonAction
	{
		public string buttonText { get; set; }
		public Guid triggerGUID { get; set; }
		public string triggerName { get; set; }
	}

	public class DPData
	{
		public Guid GUID { get; set; }
	}

	public class EnemyGroupData : INotifyPropertyChanged
	{
		CustomInstructionType _customInstructionType;
		string _customText, _cardName, _cardID;

		public Guid GUID { get; set; }
		public string cardName { get { return _cardName; } set { _cardName = value; PC(); } }
		public string cardID { get { return _cardID; } set { _cardID = value; PC(); } }
		public CustomInstructionType customInstructionType { get { return _customInstructionType; } set { _customInstructionType = value; PC(); } }
		public string customText { get { return _customText; } set { _customText = value; PC(); } }
		public ObservableCollection<DPData> pointList { get; set; } = new();

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public EnemyGroupData()
		{

		}

		public EnemyGroupData( DeploymentCard dc, DeploymentPoint dp )
		{
			GUID = Guid.NewGuid();
			cardName = dc.name;
			cardID = dc.id;
			customText = "";
			customInstructionType = CustomInstructionType.Replace;
			for ( int i = 0; i < dc.size; i++ )
			{
				pointList.Add( new() { GUID = dp.GUID } );
			}
		}

		public void SetDP( Guid guid )
		{
			int c = pointList.Count;
			pointList.Clear();
			for ( int i = 0; i < c; i++ )
			{
				pointList.Add( new() { GUID = guid } );
			}
		}
	}

	public interface IPropertyModel { };
	public interface IMapEntity
	{
		Guid GUID { get; set; }
		string name { get; set; }
		EntityType entityType { get; set; }
		Vector entityPosition { get; set; }
		double entityRotation { get; set; }
		[JsonIgnore]
		EntityRenderer mapRenderer { get; set; }
		EntityProperties entityProperties { get; set; }
		Guid mapSectionOwner { get; set; }
		void BuildRenderer( Canvas c, Vector where, double scale );
		IMapEntity Duplicate();
		bool Validate();
	}
	public interface IEventActionDialog
	{
		IEventAction eventAction { get; set; }
	}
	public interface IEventAction
	{
		Guid GUID { get; set; }
		EventActionType eventActionType { get; set; }
		string displayName { get; set; }
	}

	//internal static class WindowExtensions
	//{
	//	// from winuser.h
	//	private const int GWL_STYLE = -16,
	//										WS_MAXIMIZEBOX = 0x10000,
	//										WS_MINIMIZEBOX = 0x20000;

	//	[DllImport( "user32.dll" )]
	//	extern private static int GetWindowLong( IntPtr hwnd, int index );

	//	[DllImport( "user32.dll" )]
	//	extern private static int SetWindowLong( IntPtr hwnd, int index, int value );

	//	internal static void HideMinimizeAndMaximizeButtons( this Window window )
	//	{
	//		IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper( window ).Handle;
	//		var currentStyle = GetWindowLong( hwnd, GWL_STYLE );

	//		SetWindowLong( hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX) );
	//	}
	//}

}
