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
	public class LinkAdorner : DragAdorner
	{
		private Pen _pen;

		private IPort _port;
		public IPort Port
		{
			get { return _port; }
			set
			{
				if (_port != value)
				{
					_port = value;
					InvalidateVisual();
				}
			}
		}

		public LinkAdorner(DiagramView view, Point start)
			: base(view, start)
		{
			_pen = new Pen(new SolidColorBrush(Colors.Red), 1);
		}

		protected override bool DoDrag()
		{
			View.LinkTool.DragTo(End - Start);
			return View.LinkTool.CanDrop();
		}

		protected override void EndDrag()
		{
			View.LinkTool.EndDrag(DoCommit);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (Port != null)
			{
				var p = Port.Center;
				drawingContext.DrawLine(_pen, new Point(p.X, p.Y - 3.5), new Point(p.X, p.Y + 3.5));
				drawingContext.DrawLine(_pen, new Point(p.X - 3.5, p.Y), new Point(p.X + 3.5, p.Y));
			}
		}
	}
}
