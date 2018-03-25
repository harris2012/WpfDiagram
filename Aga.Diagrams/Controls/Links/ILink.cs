using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Aga.Diagrams.Controls
{
	public interface ILink
	{
		IPort Source { get; set; }
		IPort Target { get; set; }
		Point? SourcePoint { get; set; }
		Point? TargetPoint { get; set; }

		void UpdatePath();
	}
}
