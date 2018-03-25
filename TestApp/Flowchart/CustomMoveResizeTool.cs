using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Tools;
using Aga.Diagrams;
using System.Windows;

namespace TestApp.Flowchart
{
	class CustomMoveResizeTool: MoveResizeTool
	{
		private FlowchartModel _model;
 
		public CustomMoveResizeTool(DiagramView view, FlowchartModel model)
			: base(view)
		{
			_model = model;
		}

		public override bool CanDrop()
		{
			foreach(var item in DragItems)
			{
				var column = (int)(item.Bounds.X / View.GridCellSize.Width);
				var row = (int)(item.Bounds.Y / View.GridCellSize.Height);
				if (_model.Nodes.Where(p => !IsDragged(p) && p.Row == row && p.Column == column).Count() != 0)
					return false;
			}
			return true;
		}

		private bool IsDragged(FlowNode node)
		{
			return DragItems.Where(p => p.ModelElement == node).Count() > 0;
		}
	}
}
