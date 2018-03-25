using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Aga.Diagrams
{
	public static class GeometryHelper
	{
		public static readonly Point NullPoint = new Point(Double.NaN, Double.NaN);

		public static double NormalAngle(Point start, Point end)
		{
			var normal = new Vector(1, 0);
			var v = new Vector(end.X - start.X, end.Y - start.Y);
			return Vector.AngleBetween(normal, v);
		}

		public static double Length(Point start, Point end)
		{
			var v = new Vector(end.X - start.X, end.Y - start.Y);
			return v.Length;
		}

		public static Point EllipseLineIntersection(double a, double b, Point p)
		{
			var c = a * b / Math.Sqrt(a * a * p.Y * p.Y + b * b * p.X * p.X);
			var p1 = new Point(c * p.X, c * p.Y);
			var p2 = new Point(-c * p.X, -c * p.Y);
			return Length(p1, p) < Length(p2, p) ? p1 : p2;
		}

		public static Point RectLineIntersection(Rect rect, Point p)
		{
			var c = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			Point res;
			if (SegmentsIntersection(new Point(rect.X, rect.Y), new Point(rect.Right, rect.Y), c, p, out res))
				return res;
			if (SegmentsIntersection(new Point(rect.Right, rect.Y), new Point(rect.Right, rect.Bottom), c, p, out res))
				return res;
			if (SegmentsIntersection(new Point(rect.Right, rect.Bottom), new Point(rect.X, rect.Bottom), c, p, out res))
				return res;
			SegmentsIntersection(new Point(rect.X, rect.Bottom), new Point(rect.X, rect.Y), c, p, out res);
			return res;
		}

		public static bool SegmentsIntersection(Point a1, Point a2, Point b1, Point b2, out Point res)
		{
			var d = (a1.X - a2.X) * (b2.Y - b1.Y) - (a1.Y - a2.Y) * (b2.X - b1.X);
			var da = (a1.X - b1.X) * (b2.Y - b1.Y) - (a1.Y - b1.Y) * (b2.X - b1.X);
			var db = (a1.X - a2.X) * (a1.Y - b1.Y) - (a1.Y - a2.Y) * (a1.X - b1.X);
			if (Math.Abs(d) > 0.000001)
			{
				var ta = da / d;
				var tb = db / d;
				if (0 <= ta && ta <= 1 && 0 <= tb && tb <= 1)
				{
					res = new Point(a1.X + ta * (a2.X - a1.X), a1.Y + ta * (a2.Y - a1.Y));
					return true;
				}
			}
			res = NullPoint;
			return false;
		}

		public static Point SegmentMiddlePoint(Point p1, Point p2)
		{
			return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}

		public static bool RectContains(Rect rect, Point point)
		{
			return rect.X <= point.X && rect.Right >= point.X
				&& rect.Y <= point.Y && rect.Bottom >= point.Y;
		}

		public static bool EllipseContains(Point center, double a, double b, Point point)
		{
			var p = point - center;
			return (p.X * p.X) / (a * a) + (p.Y * p.Y) / (b * b) <= 1;
		}

		public static bool AreEquals(double a, double b)
		{
			return Math.Abs(a - b) < 0.0001;
		}
	}
}
