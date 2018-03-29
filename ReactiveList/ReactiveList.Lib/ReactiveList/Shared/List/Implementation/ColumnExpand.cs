//
//  ColumnExpand.cs
//
//  Author:
//       ahc <ahmet.cavus@cinary.com>
//
//  Copyright (c) 2018 (c) Ahmet Cavus
namespace Cinary.Xamarin.Reactive
{
	/// <summary>
    /// ReactiveList column expand mode.
	/// </summary>
	public enum ColumnExpand
	{
		/// <summary>
		/// None (default)
		/// </summary>
		None,

		/// <summary>
		/// Only first column is expanded
		/// </summary>
		First,

		/// <summary>
		/// Only last column is expanded
		/// </summary>
		Last,

		/// <summary>
		/// Columns are expanded proportionally
		/// </summary>
		Proportional,

		/// <summary>
		/// Columns are expanded proportionally
		/// First column expand more to keep columns parallel
		/// </summary>
		ProportionalFirst,

		/// <summary>
		/// Columns are expanded proportionally
		/// Last column expand more to keep columns parallel
		/// </summary>
		ProportionalLast
	}
}
