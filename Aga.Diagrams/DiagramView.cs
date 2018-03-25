using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using Aga.Diagrams.Controls;
using Aga.Diagrams.Tools;
using System.Windows.Media;
using System.Windows.Input;

namespace Aga.Diagrams
{
	public class DiagramView : Canvas
	{
		private Pen _gridPen;

		static DiagramView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
				typeof(DiagramView), new FrameworkPropertyMetadata(typeof(DiagramView)));
		}

		#region Properties

		#region GridCellSize

		public static readonly DependencyProperty GridCellSizeProperty =
			DependencyProperty.Register("GridCellSize",
									   typeof(Size),
									   typeof(DiagramView),
									   new FrameworkPropertyMetadata(new Size(10, 10)));

		public Size GridCellSize
		{
			get { return (Size)GetValue(GridCellSizeProperty); }
			set { SetValue(GridCellSizeProperty, value); }
		}

		#endregion

		#region ShowGrid

		public static readonly DependencyProperty ShowGridProperty =
			DependencyProperty.Register("ShowGrid",
									   typeof(bool),
									   typeof(DiagramView),
									   new FrameworkPropertyMetadata(false));

		public bool ShowGrid
		{
			get { return (bool)GetValue(ShowGridProperty); }
			set { SetValue(ShowGridProperty, value); }
		}

		#endregion

		#region DocumentSize

		public static readonly DependencyProperty DocumentSizeProperty =
			DependencyProperty.Register("DocumentSize",
									   typeof(Size),
									   typeof(DiagramView),
									   new FrameworkPropertyMetadata(new Size(2000, 2000)));

		public Size DocumentSize
		{
			get { return (Size)GetValue(DocumentSizeProperty); }
			set { SetValue(DocumentSizeProperty, value); }
		}

		#endregion

		#region Zoom

		public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register("Zoom",
									   typeof(double),
									   typeof(DiagramView),
									   new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnZoomChanged)));

		private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var view = d as DiagramView;
			var zoom = (double)e.NewValue;
			view._gridPen = view.CreateGridPen();
			if (Math.Abs(zoom - 1) < 0.0001)
				view.LayoutTransform = null;
			else
				view.LayoutTransform = new ScaleTransform(zoom, zoom);
		}

		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}

		#endregion

		public Selection Selection { get; private set; }

		public IDiagramController Controller { get; set; }

		public IEnumerable<DiagramItem> Items
		{
			get { return Children.OfType<DiagramItem>(); }
		}

		private IInputTool _inputTool;
		public IInputTool InputTool
		{
			get { return _inputTool; }
			set
			{
				if (value == null) 
					throw new ArgumentNullException("value");
				_inputTool = value;
			}
		}

		private IMoveResizeTool _dragTool;
		public IMoveResizeTool DragTool
		{
			get { return _dragTool; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				_dragTool = value;
			}
		}

		private ILinkTool _linkTool;
		public ILinkTool LinkTool
		{
			get { return _linkTool; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				_linkTool = value;
			}
		}

		public IDragDropTool DragDropTool { get; set; }

		private Adorner _dragAdorner;
		public Adorner DragAdorner
		{
			get { return _dragAdorner; }
			set
			{
				if (_dragAdorner != value)
				{
					var adornerLayer = AdornerLayer.GetAdornerLayer(this);
					if (_dragAdorner != null)
						adornerLayer.Remove(_dragAdorner);
					_dragAdorner = value;
					if (_dragAdorner != null)
						adornerLayer.Add(_dragAdorner);
				}
			}
		}

		public bool IsDragging { get { return DragAdorner != null; } }

		#endregion

		public DiagramView()
		{
			_gridPen = CreateGridPen();
			Selection = new Selection();
			InputTool = new InputTool(this);
			DragTool = new MoveResizeTool(this);
			LinkTool = new LinkTool(this);
			BindCommands();
			Focusable = true;

			this.LayoutUpdated += new EventHandler(DiagramView_LayoutUpdated);
		}

		void DiagramView_LayoutUpdated(object sender, EventArgs e)
		{
			foreach (var n in this.Children.OfType<Node>())
				n.UpdatePosition();
		}

		public DiagramItem FindItem(object modelElement)
		{
			return Items.FirstOrDefault(p => p.ModelElement == modelElement);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			InputTool.OnMouseDown(e);
			base.OnMouseDown(e);
			Focus();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			InputTool.OnMouseMove(e);
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			InputTool.OnMouseUp(e);
			base.OnMouseUp(e);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			InputTool.OnPreviewKeyDown(e);
			base.OnPreviewKeyDown(e);
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			if (DragDropTool != null)
				DragDropTool.OnDragEnter(e);
			base.OnDragEnter(e);
		}

		protected override void OnDragLeave(DragEventArgs e)
		{
			if (DragDropTool != null)
				DragDropTool.OnDragLeave(e);
			base.OnDragLeave(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			if (DragDropTool != null)
				DragDropTool.OnDragOver(e);
			base.OnDragOver(e);
		}

		protected override void OnDrop(DragEventArgs e)
		{
			if (DragDropTool != null)
				DragDropTool.OnDrop(e);
			base.OnDrop(e);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			base.MeasureOverride(DocumentSize);
			return DocumentSize;
		}

		protected override void OnRender(DrawingContext dc)
		{
			var rect = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
			dc.DrawRectangle(Background, null, rect);
			if (ShowGrid && GridCellSize.Width > 0 && GridCellSize.Height > 0)
				DrawGrid(dc, rect);
		}

		protected virtual void DrawGrid(DrawingContext dc, Rect rect)
		{
			//using .5 forces wpf to draw a single pixel line
			for (var i = 0.5; i < rect.Height; i += GridCellSize.Height)
				dc.DrawLine(_gridPen, new Point(0, i), new Point(rect.Width, i));
			for (var i = 0.5; i < rect.Width; i += GridCellSize.Width)
				dc.DrawLine(_gridPen, new Point(i, 0), new Point(i, rect.Height));
		}

		protected virtual Pen CreateGridPen()
		{
			return new Pen(Brushes.LightGray, (1 / Zoom));
		}

		private void BindCommands()
		{
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, ExecuteCommand, CanExecuteCommand));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, ExecuteCommand, CanExecuteCommand));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, ExecuteCommand, CanExecuteCommand));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, ExecuteCommand, CanExecuteCommand));
		}

		private void ExecuteCommand(object sender, ExecutedRoutedEventArgs e)
		{
			if (Controller != null)
				Controller.ExecuteCommand(e.Command, e.Parameter);
			if (e.Command == ApplicationCommands.Delete)
				Selection.Clear();
		}

		private void CanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			if (Controller != null)
				e.CanExecute = Controller.CanExecuteCommand(e.Command, e.Parameter);
		}
	}
}
