//
//  ReactiveListRenderer.cs
//
//  Author:
//       ahc <ahmet.cavus@cinary.com>
//
//  Copyright (c) 2017 (c) Ahmet Cavus
using System;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Cinary.Xamarin.Droid.Reactive;
using Cinary.Xamarin.Reactive;
using Cinary.Xamarin.Reactive.Event;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ReactiveList), typeof(ReactiveListRenderer))]
namespace Cinary.Xamarin.Droid.Reactive
{
	public class ReactiveListRenderer : ListViewRenderer
	{
        #region Attributes

        ReactiveList _listInstance;
        OnScrollListener _scrollListener;

        #endregion

        #region Constructor

        public ReactiveListRenderer(Context context) : base(context)
        {
        }

        #pragma warning disable CS0618 // Typ or Element is obsolete
        public ReactiveListRenderer() : base(Forms.Context)
        #pragma warning restore CS0618 // Typ or Element is obsolete
        {
        }

        #endregion

        #region Event Handler

        protected override void OnElementChanged(ElementChangedEventArgs<global::Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null) UnregisterEventHandler(e);
			if (e.NewElement != null) RegisterEventHandler(e);
		}


		protected void OnScroll(object sender, AbsListView.ScrollEventArgs e)
		{
		}

		ScrollStateChangeEventArgs _scrollChangeEventArgs = new ScrollStateChangeEventArgs();
		protected void OnScrollChange(object sender, ScrollChangeEventArgs e)
		{
			_scrollChangeEventArgs.ScrollX = e.ScrollX;
			_scrollChangeEventArgs.ScrollY = e.ScrollY;
			_scrollChangeEventArgs.OldScrollX = e.OldScrollX;
			_scrollChangeEventArgs.OldScrollY = e.OldScrollY;
			_listInstance.OnScrollStateChange(this, _scrollChangeEventArgs);
		}

		protected void OnScrollStateChanged(object sender, AbsListView.ScrollStateChangedEventArgs e)
		{
		}

		#endregion

		#region Private Methods

        void RegisterEventHandler(ElementChangedEventArgs<global::Xamarin.Forms.ListView> e)
		{
			_listInstance = (ReactiveList) e.NewElement;
			_scrollListener = new OnScrollListener(_listInstance, Control);
			Control.SetOnScrollListener(_scrollListener);
			Control.SmoothScrollbarEnabled = false;
			Control.VerticalScrollBarEnabled = false;
			Control.HorizontalScrollBarEnabled = false;
		}

		void UnregisterEventHandler(ElementChangedEventArgs<global::Xamarin.Forms.ListView> e)
		{
            _listInstance = (ReactiveList) e.NewElement;
            _scrollListener = null;
            Control.SetOnScrollListener(null);
        }

		#endregion

	}

	#region Class

	public class OnScrollListener : Java.Lang.Object, AbsListView.IOnScrollListener
	{
        ReactiveList _listInstance;
		AbsListView _control;

		public OnScrollListener(ReactiveList list, AbsListView control)
		{
			_listInstance = list;
			_control = control;
		}

		int _scrollY;
		int _prevIndex;
		int _prevViewPos;
		int _prevViewHeight;
		ScrollDirection _direction;
		ScrollEventArgs _scrollEventArgs = new ScrollEventArgs();

		public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{
			if (!_listInstance.IsListenerActive) return;
			try {
				var currView = _control.GetChildAt(0);
				int currViewPos = currView.Top;
				int diffViewPos = _prevViewPos - currViewPos;
				int currViewHeight = currView.Height;
				_scrollY += diffViewPos;
				if (firstVisibleItem > _prevIndex) {
					_scrollY += _prevViewHeight;
					diffViewPos = 0;
				} else if (firstVisibleItem < _prevIndex) {
					_scrollY -= currViewHeight;
					diffViewPos = 0;
				}
				_prevIndex = firstVisibleItem;
				_prevViewPos = currViewPos;
				_prevViewHeight = currViewHeight;
				if (diffViewPos < 0) _direction = ScrollDirection.Up;
				else if (diffViewPos > 0) _direction = ScrollDirection.Down;
				else _direction = ScrollDirection.None;
				ScrollDirection direction = _direction;
                _scrollEventArgs.FirstVisibleItem = firstVisibleItem;
                _scrollEventArgs.VisibleItemCount = visibleItemCount;
                _scrollEventArgs.TotalItemCount = totalItemCount;
                _scrollEventArgs.ScrollY = _scrollY;
                _scrollEventArgs.Direction = direction;

                _listInstance.OnScroll(this, _scrollEventArgs);
			} catch (Exception err) {
				System.Diagnostics.Debug.WriteLine(err);
			}
		}

		Android.Widget.ScrollState _scrollState;
		public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] Android.Widget.ScrollState scrollState)
		{
			if (!_listInstance.IsListenerActive) return;
			_scrollState = scrollState;
			if (_scrollState == Android.Widget.ScrollState.Fling) {
				_listInstance.OnScrollStateChanged(this, Xamarin.Reactive.Event.ScrollState.Fling);
			} else if (_scrollState == Android.Widget.ScrollState.TouchScroll) {
				_listInstance.OnScrollStateChanged(this, Xamarin.Reactive.Event.ScrollState.TouchScroll);
			} else if (_scrollState == Android.Widget.ScrollState.Idle) {
				_listInstance.OnScrollStateChanged(this, Xamarin.Reactive.Event.ScrollState.Idle);
			}
		}

	}

	#endregion

}
