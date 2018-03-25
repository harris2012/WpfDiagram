using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Aga.Diagrams
{
	public class DiagramScrollView: ScrollViewer
	{
		double _dx = 0;
		double _dy = 0;
		private DispatcherTimer _timer;

		public double Sensitivity { get; set; }
		public double ScrollStep { get; set; }
		public double Delay 
		{
			get { return _timer.Interval.TotalMilliseconds; }
			set { _timer.Interval = TimeSpan.FromMilliseconds(value); }
		}

		public DiagramScrollView()
		{
			_timer = new DispatcherTimer();
			_timer.Tick += new EventHandler(Tick);

			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			Focusable = false;
			Sensitivity = 20;
			ScrollStep = 16;
			Delay = 50;
		}

		private void Tick(object sender, EventArgs e)
		{
			if (!(Content is DiagramView) || !((DiagramView)Content).IsDragging)
				return;

			if (_dx != 0)
				this.ScrollToHorizontalOffset(this.HorizontalOffset + _dx);
			if (_dy != 0)
				this.ScrollToVerticalOffset(this.VerticalOffset + _dy);
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			if (!(Content is DiagramView) || !((DiagramView)Content).IsDragging)
			{
				_timer.IsEnabled = false;
			}
			else
			{
				_timer.IsEnabled = true;
				var point = e.GetPosition(this);
				_dx = _dy = 0;
				if (point.X < Sensitivity)
					_dx = -ScrollStep;
				else if (point.X > this.ActualWidth - Sensitivity)
					_dx = +ScrollStep;

				if (point.Y < Sensitivity)
					_dy = -ScrollStep;
				else if (point.Y > this.ActualHeight - Sensitivity)
					_dy = +ScrollStep;
			}
			base.OnPreviewMouseMove(e);
		}
	}
}
