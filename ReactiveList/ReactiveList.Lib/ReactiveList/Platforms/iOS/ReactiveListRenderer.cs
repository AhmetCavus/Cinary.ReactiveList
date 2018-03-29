//
//  ReactiveListRenderer.cs
//
//  Author:
//       ahc <ahmet.cavus@cinary.com>
//
//  Copyright (c) 2018 (c) Ahmet Cavus
using System;
using System.Diagnostics;
using System.Linq;
using Cinary.Xamarin.iOS.Reactive;
using Cinary.Xamarin.Reactive;
using Cinary.Xamarin.Reactive.Event;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ReactiveList), typeof(ReactiveListRenderer))]
namespace Cinary.Xamarin.iOS.Reactive
{
    public class ReactiveListRenderer : ListViewRenderer
	{

        #region Attributes

        ReactiveList _listInstance;

		#endregion

		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged(e);
			UnregisterEventHandler(e);
			RegisterEventHandler(e);
            Control.AllowsSelection = false;
            try
            {
                Control.VisibleCells.Any((viewCell) =>
                {
                    if (Control.VisibleCells != null) viewCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    return false;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
		}

		#endregion

		#region Private Methods

		void UnregisterEventHandler(ElementChangedEventArgs<ListView> e) 
		{
			if (e.OldElement == null) return;
			try {
				_listInstance = (ReactiveList) e.OldElement;
				_listInstance.ClearEventHandlers();
				//_listInstance.Dispose();
			} catch(Exception err){
				Debug.WriteLine($"Cannot unsubscribe from listening events in iOS: {err.Message}");
			}
		}

		void RegisterEventHandler(ElementChangedEventArgs<ListView> e) 
		{
			if (e.NewElement == null) return;
			try{
				_listInstance = (ReactiveList) e.NewElement;
				Control.Delegate = new ScrollEventsDelegate(_listInstance);
				Control.ShowsVerticalScrollIndicator = false;
				Control.ShowsHorizontalScrollIndicator = false;
			} catch(Exception err) {
				Debug.WriteLine($"EventHandler for OptimizedList not registerable for iOS: {err.Message}");
			}
		}
		#endregion

	}

	#region Classes

	class ScrollEventsDelegate : UITableViewDelegate
	{
        ReactiveList _listInstance;
        ScrollEventArgs _scrollEventArgs = new ScrollEventArgs();

        public ScrollEventsDelegate(ReactiveList list)
		{
			_listInstance = list;
		}

        nfloat _lastMoveY;
		public override void Scrolled(UIScrollView scrollView)
		{
			try {
				if (!_listInstance.IsListenerActive) return;
				var uiTable = scrollView as UITableView;
				var visibleRows = uiTable.IndexPathsForVisibleRows;
				var firstVisibleItem = visibleRows[0].Row;
				var visibleItemCount = visibleRows.Count();
				var totalItemCount = firstVisibleItem + visibleItemCount;
				var scrollX = uiTable.ContentOffset.X;
				var scrollY = uiTable.ContentOffset.Y;
				//var scrollXDiff = 0;
				var scrollYDiff = scrollY - _lastMoveY;
				var direction = scrollYDiff < 0 ? ScrollDirection.Up : ScrollDirection.Down;

                _scrollEventArgs.FirstVisibleItem = firstVisibleItem;
                _scrollEventArgs.VisibleItemCount = visibleItemCount;
                _scrollEventArgs.TotalItemCount = totalItemCount;
                _scrollEventArgs.ScrollY = (int) scrollY;
                _scrollEventArgs.Direction = direction;

                _listInstance.OnScroll(this, _scrollEventArgs);
				_lastMoveY = scrollY;
			} catch(Exception err) {
				Debug.WriteLine(err);
			}

		}

		public override void DecelerationEnded(UIScrollView scrollView)
		{
			if (!_listInstance.IsListenerActive) return;
			_listInstance.OnScrollStateChanged(this, ScrollState.Idle);
		}

		public override void DecelerationStarted(UIScrollView scrollView)
		{
			if (!_listInstance.IsListenerActive) return;
			_listInstance.OnScrollStateChanged(this, ScrollState.Fling);
		}

		public override void DraggingStarted(UIScrollView scrollView)
		{
			if (!_listInstance.IsListenerActive) return;
			_listInstance.OnScrollStateChanged(this, ScrollState.TouchScroll);
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			if (!_listInstance.IsListenerActive) return;
			_listInstance.OnScrollStateChanged(this, ScrollState.Idle);
		}

	}

	#endregion

}