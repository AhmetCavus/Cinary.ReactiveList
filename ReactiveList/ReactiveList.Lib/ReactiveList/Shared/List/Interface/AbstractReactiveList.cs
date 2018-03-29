using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cinary.Xamarin.Reactive.Event;
using Xamarin.Forms;

namespace Cinary.Xamarin.Reactive
{
    public abstract class AbstractReactiveList : ListView, IReactiveList
    {
        #region Events

        public event EventHandler<ScrollEventArgs> Scroll;
        public event EventHandler<ScrollStateChangeEventArgs> ScrollStateChange;
        public event EventHandler<ScrollStateChangedEventArgs> ScrollStateChanged;

        #endregion

        #region Attributes

        protected int _desiredColumnCount;
        public int DesiredColumnCount
        {
            get
            {
                if (_desiredColumnCount == 0)
                    return 1;

                return _desiredColumnCount;
            }
            set
            {
                _desiredColumnCount = value;
            }
        }

        protected double? _lastWidth;

        protected int _page = 1;

        protected IList _cacheList = new List<object>();
        protected IList CacheList
        {
            get
            {
                return _cacheList;
            }

            set
            {
                _cacheList = value;
                if (ColumnCount <= 1)
                {
                    ItemsSource = _cacheList;
                    OnPropertyChanged("ItemsSource");
                }
                else
                {
                    OnPropertyChanged("StackList");
                }
            }
        }

        protected IList _stackList;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty ScrollListenerProperty =
          BindableProperty.Create("ScrollListener", typeof(IScrollListener), typeof(ReactiveList));

        IScrollListener _scrollListener;
        // Gets or sets value of this BindableProperty
        public IScrollListener ScrollListener
        {
            get => (IScrollListener) GetValue(ScrollListenerProperty);
            set => SetValue(ScrollListenerProperty, value);
        }

        /// <summary>
        /// The flow group grouping key selector property.
        /// </summary>
        public static BindableProperty ColumnExpandProperty = BindableProperty.Create("ColumnExpand", typeof(ColumnExpand), typeof(ReactiveList), ColumnExpand.None);

        /// <summary>
        /// Gets or sets ReactiveListView column expand mode.
        /// It defines how columns should expand when 
        /// row current column count is less than defined columns templates count
        /// </summary>
        /// <value>ReactiveListView column expand mode.</value>
        public ColumnExpand ColumnExpand
        {
            get { return (ColumnExpand)GetValue(ColumnExpandProperty); }
            set { SetValue(ColumnExpandProperty, value); }
        }

        /// <summary>
        /// The flow column default minimum width property.
        /// </summary>
        public static BindableProperty ColumnMinWidthProperty = BindableProperty.Create("ColumnMinWidth", typeof(double), typeof(ReactiveList), 50d);

        /// <summary>
        /// Gets or sets the minimum column width of ReactiveListView.
        /// Currently used only with <c>FlowAutoColumnCount</c> option
        /// </summary>
        /// <value>The minimum column width.</value>
        public double ColumnMinWidth
        {
            get { return (double)GetValue(ColumnMinWidthProperty); }
            set { SetValue(ColumnMinWidthProperty, value); }
        }

        public static readonly BindableProperty StackListProperty =
            BindableProperty.Create("StackList", typeof(IList), typeof(ReactiveList),
                                    defaultBindingMode: BindingMode.TwoWay,
                                    propertyChanging: (bindable, oldValue, newValue) => {
                                        ((ReactiveList)bindable).StackList = (IList)newValue;
                                    });

        // Gets or sets value of this BindableProperty
        public IList StackList
        {
            get { return (IList)GetValue(StackListProperty); }
            set
            {

                if (_stackList == value) return;

                if (value != null)
                {
                    _stackList = new List<object>(value.Count);
                    foreach (var item in value)
                    {
                        _stackList.Add(item);
                    }
                }
                else
                {
                    _stackList = new List<object>();
                }
                SetValue(StackListProperty, value);
                _page = 1;
                int pageMax = _page * Paging;


                if (_stackList.Count < pageMax)
                {
                    CacheList = _stackList;
                }
                else
                {
                    int itemIndex = 0;
                    ExpandItems(itemIndex);
                }

            }
        }

        /// <summary>
        /// The flow row background color property.
        /// </summary>
        public static BindableProperty RowBackgroundColorProperty = BindableProperty.Create("RowBackgroundColor", typeof(Color), typeof(ReactiveList), Color.Transparent);

        /// <summary>
        /// Gets or sets the color of the flow default row background.
        /// Default: Transparent
        /// </summary>
        /// <value>The color of the flow default row background.</value>
        public Color RowBackgroundColor
        {
            get { return (Color)GetValue(RowBackgroundColorProperty); }
            set { SetValue(RowBackgroundColorProperty, value); }
        }

        /// <summary>
        /// ColumnsTemplatesProperty.
        /// </summary>
        public static readonly BindableProperty ColumnTemplateProperty = BindableProperty.Create("ColumnTemplate", typeof(DataTemplate), typeof(ReactiveList), default(DataTemplate));

        /// <summary>
        /// Gets or sets ReactiveListView columns templates.
        /// Use instance of <c>FlowColumnSimpleTemplateSelector</c> for simple single view scenarios
        /// or implement your own FlowColumnTemplateSelector which can return cell type 
        /// basing on current cell BindingContext
        /// </summary>
        /// <value>ReactiveListView columns templates.</value>
        public DataTemplate ColumnTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ColumnTemplateProperty);
            }
            set
            {
                SetValue(ColumnTemplateProperty, value);
            }
        }

        /// <summary>
        /// The column count property.
        /// </summary>
        public static BindableProperty ColumnCountProperty = BindableProperty.Create("ColumnCount", typeof(int?), typeof(ReactiveList), defaultValue: 1,
             propertyChanging: (bindable, oldValue, newValue) => {
                 ((ReactiveList)bindable).ColumnCount = (int?)newValue;
             });

        /// <summary>
        /// Enables or disables ReactiveList auto/manual column count.
        /// Auto Column count is calculated basing on View width 
        /// and <c>ColumnMinWidth</c> property
        /// </summary>
        /// <value>The column count.</value>
        public int? ColumnCount
        {
            get { return (int?)GetValue(ColumnCountProperty); }
            set
            {
                SetValue(ColumnCountProperty, value);
                bool isMultiColumn = value > 1;
                if (isMultiColumn)
                {
                    RegisterEvents();
                    CreateMultiColumnItemTemplate();
                }
                else
                {
                    UnregisterEvents();
                }

            }
        }

        public static readonly BindableProperty RefreshContentProperty =
            BindableProperty.Create("RefreshContent", typeof(bool), typeof(ReactiveList), defaultValue: false,
                                    propertyChanged: (bindable, oldValue, newValue) => {
                                        ((ReactiveList)bindable).RefreshContent = (bool)newValue;
                                    });

        // Gets or sets value of this BindableProperty
        public bool RefreshContent
        {
            get { return (bool)GetValue(RefreshContentProperty); }
            set
            {
                SetValue(RefreshContentProperty, value);
                if (value) ForceReload(true);
            }
        }

        public static readonly BindableProperty ToHideImageOnScrollReferenceNameProperty =
          BindableProperty.Create("ToHideImageOnScrollReferenceName", typeof(string), typeof(ReactiveList),
                                  propertyChanged: (bindable, oldValue, newValue) => {
                                      ((ReactiveList)bindable).ToHideImageOnScrollReferenceName = (string)newValue;
                                  });

        // Gets or sets value of this BindableProperty
        public string ToHideImageOnScrollReferenceName
        {
            get { return (string)GetValue(ToHideImageOnScrollReferenceNameProperty); }
            set { SetValue(ToHideImageOnScrollReferenceNameProperty, value); }
        }

        public static readonly BindableProperty ScrollStateChangeCommandProperty =
              BindableProperty.Create("ScrollStateChangeCommand", typeof(Command), typeof(ReactiveList));

        // Gets or sets value of this BindableProperty
        public Command ScrollStateChangeCommand
        {
            get { return (Command)GetValue(ScrollStateChangeCommandProperty); }
            set { SetValue(ScrollStateChangeCommandProperty, value); }
        }

        public static readonly BindableProperty ScrollStateChangedCommandProperty =
              BindableProperty.Create("ScrollStateChangedCommand", typeof(Command), typeof(ReactiveList));

        // Gets or sets value of this BindableProperty
        public Command ScrollStateChangedCommand
        {
            get { return (Command)GetValue(ScrollStateChangedCommandProperty); }
            set { SetValue(ScrollStateChangedCommandProperty, value); }
        }

        public static readonly BindableProperty ScrollCommandProperty =
          BindableProperty.Create("ScrollCommand", typeof(Command), typeof(ReactiveList));

        // Gets or sets value of this BindableProperty
        public Command ScrollCommand
        {
            get { return (Command)GetValue(ScrollCommandProperty); }
            set { SetValue(ScrollCommandProperty, value); }
        }

        public static readonly BindableProperty PagingProperty =
            BindableProperty.Create("Paging", typeof(int), typeof(ReactiveList), defaultValue: 10,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((ReactiveList)bindable).Paging = (int)newValue;
                });

        // Gets or sets value of this BindableProperty
        public int Paging
        {
            get { return (int)GetValue(PagingProperty); }
            set { SetValue(PagingProperty, value); }
        }

        public static readonly BindableProperty OrientationProperty =
          BindableProperty.Create("Orientation", typeof(ScrollOrientation), typeof(ReactiveList),
                                  defaultValue: ScrollOrientation.Vertical,
                                  propertyChanged: (bindable, oldValue, newValue) =>
                                  {
                                      ((ReactiveList)bindable).Orientation = (ScrollOrientation)newValue;
                                  });

        // Gets or sets value of this BindableProperty
        public ScrollOrientation Orientation
        {
            get { return (ScrollOrientation)GetValue(OrientationProperty); }
            set
            {
                SetValue(OrientationProperty, value);
                if (value == ScrollOrientation.Horizontal)
                {
                    this.Rotation = 270;
                }
                else
                {
                    this.RotationY = 0;
                }
            }
        }

        public static readonly BindableProperty IsListenerActiveProperty =
          BindableProperty.Create(
              "IsListenerActive",
              typeof(bool),
              typeof(ReactiveList),
              defaultValue: false
        );

        // Gets or sets value of this BindableProperty
        public bool IsListenerActive
        {
            get { return (bool)GetValue(IsListenerActiveProperty); }
            set { SetValue(IsListenerActiveProperty, value); }
        }

        public abstract bool UseAbsoluteLayoutInternally { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveList"/> class.
        /// </summary>
        public AbstractReactiveList() : base(ListViewCachingStrategy.RecycleElement)
        {
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveList"/> class.
        /// </summary>
        /// <param name="cachingStrategy">Caching strategy.</param>
        public AbstractReactiveList(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
            Init();
        }

        #endregion

        #region Protected Methods

        void Init()
        {
            ColumnExpand = ColumnExpand.None;
            SeparatorVisibility = SeparatorVisibility.None;
        }

        void ExpandItems(int itemIndex)
        {
            int pageMax = _page * Paging;
            pageMax = pageMax > _stackList.Count ? _stackList.Count : pageMax;
            int loopCount = 0;
            if (itemIndex >= _stackList.Count)
            {
                loopCount = _stackList.Count;
            }
            else
            {
                loopCount = pageMax;
            }
            _cacheList.Clear();
            try
            {
                for (int index = 0; index < loopCount; index++)
                {
                    var item = _stackList[index];
                    _cacheList.Add(item);
                }
                CacheList = _cacheList;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        protected abstract void RegisterEvents();

        protected abstract void UnregisterEvents();

        protected abstract void CreateMultiColumnItemTemplate();

        #endregion

        #region Public Methods

        /// <summary>
        /// Forces ReactiveList reload.
        /// </summary>
        public abstract void ForceReload(bool updateOnly = false);
        public abstract void OnScroll(object sender, ScrollEventArgs e);
        public abstract void OnScrollStateChange(object sender, ScrollStateChangeEventArgs e);
        public abstract void OnScrollStateChanged(object sender, ScrollState state);
        public abstract void ClearEventHandlers();

        #endregion
    }
}
