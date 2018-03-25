using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Aga.Diagrams.Adorners;
using Aga.Diagrams.Controls;

namespace Aga.Diagrams.Tools
{
	public class InputTool : IInputTool
	{
		protected DiagramView View { get; private set; }
		protected IDiagramController Controller { get { return View.Controller; } }
		protected Point? MouseDownPoint { get; set; }
		protected DiagramItem MouseDownItem { get; set; }

		public InputTool(DiagramView view)
		{
			View = view;
		}

		public virtual void OnMouseDown(MouseButtonEventArgs e)
		{
			MouseDownItem = (e.OriginalSource as DependencyObject).FindParent<DiagramItem>();
			MouseDownPoint = e.GetPosition(View);
			e.Handled = true;
		}

		public virtual void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && MouseDownPoint.HasValue)
			{
				if (MouseDownItem != null)
				{
					View.DragTool.BeginDrag(MouseDownPoint.Value, MouseDownItem, DragThumbKinds.Center);
				}
				else
				{
					View.Selection.Clear();
					View.DragAdorner = CreateRubberbandAdorner();
				}
				MouseDownItem = null;
				MouseDownPoint = null;
			}
			e.Handled = true;
		}

		public virtual void OnMouseUp(MouseButtonEventArgs e)
		{
			if (MouseDownPoint == null)
				return;

			var item = (e.Source as DependencyObject).FindParent<DiagramItem>();
			SelectItem(item);
			e.Handled = true;
		}

		public virtual void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				if (View.DragAdorner != null && View.DragAdorner.IsMouseCaptured)
					View.DragAdorner.ReleaseMouseCapture();
				else
					View.Selection.Clear();
			}
		}

		protected virtual void SelectItem(DiagramItem item)
		{
			var sel = View.Selection;
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				if (item != null && item.CanSelect)
				{
					if (item.IsSelected)
						sel.Remove(item);
					else
						sel.Add(item);
				}
			}
			else if (Keyboard.Modifiers == ModifierKeys.Shift)
			{
				if (item != null && item.CanSelect)
					sel.Add(item);
			}
			else
			{
				if (item != null && item.CanSelect)
					sel.Set(item);
				else
					sel.Clear();
			}
		}

		protected virtual Adorner CreateRubberbandAdorner()
		{
			return new RubberbandAdorner(View, MouseDownPoint.Value);
		}
	}
}
