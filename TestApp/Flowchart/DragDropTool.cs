using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Tools;
using Aga.Diagrams;
using System.Windows;

namespace TestApp.Flowchart
{
	class DragDropTool: IDragDropTool
	{
		DiagramView _view; 
		FlowchartModel _model;
		int _row, _column;

		public DragDropTool(DiagramView view, FlowchartModel model)
		{
			_view = view;
			_model = model;
		}

		public void OnDragEnter(System.Windows.DragEventArgs e)
		{
		}

		public void OnDragOver(System.Windows.DragEventArgs e)
		{
			e.Effects = DragDropEffects.None;
			if (e.Data.GetDataPresent(typeof(NodeKinds)))
			{
				var position = e.GetPosition(_view);
				_column = (int)(position.X / _view.GridCellSize.Width);
				_row = (int)(position.Y / _view.GridCellSize.Height);
				if (_column >= 0 && _row >= 0)
					if (_model.Nodes.Where(p => p.Column == _column && p.Row == _row).Count() == 0)
						e.Effects = e.AllowedEffects;
			}
			e.Handled = true;
		}

		public void OnDragLeave(System.Windows.DragEventArgs e)
		{
		}

		public void OnDrop(System.Windows.DragEventArgs e)
		{
			var node = new FlowNode((NodeKinds)e.Data.GetData(typeof(NodeKinds)));
			node.Row = _row;
			node.Column = _column;
			_model.Nodes.Add(node);
			e.Handled = true;
		}
	}
}
