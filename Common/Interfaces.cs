using System;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	//when notifying objects, they can either update their broken reference to Guid.None or just report they have a broken reference (NotifyMode.Report)
	public interface IHasEventReference
	{
		BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode );
		BrokenRefInfo SelfCheckEvents();
	}
	public interface IHasTriggerReference
	{
		BrokenRefInfo NotifyTriggerRemoved( Guid guid, NotifyMode mode );
	}
	public interface IHasEntityReference
	{
		BrokenRefInfo NotifyEntityRemoved( Guid guid, NotifyMode mode );
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
		bool hasProperties { get; }
		[JsonIgnore]
		bool hasColor { get; }
		[JsonIgnore]
		EntityRenderer mapRenderer { get; set; }
		EntityProperties entityProperties { get; set; }
		Guid mapSectionOwner { get; set; }
		void BuildRenderer( Canvas c, Vector where, double scale );
		IMapEntity Duplicate();
		bool Validate();
		void Dim( Guid guid );
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
}