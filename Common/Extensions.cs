using System.Collections.Generic;
using System.Windows;

namespace Imperial_Commander_Editor
{
	public static class Extensions
	{
		public static Vector ToVector( this Point p )
		{
			return new Vector( p.X, p.Y );
		}

		public static T[] Fill<T>( this T[] arr, T value )
		{
			for ( int i = 0; i < arr.Length; i++ )
			{
				arr[i] = value;
			}
			return arr;
		}

		public static T FirstOr<T>( this IEnumerable<T> source, T alternate )
		{
			foreach ( T t in source )
				return t;
			return alternate;
		}
	}
}
