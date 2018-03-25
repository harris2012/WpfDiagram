using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Aga.Diagrams.Controls
{
	public class RectPort : PortBase
	{
		static RectPort()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
				typeof(RectPort), new FrameworkPropertyMetadata(typeof(RectPort)));
		}

		public override Point GetEdgePoint(Point target)
		{
			var rect = new Rect(Center.X - ActualWidth / 2, Center.Y - ActualHeight / 2, ActualWidth, ActualHeight);
			return GeometryHelper.RectLineIntersection(rect, target);
		}

		public override bool IsNear(Point point)
		{
			var rect = new Rect(Center.X - ActualWidth / 2, Center.Y - ActualHeight / 2, ActualWidth, ActualHeight);
			rect.Inflate(Sensitivity, Sensitivity);
			return GeometryHelper.RectContains(rect, point);
		}
	}
}
