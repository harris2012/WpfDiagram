using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestApp
{
	/// <summary>
	/// Helper class used to start drag&drop operation and display a dragging object
	/// </summary>
	public class ItemsControlDragHelper : IDisposable
	{
		#region DragDropAdorner

		/// <summary>
		/// Renders a visual which can follow the mouse cursor, 
		/// such as during a drag-and-drop operation.
		/// </summary>
		// Copyright (C) Josh Smith - January 2007
		private class DragDropAdorner : Adorner
		{
			#region Data

			private Rectangle child = null;
			private double offsetLeft = 0;
			private double offsetTop = 0;

			#endregion // Data

			#region Constructor

			/// <summary>
			/// Initializes a new instance of DragVisualAdorner.
			/// </summary>
			/// <param name="adornedElement">The element being adorned.</param>
			/// <param name="size">The size of the adorner.</param>
			/// <param name="brush">A brush to with which to paint the adorner.</param>
			public DragDropAdorner(UIElement adornedElement, Size size, Brush brush)
				: base(adornedElement)
			{
				Rectangle rect = new Rectangle();
				rect.Fill = brush;
				rect.Width = size.Width;
				rect.Height = size.Height;
				rect.IsHitTestVisible = false;
				this.child = rect;
			}

			#endregion // Constructor

			#region Public Interface

			#region GetDesiredTransform

			/// <summary>
			/// Override.
			/// </summary>
			/// <param name="transform"></param>
			/// <returns></returns>
			public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
			{
				GeneralTransformGroup result = new GeneralTransformGroup();
				result.Children.Add(base.GetDesiredTransform(transform));
				result.Children.Add(new TranslateTransform(this.offsetLeft, this.offsetTop));
				return result;
			}

			#endregion // GetDesiredTransform

			#region OffsetLeft

			/// <summary>
			/// Gets/sets the horizontal offset of the adorner.
			/// </summary>
			public double OffsetLeft
			{
				get { return this.offsetLeft; }
				set
				{
					this.offsetLeft = value;
					UpdateLocation();
				}
			}

			#endregion // OffsetLeft

			#region SetOffsets

			/// <summary>
			/// Updates the location of the adorner in one atomic operation.
			/// </summary>
			/// <param name="left"></param>
			/// <param name="top"></param>
			public void SetOffsets(double left, double top)
			{
				this.offsetLeft = left;
				this.offsetTop = top;
				this.UpdateLocation();
			}

			#endregion // SetOffsets

			#region OffsetTop

			/// <summary>
			/// Gets/sets the vertical offset of the adorner.
			/// </summary>
			public double OffsetTop
			{
				get { return this.offsetTop; }
				set
				{
					this.offsetTop = value;
					UpdateLocation();
				}
			}

			#endregion // OffsetTop

			#endregion // Public Interface

			#region Protected Overrides

			/// <summary>
			/// Override.
			/// </summary>
			/// <param name="constraint"></param>
			/// <returns></returns>
			protected override Size MeasureOverride(Size constraint)
			{
				this.child.Measure(constraint);
				return this.child.DesiredSize;
			}

			/// <summary>
			/// Override.
			/// </summary>
			/// <param name="finalSize"></param>
			/// <returns></returns>
			protected override Size ArrangeOverride(Size finalSize)
			{
				this.child.Arrange(new Rect(finalSize));
				return finalSize;
			}

			/// <summary>
			/// Override.
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			protected override Visual GetVisualChild(int index)
			{
				return this.child;
			}

			/// <summary>
			/// Override.  Always returns 1.
			/// </summary>
			protected override int VisualChildrenCount
			{
				get { return 1; }
			}

			#endregion // Protected Overrides

			#region Private Helpers

			private void UpdateLocation()
			{
				AdornerLayer adornerLayer = this.Parent as AdornerLayer;
				if (adornerLayer != null)
					adornerLayer.Update(this.AdornedElement);
			}

			#endregion // Private Helpers
		}

		#endregion

		private Point? _mouseDown;
		private Point _offset;
		private ItemsControl _source;
		private UIElement _dragScope;
		private DragDropAdorner _adorner;
		private DragEventHandler _dragOver, _dragEnter, _dragLeave;

		public ItemsControlDragHelper(ItemsControl source, UIElement dragScope)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			_dragEnter = new DragEventHandler(DragScope_DragEnter);
			_dragOver = new DragEventHandler(DragScope_DragOver);
			_dragLeave = new DragEventHandler(DragScope_DragLeave);

			_source = source;
			_dragScope = dragScope;
			_source.PreviewMouseLeftButtonDown += SourceMouseLeftButtonDown;
			_source.PreviewMouseMove += SourceMouseMove;
		}

		private void SourceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var point = e.GetPosition(_source);
			if (!IsMouseOverScrollbar(point))
				_mouseDown = point;
		}

		private void SourceMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (_mouseDown.HasValue && HasMoved(e.GetPosition(_source)))
				{
					var adornerSource = e.OriginalSource as UIElement;
					adornerSource = _source.ContainerFromElement(adornerSource) as UIElement;

					var input = adornerSource as IInputElement;
					if (input != null)
						_offset = e.GetPosition(input);
					else
						_offset = new Point(0, 0);

					object subj = e.OriginalSource;
					var fe = adornerSource as FrameworkElement;
					if (fe != null && fe.Tag != null)
						subj = fe.Tag;

					DoDragDrop(subj, adornerSource);
					e.Handled = true;
				}
			}
			else
				_mouseDown = null;
		}

		private bool HasMoved(Point point)
		{
			return Math.Abs(point.X - _mouseDown.Value.X) > SystemParameters.MinimumHorizontalDragDistance / 2
				|| Math.Abs(point.Y - _mouseDown.Value.Y) > SystemParameters.MinimumVerticalDragDistance / 2;
		}

		private bool IsMouseOverScrollbar(Point ptMouse)
		{
			HitTestResult res = VisualTreeHelper.HitTest(_source, ptMouse);
			if (res == null)
				return false;

			DependencyObject depObj = res.VisualHit;
			while (depObj != null)
			{
				if (depObj is System.Windows.Controls.Primitives.ScrollBar)
					return true;

				// VisualTreeHelper works with objects of type Visual or Visual3D.
				// If the current object is not derived from Visual or Visual3D,
				// then use the LogicalTreeHelper to find the parent element.
				if (depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D)
					depObj = VisualTreeHelper.GetParent(depObj);
				else
					depObj = LogicalTreeHelper.GetParent(depObj);
			}

			return false;
		}

		private void DoDragDrop(object dragItem, UIElement adornerSource)
		{
			if (adornerSource != null)
			{
				var rect = VisualTreeHelper.GetDescendantBounds(adornerSource);
				var size = new Size((double)Math.Ceiling(rect.Width), (double)Math.Ceiling(rect.Height));
				var brush = new VisualBrush(adornerSource);
				_adorner = new DragDropAdorner(_dragScope, size, brush);
				_adorner.Opacity = 0.7;
				_adorner.Visibility = Visibility.Hidden;
			}

			DragDrop.AddPreviewDragEnterHandler(_dragScope, _dragEnter);
			DragDrop.AddPreviewDragOverHandler(_dragScope, _dragOver);
			DragDrop.AddPreviewDragLeaveHandler(_dragScope, _dragLeave);

			var resultEffects = DragDrop.DoDragDrop(_source, dragItem, DragDropEffects.All);
			DragFinished(resultEffects);

			DragDrop.RemovePreviewDragEnterHandler(_dragScope, _dragEnter);
			DragDrop.RemovePreviewDragOverHandler(_dragScope, _dragOver);
			DragDrop.RemovePreviewDragLeaveHandler(_dragScope, _dragLeave);
		}

		private void DragScope_DragEnter(object sender, DragEventArgs args)
		{
			if (_adorner != null)
				AdornerLayer.GetAdornerLayer(_source).Add(_adorner);
		}

		private void DragScope_DragOver(object sender, DragEventArgs args)
		{
			if (_adorner != null)
			{
				var pos = args.GetPosition(_dragScope);
				_adorner.SetOffsets(pos.X - _offset.X, pos.Y - _offset.Y);
				_adorner.Visibility = Visibility.Visible;
			}
		}

		private void DragScope_DragLeave(object sender, DragEventArgs args)
		{
			if (_adorner != null)
				AdornerLayer.GetAdornerLayer(_source).Remove(_adorner);
		}

		protected void DragFinished(DragDropEffects ret)
		{
			if (_adorner != null)
				AdornerLayer.GetAdornerLayer(_source).Remove(_adorner);
			_adorner = null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			_source.MouseLeftButtonDown -= SourceMouseLeftButtonDown;
			_source.MouseMove -= SourceMouseMove;
		}

		#endregion
	}
}
