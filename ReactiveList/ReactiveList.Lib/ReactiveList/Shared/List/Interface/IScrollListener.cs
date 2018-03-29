//
//  IScrollListener.cs
//
//  Author:
//       ahc <ahmet.cavus@cinary.com>
//
//  Copyright (c) 2018 (c) Ahmet Cavus
using Cinary.Xamarin.Reactive.Event;

namespace Cinary.Xamarin.Reactive
{
	public interface IScrollListener
	{

		void OnScroll(object sender, ScrollEventArgs e);

		void OnScrollStateChange(object sender, ScrollStateChangeEventArgs e);

		void OnScrollStateChanged(object sender, ScrollStateChangedEventArgs e);

	}
}
