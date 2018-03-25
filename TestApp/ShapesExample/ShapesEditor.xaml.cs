using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Aga.Diagrams.Tools;
using Aga.Diagrams;
using Aga.Diagrams.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace TestApp.ShapesExample
{
	/// <summary>
	/// Interaction logic for ShapesEditor.xaml
	/// </summary>
	public partial class ShapesEditor : UserControl
	{
		public ShapesEditor()
		{
			InitializeComponent();

			/*_diagramView.DragTool = new MoveResizeTool(_diagramView) 
			{ 
				MoveGridCell = _diagramView.GridCellSize,
				ResizeGridCell = _diagramView.GridCellSize
			};*/
			_diagramView.Controller = new ShapesController(_diagramView);
			_diagramView.Selection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Selection_PropertyChanged);
		}

		void Selection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var p = _diagramView.Selection.Primary;
			_propertiesView.SelectedObject = p != null ? p.ModelElement : null;
		}
	}
}
