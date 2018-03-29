using System;

namespace Cinary.Xamarin.Reactive.Event
{
    public class ScrollEventArgs : EventArgs
    {
        public int FirstVisibleItem { get; set; }
        public int VisibleItemCount { get; set; }
        public int TotalItemCount { get; set; }
        public int ScrollX { get; set; }
        public int ScrollY { get; set; }
        public double ScrollYDiff { get; set; }
        public bool IsFirstRow { get; set; }
        public bool IsLastRow { get; set; }
        public ScrollDirection Direction { get; set; }
    }
}
