using System;

namespace Cinary.Xamarin.Reactive.Event
{
    public class ScrollStateChangedEventArgs : EventArgs
    {
        public ScrollState ScrollState { get; set; }
    }
}