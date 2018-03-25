using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Aga.Diagrams.Controls
{
	public interface INode
	{
		IEnumerable<IPort> Ports { get; }
	}
}
