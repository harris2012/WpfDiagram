using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Controls;
using System.Windows;

namespace Aga.Diagrams
{
	public class LinkInfo
	{
		public IPort Source { get; set; }
		public IPort Target { get; set; }
		public Point? SourcePoint { get; set; }
		public Point? TargetPoint { get; set; }

		public LinkInfo(ILink link)
		{
			Source = link.Source;
			Target = link.Target;
			SourcePoint = link.SourcePoint;
			TargetPoint = link.TargetPoint;
		}

		public void UpdateLink(ILink link)
		{
			link.Source = Source;
			link.Target = Target;
			link.SourcePoint = SourcePoint;
			link.TargetPoint = TargetPoint;
		}
	}
}
