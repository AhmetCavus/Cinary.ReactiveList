//
//  ReactiveListRenderer.cs
//
//  Author:
//       ahc <ahmet.cavus@cinary.com>
//
//  Copyright (c) 2017 (c) Ahmet Cavus
using System;
using Cinary.Xamarin.Reactive;
using Cinary.Xamarin.Reactive.Event;
using Cinary.Xamarin.UWP.Reactive;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ReactiveList), typeof(ReactiveListRenderer))]
namespace Cinary.Xamarin.UWP.Reactive
{
    public class ReactiveListRenderer : ListViewRenderer
	{
        #region Attributes

        ReactiveList _listInstance;

        #endregion

        #region Constructor

        #endregion

        #region Event Handler

        protected override void OnElementChanged(ElementChangedEventArgs<global::Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null) UnregisterEventHandler(e);
			if (e.NewElement != null) RegisterEventHandler(e);
        }

        ScrollViewer _scrollViewer;
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Assumes default template
            _scrollViewer = GetScrollViewer(Control);
            _scrollViewer.ViewChanging += OnScrollViewerViewChanging;
            _scrollViewer.DirectManipulationStarted += OnScrollViewerDirectManipulationStarted;
            _scrollViewer.DirectManipulationCompleted += OnScrollViewerDirectManipulationCompleted;

            // Not needed any more
            SizeChanged -= OnSizeChanged;
        }

        private void OnScrollViewerDirectManipulationCompleted(object sender, object e)
        {
            _listInstance.OnScrollStateChanged(this, ScrollState.Idle);
        }

        private void OnScrollViewerDirectManipulationStarted(object sender, object e)
        {
            _listInstance.OnScrollStateChanged(this, ScrollState.TouchScroll);
        }

        double _scrollY;
        int _prevIndex;
        int _prevViewPos;
        int _prevViewHeight;
        ScrollDirection _direction;
        ScrollEventArgs _scrollEventArgs = new ScrollEventArgs();
        void OnScrollViewerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (!_listInstance.IsListenerActive) return;
            try
            {
                var scrollYDiff = _scrollViewer.VerticalOffset - _scrollY;
               if(scrollYDiff >= 0)
                {
                    _direction = ScrollDirection.Down;
                } else
                {
                    _direction = ScrollDirection.Up;
                }
                _scrollY = _scrollViewer.VerticalOffset;
                _scrollEventArgs.ScrollY = (int) _scrollY;
                _scrollEventArgs.ScrollYDiff = scrollYDiff;
                _scrollEventArgs.Direction = _direction;

                _listInstance.OnScroll(this, _scrollEventArgs);
                _listInstance.OnScrollStateChanged(this, ScrollState.Fling);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err);
            }
            //System.Diagnostics.Debug.WriteLine($"ScrollViewer_ViewChanging {_scrollViewer.VerticalOffset} {_scrollViewer.ActualHeight} {_scrollViewer.DesiredSize} {_scrollViewer.ExtentHeight}");
        }

        #endregion

        #region Private Methods

        void RegisterEventHandler(ElementChangedEventArgs<global::Xamarin.Forms.ListView> e)
		{
			_listInstance = (ReactiveList) e.NewElement;
            SizeChanged += OnSizeChanged;
        }

        void UnregisterEventHandler(ElementChangedEventArgs<global::Xamarin.Forms.ListView> e)
		{
            SizeChanged -= OnSizeChanged;
            _scrollViewer.ViewChanging -= OnScrollViewerViewChanging;
            _scrollViewer.DirectManipulationStarted -= OnScrollViewerDirectManipulationStarted;
            _scrollViewer.DirectManipulationCompleted -= OnScrollViewerDirectManipulationCompleted;
        }

        #endregion

        #region Helper

        ScrollViewer GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            {
                return o as ScrollViewer;
            }

            var child = VisualTreeHelper.GetChild(o, 0);

            var result = GetScrollViewer(child);
            if (result == null)
            {
            }
            else
            {
                return result;
            }
            return null;
        }

        #endregion

    }

}
