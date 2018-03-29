using System;

namespace Cinary.Xamarin.Reactive.Event
{
    public class ScrollStateChangeEventArgs : EventArgs
    {
        public int ScrollX { get; set; }
        public int ScrollY { get; set; }
        public int OldScrollX { get; set; }
        public int OldScrollY { get; set; }
    }
}
