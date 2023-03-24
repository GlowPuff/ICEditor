using System;
using System.Collections.Generic;
using System.Linq;
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

		/// <summary>
		/// Returns asset folder based on ID of mugshot
		/// </summary>
		public static string ThumbFolder( this string id )
		{
			if ( id.ToLower().Contains( "customimperial" ) )
				return "Imperial";
			else if ( id.ToLower().Contains( "customrebel" ) )
				return "Rebel";
			else if ( id.ToLower().Contains( "custommercenary" ) )
				return "Mercenary";
			else if ( id.ToLower().Contains( "stockhero" ) )
				return "StockHero";
			else if ( id.ToLower().Contains( "stockally" ) )
				return "StockAlly";
			else if ( id.ToLower().Contains( "stockimperial" ) )
				return "StockImperial";
			else if ( id.ToLower().Contains( "stockvillain" ) )
				return "StockVillain";
			else
				return "Other";
		}

		/// <summary>
		/// Extract the non-zero digit(s) from a DG ID, ie M001 = 1
		/// </summary>
		public static string GetDigits( this string s )
		{
			string s1 = new string( s.Where( Char.IsDigit ).ToArray() );
			//remove leading zeroes
			return s1.TrimStart( new Char[] { '0' } );
			//return new String( s.Where( Char.IsDigit ).Where( x => x != '0' ).ToArray() );
		}

		/// <summary>
		/// Strip the digits, return the letters
		/// </summary>
		public static string StripDigits( this string s )
		{
			return new string( s.Where( Char.IsLetter ).ToArray() );
		}
	}
}
