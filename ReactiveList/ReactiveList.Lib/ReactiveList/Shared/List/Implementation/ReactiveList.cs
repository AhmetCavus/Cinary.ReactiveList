//
//  ReactiveList.cs
//
//  Author:
//       ahc <ahmet.cavus@cinary.com>
//
//  Copyright (c) 2018 (c) Ahmet Cavus
using Cinary.Xamarin.Reactive.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Cinary.Xamarin.Reactive
{
	public class ReactiveList : AbstractReactiveList, IDisposable
	{
		#region Events

		ScrollEventArgs scrollEventArgs = new ScrollEventArgs();
		ScrollStateChangeEventArgs scrollStateChangeEventArgs = new ScrollStateChangeEventArgs();
		ScrollStateChangedEventArgs scrollStateChangedEventArgs = new ScrollStateChangedEventArgs();

		#endregion

		#region Bindable Properties

		#endregion

		#region .Net Properties

		/// <summary>
		/// Forces ReactiveList to use AbsoluteLayout internally
		/// When Enabled, auto row height can't be measured automatically,
		/// but it can improve performance
		/// </summary>
		/// <value><c>true</c> if flow use absolute layout internally; otherwise, <c>false</c>.</value>
		public override bool UseAbsoluteLayoutInternally { get; set; } = false;

		#endregion

		#region Attributes

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactiveList"/> class.
		/// </summary>
		public ReactiveList() : base(ListViewCachingStrategy.RecycleElement)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactiveList"/> class.
		/// </summary>
		/// <param name="cachingStrategy">Caching strategy.</param>
		public ReactiveList(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
		}

		#endregion

		#region Event Handler

		public override void OnScroll(object sender, ScrollEventArgs e)
		{
			RequestExpandItems(e.FirstVisibleItem + e.VisibleItemCount);
			e.IsFirstRow = e.FirstVisibleItem <= 0;
			e.IsLastRow = e.TotalItemCount * _desiredColumnCount >= StackList?.Count;
			ScrollListener?.OnScroll(sender, e);
            ScrollCommand?.Execute(scrollEventArgs);
        }

		public override void OnScrollStateChange(object sender, ScrollStateChangeEventArgs e)
		{
            ScrollListener?.OnScrollStateChange(sender, e);
            ScrollStateChangeCommand?.Execute(e);
        }

		public override void OnScrollStateChanged(object sender, ScrollState state)
		{
			scrollStateChangedEventArgs.ScrollState = state;
            ScrollStateChangedCommand?.Execute(scrollStateChangedEventArgs);
            ScrollListener?.OnScrollStateChanged(this, scrollStateChangedEventArgs);
		}

		protected void ListViewPropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			if (e.PropertyName == ReactiveList.StackListProperty.PropertyName)
			{
                if (StackList is INotifyCollectionChanged itemSource)
                    itemSource.CollectionChanged -= ItemsSourceCollectionChanged;

                if (IsGroupingEnabled)
				{
                    if (StackList is IEnumerable<INotifyCollectionChanged> groupedSource)
                    {
                        foreach (var gr in groupedSource)
                        {
                            gr.CollectionChanged -= ItemsSourceCollectionChanged;
                        }
                    }
                }
			}
		}

		protected void ListViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == StackListProperty.PropertyName)
			{
                if (StackList is INotifyCollectionChanged itemSource)
                    itemSource.CollectionChanged += ItemsSourceCollectionChanged;

                if (IsGroupingEnabled)
				{
                    if (StackList is IEnumerable<INotifyCollectionChanged> groupedSource)
                    {
                        foreach (var gr in groupedSource)
                        {
                            gr.CollectionChanged += ItemsSourceCollectionChanged;
                        }
                    }
                }

				if (ColumnTemplate == null || StackList == null)
				{
					ItemsSource = null;
					return;
				}

				ForceReload();
			}
		}

		protected void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ForceReload(updateOnly: true);
		}

		#endregion

		#region Private Methods

		protected override void RegisterEvents() {
			PropertyChanged += ListViewPropertyChanged;
			PropertyChanging += ListViewPropertyChanging;
		}

        protected override void UnregisterEvents() {
			PropertyChanged -= ListViewPropertyChanged;
			PropertyChanging -= ListViewPropertyChanging;
		}

        protected override void CreateMultiColumnItemTemplate() {
			var listViewRef = new WeakReference<ReactiveList>(this);
			ItemTemplate = new DataTemplate(() => new ReactiveListCell(listViewRef));
		}

		void RequestExpandItems(int visibleItems) 
		{
			if (ColumnCount == null) ColumnCount = 1;
			int pageMax = ((_page * Paging) / ColumnCount.Value);
			int itemIndex = visibleItems;
			if (itemIndex < pageMax) return;
			_page += 1;
			ExpandItems(itemIndex);
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
			try {
				for (int index = 0; index < loopCount; index++)
				{
					var item = _stackList[index];
					_cacheList.Add(item);
				}
				CacheList = _cacheList;
			} catch(Exception err) {
				Debug.WriteLine(err);
			}
		}

		protected void UpdateContainerList()
		{
			var currentSource = _cacheList as ObservableCollection<ObservableCollection<object>>;

			if (currentSource != null && currentSource.Count > 0)
			{
				var tempList = GetContainerList();

				bool structureIsChanged = false;
				for (int i = 0; i < tempList.Count; i++)
				{
					if (currentSource.Count <= i)
					{
						currentSource.Add(tempList[i]);
					}
					else
					{
						if (structureIsChanged || tempList[i].Any(v => !(currentSource[i].Contains(v))))
						{
							structureIsChanged = true;
							currentSource[i] = tempList[i];
						}
					}
				}

				while (currentSource.Count > tempList.Count)
				{
					currentSource.RemoveAt(currentSource.Count - 1);
				}
			}
			else
			{
				ReloadContainerList();
			}
		}

		protected void ReloadContainerList()
		{
			ItemsSource = GetContainerList();
		}

		protected void UpdateGroupedContainerList()
		{

            if (ItemsSource is ObservableCollection<ListGroup> currentSource && currentSource.Count > 0)
            {
                var tempList = GetGroupedContainerList();

                // GROUPS HEADERS
                bool structureIsChanged = false;
                for (int i = 0; i < tempList.Count; i++)
                {
                    if (currentSource.Count <= i)
                    {
                        currentSource.Add(tempList[i]);
                    }
                    else
                    {
                        if (structureIsChanged || tempList[i].Any(v => !(currentSource[i].Contains(v))))
                        {
                            structureIsChanged = true;
                            currentSource[i] = tempList[i];
                        }
                    }
                }

                while (currentSource.Count > tempList.Count)
                {
                    currentSource.RemoveAt(currentSource.Count - 1);
                }

                for (int grId = 0; grId < tempList.Count; grId++)
                {
                    bool groupStructureIsChanged = false;
                    for (int i = 0; i < tempList[grId].Count; i++)
                    {
                        if (currentSource[grId].Count <= i)
                        {
                            currentSource[grId].Add(tempList[grId][i]);
                        }
                        else
                        {
                            if (groupStructureIsChanged || tempList[grId][i].Any(v => !(currentSource[grId][i].Contains(v))))
                            {
                                groupStructureIsChanged = true;
                                currentSource[grId][i] = tempList[grId][i];
                            }
                        }
                    }

                    while (currentSource[grId].Count > tempList[grId].Count)
                    {
                        currentSource[grId].RemoveAt(currentSource[grId].Count - 1);
                    }
                }
            }
            else
            {
                ReloadGroupedContainerList();
            }
        }

		protected ObservableCollection<ObservableCollection<object>> GetContainerList()
		{
			var colCount = DesiredColumnCount;

			int capacity = (_cacheList.Count / colCount) +
				(_cacheList.Count % colCount) > 0 ? 1 : 0;

			List<ObservableCollection<object>> tmpList = new List<ObservableCollection<object>>(capacity);
			//_tmpList.Clear();
			int position = -1;

			for (int i = 0; i < _cacheList.Count; i++)
			{
				if (i % colCount == 0)
				{
					position++;

					tmpList.Add(new ObservableCollection<object> {
							_cacheList[i]
						});
				}
				else
				{
					var exContItm = tmpList[position];
					exContItm.Add(_cacheList[i]);
				}
			}
			return new ObservableCollection<ObservableCollection<object>>(tmpList);
		}

		protected ObservableCollection<ListGroup> GetGroupedContainerList()
		{
			var colCount = DesiredColumnCount;
			var flowGroupsList = new List<ListGroup>(StackList.Count);
			var groupDisplayPropertyName = (GroupDisplayBinding as Binding)?.Path;

			foreach (var groupContainer in ItemsSource)
			{
				var isAlreadyFlowGroup = groupContainer as ListGroup;

				if (isAlreadyFlowGroup != null)
				{
					flowGroupsList.Add(isAlreadyFlowGroup);
				}
				else
				{
					var gr = groupContainer as IList;
					if (gr != null)
					{
						var type = gr?.GetType();

						object groupKeyValue = null;

						if (type != null && groupDisplayPropertyName != null)
						{
							PropertyInfo groupDisplayProperty = type?.GetRuntimeProperty(groupDisplayPropertyName);
							groupKeyValue = groupDisplayProperty?.GetValue(gr);
						}

						var flowGroup = new ListGroup(groupKeyValue);

						int position = -1;

						for (int i = 0; i < gr.Count; i++)
						{
							if (i % colCount == 0)
							{
								position++;

								flowGroup.Add(new ObservableCollection<object>() { gr[i] });
							}
							else
							{
								var exContItm = flowGroup[position];
								exContItm.Add(gr[i]);
							}
						}

						flowGroupsList.Add(flowGroup);
					}
				}
			}

			return new ObservableCollection<ListGroup>(flowGroupsList);
		}

		protected void ReloadGroupedContainerList()
		{
			ItemsSource = GetGroupedContainerList();
		}

		protected void RefreshDesiredColumnCount()
		{
			if (!ColumnCount.HasValue)
			{
				double listWidth = Math.Max(Math.Max(Width, WidthRequest), MinimumWidthRequest);

				if (listWidth > 0)
				{
					DesiredColumnCount = (int)Math.Floor(listWidth / ColumnMinWidth);
				}
			}
			else
			{
				DesiredColumnCount = ColumnCount.Value;
			}
		}

		protected void ListSizeChanged(object sender, EventArgs e)
		{
			if (!ColumnCount.HasValue)
			{
				double listWidth = Math.Max(Math.Max(Width, WidthRequest), MinimumWidthRequest);

				if (listWidth > 0)
				{
					if ((_lastWidth.HasValue && Math.Abs(_lastWidth.Value - listWidth) > Double.MinValue)
						|| !_lastWidth.HasValue)
					{
						if (ItemsSource != null)
							ForceReload();
					}

					_lastWidth = listWidth;
				}
			}
		}

		protected void HandlePaging() {
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Forces ReactiveList reload.
        /// </summary>
        public override void ForceReload(bool updateOnly = false)
		{
			if (updateOnly)
			{
				if (IsGroupingEnabled)
					UpdateGroupedContainerList();
				else
					UpdateContainerList();
			}
			else
			{
				RefreshDesiredColumnCount();

				if (IsGroupingEnabled)
					ReloadGroupedContainerList();
				else
					ReloadContainerList();
			}
		}

        public override void ClearEventHandlers()
        {
        }

        public void Dispose()
		{
            PropertyChanged -= ListViewPropertyChanged;
			PropertyChanging -= ListViewPropertyChanging;
			SizeChanged -= ListSizeChanged;

            if (StackList is INotifyCollectionChanged itemSource)
            {
                itemSource.CollectionChanged -= ItemsSourceCollectionChanged;
            }
        }

		#endregion

	}

}