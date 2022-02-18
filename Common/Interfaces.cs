using System;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
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