using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Aga.Diagrams.Controls;

namespace Aga.Diagrams.Adorners
{
	class RubberbandAdorner : DragAdorner
	{
		private Pen _pen;

		public RubberbandAdorner(DiagramView view, Point start)
			: base(view, start)
		{
			_pen = new Pen(Brushes.Black, 2);
		}

		protected override bool DoDrag()
		{
			InvalidateVisual();
			return true;
		}

		protected override void EndDrag()
		{
			if (DoCommit)
			{
				var rect = new Rect(Start, End);
				var items = View.Items.Where(p => p.CanSelect && rect.Contains(p.Bounds));
				View.Selection.SetRange(items);
			}
		}

		protected override void OnRender(DrawingContext dc)
		{
			dc.DrawRectangle(Brushes.Transparent, _pen, new Rect(Start, End));
		}
	}
}
