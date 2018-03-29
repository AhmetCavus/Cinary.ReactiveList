using Cinary.Xamarin.Reactive.Event;
using System;
using System.Collections;
using Xamarin.Forms;

namespace Cinary.Xamarin.Reactive
{
    public interface IReactiveList
    {
        #region Events

        event EventHandler<ScrollEventArgs> Scroll;
        event EventHandler<ScrollStateChangeEventArgs> ScrollStateChange;
        event EventHandler<ScrollStateChangedEventArgs> ScrollStateChanged;

        #endregion

        #region Bindable Properties

        IScrollListener ScrollListener { get; set; }

        ColumnExpand ColumnExpand { get; set; }

        double ColumnMinWidth { get; set; }

        IList StackList { get; set; }

        DataTemplate ColumnTemplate { get; set; }

        int? ColumnCount { get; set; }

        bool RefreshContent { get; set; }

        Command ScrollStateChangeCommand { get; set; }

        Command ScrollStateChangedCommand { get; set; }

        Command ScrollCommand { get; set; }

        int Paging { get; set; }

        ScrollOrientation Orientation { get; set; }

        bool IsListenerActive { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Forces ReactiveListView to use AbsoluteLayout internally
        /// When Enabled, auto row height can't be measured automatically,
        /// but it can improve performance
        /// </summary>
        /// <value><c>true</c> if flow use absolute layout internally; otherwise, <c>false</c>.</value>
        bool UseAbsoluteLayoutInternally { get; set; }

        #endregion

        #region Event Handler

        void OnScroll(object sender, ScrollEventArgs e);

        void OnScrollStateChange(object sender, ScrollStateChangeEventArgs e);

        void OnScrollStateChanged(object sender, ScrollState state);

        #endregion

        #region Methods

        /// <summary>
        /// Forces ReactiveList reload.
        /// </summary>
        void ForceReload(bool updateOnly = false);

        void ClearEventHandlers();

        #endregion
    }
}
