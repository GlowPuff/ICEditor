using System;

namespace Imperial_Commander_Editor
{
	public class MapManagement : EventAction
	{
		public Guid mapTile { get; set; }
		public Guid mapTileRemove { get; set; }
		public Guid mapSection { get; set; }
		public Guid mapSectionRemove { get; set; }

		public MapManagement()
		{

		}

		public MapManagement( string dname
			, EventActionType et ) : base( et, dname )
		{
			mapSection = Guid.Empty;
			mapSectionRemove = Guid.Empty;
			mapTile = Guid.Empty;
			mapTileRemove = Guid.Empty;
		}
	}
}
