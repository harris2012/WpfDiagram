using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Aga.Diagrams.Adorners
{
	public abstract class DragAdorner: Adorner
	{
		public DiagramView View { get; private set; }
		protected bool DoCommit { get; set; }
		private bool CanDrop { get; set; }
		protected Point Start { get; set; }
		protected Point End { get; set; }

		protected DragAdorner(DiagramView view, Point start): base(view)
		{
			View = view;
			End = Start = start;
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			DoCommit = false;
			CaptureMouse();
		}

		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			End = e.GetPosition(View);
			CanDrop = DoDrag();
			Mouse.OverrideCursor = CanDrop ? Cursor : Cursors.No;
		}

		protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
		{
			if (this.IsMouseCaptured)
			{
				DoCommit = CanDrop;
				this.ReleaseMouseCapture();
			}
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			View.DragAdorner = null;
			Mouse.OverrideCursor = null;
			EndDrag();
		}

		/// <summary>
		/// Returns true if drop is possible at this location
		/// </summary>
		protected abstract bool DoDrag();
		protected abstract void EndDrag();
	}
}
