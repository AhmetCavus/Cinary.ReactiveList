//  ReactiveListCell.cs
//
//  Author:
//       ahc <ahmet.cavus@cinary.com>
//
//  Copyright (c) 2018 (c) Ahmet Cavus
using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Cinary.Xamarin.Reactive
{
    public class ReactiveListCell : ViewCell
	{
		#region Attributes

		WeakReference<ReactiveList> _reactiveListRef;
        ReactiveList _reactiveList;
		AbsoluteLayout _rootLayout;
		Grid _rootLayoutAuto;
		bool _useGridAsMainRoot;
		int _desiredColumnCount;
		DataTemplate _columnTemplate;
		ColumnExpand _columnExpand;
		IList<DataTemplate> _currentColumnTemplates = null;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactiveList"/> class.
		/// </summary>
		/// <param name="reactiveListRef">reactiveList reference.</param>
		public ReactiveListCell(WeakReference<ReactiveList> reactiveListRef)
		{
			Init(reactiveListRef);
		}

		#endregion

		#region Private Methods

		protected virtual void Init(WeakReference<ReactiveList> reactiveListRef)
		{
			_reactiveListRef = reactiveListRef;
			_reactiveListRef.TryGetTarget(out _reactiveList);
			_useGridAsMainRoot = !_reactiveList.UseAbsoluteLayoutInternally;

			if (!_useGridAsMainRoot)
			{
				_rootLayout = new AbsoluteLayout()
				{
					Padding = 0d,
					BackgroundColor = _reactiveList.RowBackgroundColor,
				};
				View = _rootLayout;
			}
			else
			{
				_rootLayoutAuto = new Grid()
				{
					RowSpacing = 0d,
					ColumnSpacing = 0d,
					Padding = 0d,
					BackgroundColor = _reactiveList.RowBackgroundColor,
				};
				View = _rootLayoutAuto;
			}

			_columnTemplate = _reactiveList.ColumnTemplate;
			_desiredColumnCount = _reactiveList.DesiredColumnCount;
			_columnExpand = _reactiveList.ColumnExpand;
		}

		protected virtual DataTemplate RequestColumnTemplate()
		{
            ReactiveList reactiveList = null;
			DataTemplate res;
			if (_reactiveListRef.TryGetTarget(out reactiveList) && reactiveList != null)
			{
				res = reactiveList.ColumnTemplate;
				_desiredColumnCount = reactiveList.DesiredColumnCount;
				_columnExpand = reactiveList.ColumnExpand;
			}
			else
			{
				res = default(DataTemplate);
			}
			return res;
		}

		protected virtual void ReuseViews(IList container)
		{
			if (_useGridAsMainRoot)
			{
				for (int i = 0; i < container.Count; i++)
				{
					SetBindingContextForView(_rootLayoutAuto.Children[i], container[i]);
				}
			}
			else
			{
				for (int i = 0; i < container.Count; i++)
				{
					SetBindingContextForView(_rootLayout.Children[i], container[i]);
				}
			}
		}

		protected virtual void RecreateColumns(IList container, IList<DataTemplate> templates)
		{
			if (_useGridAsMainRoot)
			{
				if (_rootLayoutAuto.Children.Count > 0)
					_rootLayoutAuto.Children.Clear();
			}
			else
			{
				if (_rootLayout.Children.Count > 0)
					_rootLayout.Children.Clear();
			}

			_currentColumnTemplates = new List<DataTemplate>(templates);

			if (_useGridAsMainRoot)
			{
				if (_rootLayoutAuto.Children.Count > 0)
					_rootLayoutAuto.Children.Clear();

				var colDefs = new ColumnDefinitionCollection();
				for (int i = 0; i < _desiredColumnCount; i++)
				{
					colDefs.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });
				}
				_rootLayoutAuto.ColumnDefinitions = colDefs;

				for (int i = 0; i < container.Count; i++)
				{
					var view = (View) templates[i].CreateContent();

					view.GestureRecognizers.Add(new TapGestureRecognizer()
					{
						Command = new Command(async (obj) =>
						{
							ExecuteTapGestureRecognizer(view);
						})
					});

					SetBindingContextForView(view, container[i]);
					if (container.Count == 0 || _desiredColumnCount == 0)
						return;

					AddViewToLayoutAutoHeightEnabled(view, container.Count, i);
				}
			}
			else
			{
				if (_rootLayout.Children.Count > 0)
					_rootLayout.Children.Clear();

				for (int i = 0; i < container.Count; i++)
				{
					var view = (View) templates[i].CreateContent();

					view.GestureRecognizers.Add(new TapGestureRecognizer()
					{
						Command = new Command(async (obj) =>
						{
							ExecuteTapGestureRecognizer(view);
						})
					});

					SetBindingContextForView(view, container[i]);
					if (container.Count == 0 || _desiredColumnCount == 0)
						return;

					AddViewToLayoutAutoHeightDisabled(view, container.Count, i);
				}
			}
		}

		protected virtual void Render(bool layoutChanged, IList container, IList<DataTemplate> templates)
		{
			var containerCount = container.Count;

			if (!layoutChanged) // REUSE VIEWS
			{
				ReuseViews(container);
			}
			else // RECREATE COLUMNS
			{
				RecreateColumns(container, templates);
			}
		}

		protected virtual IList<DataTemplate> GetDataTemplates(IList container)
		{
			List<DataTemplate> templates = new List<DataTemplate>();

			//var flowTemplateSelector = _columnTemplate as FlowTemplateSelector;
			//if (flowTemplateSelector != null)
			//{
			//	for (int i = 0; i < container.Count; i++)
			//	{
			//		var template = flowTemplateSelector.SelectTemplate(container[i], i, _reactiveList);
			//		templates.Add(template);
			//	}
			//}
			//else
			//{
			var templateSelector = _columnTemplate as DataTemplateSelector;
			if (templateSelector != null)
			{
				for (int i = 0; i < container.Count; i++)
				{
					var template = templateSelector.SelectTemplate(container[i], _reactiveList);
					templates.Add(template);
				}
			}
			else
			{
				for (int i = 0; i < container.Count; i++)
				{
					templates.Add(_columnTemplate);
				}
			}
			return templates;
		}

		protected virtual bool RowLayoutChanged(int containerCount, IList<DataTemplate> templates)
		{
			// Check if desired number of columns is equal to current number of columns
			if (_currentColumnTemplates == null || containerCount != _currentColumnTemplates.Count)
			{
				return true;
			}

			 //Check if desired column view types are equal to current columns view types
			for (int i = 0; i < containerCount; i++)
			{
				if (_currentColumnTemplates[i].GetType() != templates[i].GetType())
				{
					return true;
				}
			}

			return false;
		}

		protected virtual void SetBindingContextForView(View view, object bindingContext)
		{
			if (view != null && view.BindingContext != bindingContext)
				view.BindingContext = bindingContext;
		}

		void AddViewToLayoutAutoHeightEnabled(View view, int containerCount, int colNumber)
		{
			if (_desiredColumnCount > containerCount)
			{
				int diff = _desiredColumnCount - containerCount;
				bool isLastColumn = colNumber == containerCount - 1;

				switch (_columnExpand)
				{
					case ColumnExpand.None:

						_rootLayoutAuto.Children.Add(view, colNumber, 0);

						break;

					case ColumnExpand.First:

						if (colNumber == 0)
						{
							_rootLayoutAuto.Children.Add(view, colNumber, colNumber + diff + 1, 0, 1);
						}
						else
						{
							_rootLayoutAuto.Children.Add(view, colNumber + diff, colNumber + diff + 1, 0, 1);
						}

						break;

					case ColumnExpand.Last:

						if (isLastColumn)
						{
							_rootLayoutAuto.Children.Add(view, colNumber, colNumber + diff + 1, 0, 1);
						}
						else
						{
							_rootLayoutAuto.Children.Add(view, colNumber, 0);
						}

						break;

					case ColumnExpand.Proportional:

						int howManyP = _desiredColumnCount / containerCount - 1;
						_rootLayoutAuto.Children.Add(view, colNumber + colNumber * howManyP, colNumber + colNumber * howManyP + howManyP + 1, 0, 1);

						break;

					case ColumnExpand.ProportionalFirst:

						int firstSizeAdd = (int)((double)_desiredColumnCount) % containerCount; //1
						int otherSize = (int)Math.Floor((double)_desiredColumnCount / containerCount); //2

						if (colNumber == 0)
							_rootLayoutAuto.Children.Add(view, 0, otherSize + firstSizeAdd, 0, 1);
						else
							_rootLayoutAuto.Children.Add(view, (colNumber * otherSize) + firstSizeAdd, ((colNumber + 1) * otherSize) + firstSizeAdd, 0, 1);

						break;

					case ColumnExpand.ProportionalLast:

						int lastSizeAdd = (int)((double)_desiredColumnCount) % containerCount; //1
						int otherSize1 = (int)Math.Floor((double)_desiredColumnCount / containerCount); //2

						if (isLastColumn)
						{
							_rootLayoutAuto.Children.Add(view, (colNumber * otherSize1), ((colNumber + 1) * otherSize1) + lastSizeAdd, 0, 1);
						}
						else
						{
							_rootLayoutAuto.Children.Add(view, (colNumber * otherSize1), ((colNumber + 1) * otherSize1), 0, 1);
						}

						break;
				}
			}
			else
			{
				_rootLayoutAuto.Children.Add(view, colNumber, 0);
			}
		}

		void AddViewToLayoutAutoHeightDisabled(View view, int containerCount, int colNumber)
		{
			double desiredColumnWidth = 1d / _desiredColumnCount;
			Rectangle bounds = Rectangle.Zero;

			if (_columnExpand != ColumnExpand.None && _desiredColumnCount > containerCount)
			{
				int diff = _desiredColumnCount - containerCount;
				bool isLastColumn = colNumber == containerCount - 1;

				switch (_columnExpand)
				{
					case ColumnExpand.First:

						if (colNumber == 0)
						{
							bounds = new Rectangle(0d, 0d, desiredColumnWidth + (desiredColumnWidth * diff), 1d);
						}
						else if (isLastColumn)
						{
							bounds = new Rectangle(1d, 0d, desiredColumnWidth, 1d);
						}
						else
						{
							bounds = new Rectangle(desiredColumnWidth * (colNumber + diff) / (1d - desiredColumnWidth), 0d, desiredColumnWidth, 1d);
						}

						break;

					case ColumnExpand.Last:

						if (colNumber == 0)
						{
							bounds = new Rectangle(0d, 0d, desiredColumnWidth + (desiredColumnWidth * diff), 1d);
						}
						else if (isLastColumn)
						{
							bounds = new Rectangle(1d, 0d, desiredColumnWidth + (desiredColumnWidth * diff), 1d);
						}
						else
						{
							bounds = new Rectangle(desiredColumnWidth * colNumber / (1d - desiredColumnWidth), 0d, desiredColumnWidth, 1d);
						}

						break;

					case ColumnExpand.Proportional:

						double propColumnsWidth = 1d / containerCount;
						if (colNumber == 0)
						{
							bounds = new Rectangle(0d, 0d, propColumnsWidth, 1d);
						}
						else if (isLastColumn)
						{
							bounds = new Rectangle(1d, 0d, propColumnsWidth, 1d);
						}
						else
						{
							bounds = new Rectangle(propColumnsWidth * colNumber / (1d - propColumnsWidth), 0d, propColumnsWidth, 1d);
						}

						break;

					case ColumnExpand.ProportionalFirst:

						int propFMod = _desiredColumnCount % containerCount;
						double propFSize = desiredColumnWidth * Math.Floor((double)_desiredColumnCount / containerCount);
						double propFSizeFirst = propFSize + desiredColumnWidth * propFMod;

						if (colNumber == 0)
						{
							bounds = new Rectangle(0d, 0d, propFSizeFirst, 1d);
						}
						else if (isLastColumn)
						{
							bounds = new Rectangle(1d, 0d, propFSize, 1d);
						}
						else
						{
							bounds = new Rectangle(((propFSize * colNumber) + (propFSizeFirst - propFSize)) / (1d - propFSize), 0d, propFSize, 1d);
						}

						break;

					case ColumnExpand.ProportionalLast:

						int propLMod = _desiredColumnCount % containerCount;
						double propLSize = desiredColumnWidth * Math.Floor((double)_desiredColumnCount / containerCount);
						double propLSizeLast = propLSize + desiredColumnWidth * propLMod;

						if (colNumber == 0)
						{
							bounds = new Rectangle(0d, 0d, propLSize, 1d);
						}
						else if (isLastColumn)
						{
							bounds = new Rectangle(1d, 0d, propLSizeLast, 1d);
						}
						else
						{
							bounds = new Rectangle((propLSize * colNumber) / (1d - propLSize), 0d, propLSize, 1d);
						}

						break;
				}
			}
			else
			{
				if (Math.Abs(1d - desiredColumnWidth) < Double.MaxValue)
				{
					bounds = new Rectangle(1d, 0d, desiredColumnWidth, 1d);
				}
				else
				{
					bounds = new Rectangle(desiredColumnWidth * colNumber / (1d - desiredColumnWidth), 0d, desiredColumnWidth, 1d);
				}
			}

			_rootLayout.Children.Add(view, bounds, AbsoluteLayoutFlags.All);
		}

		void ExecuteTapGestureRecognizer(View view)
		{
			//var flowCell = view as IFlowViewCell;
			//if (flowCell != null)
			//{
			//	flowCell.OnTapped();
			//}

			//FlowListView flowListView = null;
			//_flowListViewRef.TryGetTarget(out flowListView);

			//if (flowListView != null)
			//{
			//	int tapBackgroundEffectDelay = flowListView.FlowTappedBackgroundDelay;

			//	try
			//	{
			//		if (tapBackgroundEffectDelay != 0)
			//		{
			//			view.BackgroundColor = flowListView.FlowTappedBackgroundColor;
			//		}

			//		flowListView.FlowPerformTap(view.BindingContext);
			//	}
			//	finally
			//	{
			//		if (tapBackgroundEffectDelay != 0)
			//		{
			//			await Task.Delay(tapBackgroundEffectDelay);
			//			view.BackgroundColor = flowListView.FlowRowBackgroundColor;
			//		}
			//	}
			//}
		}

		#endregion

		#region Event Handler

		/// <summary>
		/// Override this method to execute an action when the BindingContext changes.
		/// </summary>
		/// <remarks></remarks>
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var container = BindingContext as IList;
			if (container == null) return;

			RequestColumnTemplate();

			// Getting view types from templates
			var containerCount = container.Count;
			IList<DataTemplate> templates = GetDataTemplates(container);
			bool layoutChanged = RowLayoutChanged(containerCount, templates);
			Render(layoutChanged, container, templates);
		}

		#endregion

	}

}