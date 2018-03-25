using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using Aga.Diagrams.Adorners;

namespace Aga.Diagrams.Controls
{
	public class DragThumb: Control
	{
		public DragThumbKinds Kind { get; set; }

		protected Point? MouseDownPoint { get; set; }

		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				var item = this.DataContext as DiagramItem;
				var view = VisualHelper.FindParent<DiagramView>(item);
				if (item != null && view != null)
				{
					MouseDownPoint = e.GetPosition(view);
					view.DragTool.BeginDrag(MouseDownPoint.Value, item, this.Kind);
					e.Handled = true;
				}
			}
		}
	}
}
