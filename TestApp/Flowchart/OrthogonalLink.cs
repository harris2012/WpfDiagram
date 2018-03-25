using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Aga.Diagrams.Controls;
using Aga.Diagrams;
using System.Windows.Controls;

namespace TestApp.Flowchart
{
	public class OrthogonalLink : SegmentLink
	{
		public OrthogonalLink()
		{
		}

		protected override Point[] CalculateSegments()
		{
			if (Source != null && Target != null)
			{
				var points = new List<Point>();
				var ends = GetEndPoinds();
				if (ends == null)
					return ends;
				else
				{
					points.Add(ends[0]);
					points.AddRange(GetMiddlePoints(ends[0], ends[1]));
					points.Add(ends[1]);
					var res = points.ToArray();
					UpdateEdges(res);
					return res;
				}
			}
			else
				return base.CalculateSegments();
		}

		private IEnumerable<Point> GetMiddlePoints(Point start, Point end)
		{
			var view = VisualHelper.FindParent<DiagramView>(this);
			if (view == null)
				return new Point[0];

			var points = new List<Point>();

			var p0 = GetFirstSegment(Source, start, view.GridCellSize);
			var p1 = GetFirstSegment(Target, end, view.GridCellSize);

			if (p0 == p1)
				return points;

			
			var p2 = new Point(GetNearestCross(p0.X, p1.X), GetNearestCross(p0.Y, p1.Y));
			var p3 = new Point(GetNearestCross(p1.X, p0.X), GetNearestCross(p1.Y, p0.Y));
			if (p2 == p3)
			{
				points.Add(p0);
				points.Add(p2);
				points.Add(p1);
			}
			else
			{
				points.Add(p0);
				points.Add(p2);
				if (!GeometryHelper.AreEquals(p2.X, p3.X) && !GeometryHelper.AreEquals(p2.Y, p3.Y))
					points.Add(new Point(p2.X, p3.Y));
				points.Add(p3);
				points.Add(p1);
			}
			DoScale(points, view.GridCellSize);
			return points;
		}

		private void DoScale(List<Point> points, Size cellSize)
		{
			for (int i = 0; i < points.Count; i++)
			{
				points[i] = new Point(points[i].X * cellSize.Width,
					points[i].Y * cellSize.Height);
			}
		}

		private double GetNearestCross(double a, double b)
		{
			if (GeometryHelper.AreEquals(a, b) && (int)a == a)
				return a;
			else if (a < b)
				return Math.Ceiling(a);
			else
				return Math.Floor(a);
		}

		private Point GetFirstSegment(IPort iPort, Point point, Size cellSize)
		{
			var port = iPort as PortBase;
			double x = (int)(point.X / cellSize.Width) + 0.5;
			double y = (int)(point.Y / cellSize.Height) + 0.5;
			if (port.VerticalAlignment == VerticalAlignment.Top)
				return new Point(x, y - 0.5);
			else if (port.VerticalAlignment == VerticalAlignment.Bottom)
				return new Point(x, y + 0.5);
			else if (port.HorizontalAlignment == HorizontalAlignment.Left)
				return new Point(x - 0.5, y);
			else
				return new Point(x + 0.5, y);
		}
	}
}
