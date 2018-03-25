using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aga.Diagrams.Tools
{
	public interface IDragDropTool
	{
		void OnDragEnter(System.Windows.DragEventArgs e);
		void OnDragOver(System.Windows.DragEventArgs e);
		void OnDragLeave(System.Windows.DragEventArgs e);
		void OnDrop(System.Windows.DragEventArgs e);
	}
}
