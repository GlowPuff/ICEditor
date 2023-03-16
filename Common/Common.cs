//using System.Runtime.InteropServices;
//using System.Windows;

namespace Imperial_Commander_Editor
{
	///enums
	public enum CustomInstructionType { Top, Bottom, Replace }
	public enum ThreatModifierType { None, Fixed, Multiple }
	public enum YesNoAll { Yes, No, All, Multi }
	public enum PriorityTargetType { Rebel, Hero, Ally, Other, Trait }
	public enum Expansion { Core, Twin, Hoth, Bespin, Jabba, Empire, Lothal }
	//public enum Expansion { Core, Bespin, Empire, Hoth, Jabba, Lothal, Twin }
	public enum EntityType { Tile, Console, Crate, DeploymentPoint, Token, Highlight, Door }
	public enum TokenShape { Circle, Square, Rectangle }
	public enum EventActionType { G1, G2, G3, G4, G5, G6, D1, D2, D3, D4, D5, GM1, GM2, GM3, M1, M2, G7, GM4, GM5, G8, G9, D6, GM6 }
	public enum ThreatAction { Add, Remove }
	public enum DeploymentSpot { Active, Specific }
	public enum GroupType { All, Specific }
	public enum MarkerType { Neutral, Rebel, Imperial }
	public enum MissionType { Story, Side, Forced }
	public enum MissionSubType { Agenda, Threat, Other, Finale, General, Personal, Villain, Ally }
	public enum DiceColor { White, Black, Yellow, Red, Green, Blue, Grey }
	public enum AttackType { Ranged, Melee, None }
	public enum FigureSize { Small1x1, Medium1x2, Large2x2, Huge2x3 }
	public enum GroupTraits { Trooper, Leader, HeavyWeapon, Guardian, Brawler, Droid, Vehicle, Hunter, Creature, Smuggler, Spy, ForceUser, Wookiee, Hero }
	public enum NotifyMode { Report, Fix }
	public enum NotifyType { Event, Trigger, Entity, StartingEvent, EventGroup, EntityGroup, InitialGroup }
	public enum CharacterType { Hero, Ally, Enemy, Villain, Rebel }
	public enum ThumbType { All, Other, Rebel, Imperial, Mercenary }
	public enum Factions { Imperial, Mercenary }

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
