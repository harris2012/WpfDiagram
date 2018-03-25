using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using Aga.Diagrams.Adorners;
using System.Windows.Documents;

namespace Aga.Diagrams.Controls
{
	public abstract class LinkBase : DiagramItem, ILink, INotifyPropertyChanged
	{
		#region Properties

		#region CanRelink Property

		public bool CanRelink
		{
			get { return (bool)GetValue(CanRelinkProperty); }
			set { SetValue(CanRelinkProperty, value); }
		}

		public static readonly DependencyProperty CanRelinkProperty =
			DependencyProperty.Register("CanRelink",
									   typeof(bool),
									   typeof(LinkBase),
									   new FrameworkPropertyMetadata(true));

		#endregion

		private IPort _source;
		public IPort Source
		{
			get { return _source; }
			set
			{
				if (_source != null)
					_source.Links.Remove(this);
				_source = value;
				if (_source != null)
					_source.Links.Add(this);
			}
		}

		private IPort _target;
		public IPort Target
		{
			get { return _target; }
			set
			{
				if (_target != null)
					_target.Links.Remove(this);
				_target = value;
				if (_target != null)
					_target.Links.Add(this);
			}
		}

		public Point? SourcePoint { get; set; }
		public Point? TargetPoint { get; set; }


		private bool _startCap;
		public bool StartCap
		{
			get { return _startCap; }
			set
			{
				_startCap = value;
				OnPropertyChanged("StartCap");
			}
		}

		private bool _endCap;
		public bool EndCap
		{
			get { return _endCap; }
			set
			{
				_endCap = value;
				OnPropertyChanged("EndCap");
			}
		}

		private Brush _brush = new SolidColorBrush(Colors.Black);
		public Brush Brush
		{
			get { return _brush; }
			set { _brush = value; }
		}

		private Point _startPoint;
		public Point StartPoint
		{
			get { return _startPoint; }
			protected set
			{
				_startPoint = value;
				OnPropertyChanged("StartPoint");
			}
		}

		private Point _endPoint;
		public Point EndPoint
		{
			get { return _endPoint; }
			protected set
			{
				_endPoint = value;
				OnPropertyChanged("EndPoint");
			}
		}

		private double _startCapAngle;
		public double StartCapAngle
		{
			get { return _startCapAngle; }
			protected set
			{
				_startCapAngle = value;
				OnPropertyChanged("StartCapAngle");
			}
		}

		private double _endCapAngle;
		public double EndCapAngle
		{
			get { return _endCapAngle; }
			protected set
			{
				_endCapAngle = value;
				OnPropertyChanged("EndCapAngle");
			}
		}

		private PathGeometry _pathGeomtry;
		public PathGeometry PathGeometry
		{
			get { return _pathGeomtry; }
			protected set
			{
				_pathGeomtry = value;
				OnPropertyChanged("PathGeometry");
			}
		}

		private Point _labelPosition;
		public Point LabelPosition
		{
			get { return _labelPosition; }
			set
			{
				_labelPosition = value;
				OnPropertyChanged("LabelPosition");
			}
		}

		#region Label Property

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty =
			DependencyProperty.Register("Label", typeof(string), typeof(LinkBase));

		#endregion

		public override Rect Bounds
		{
			get
			{
				var x = Math.Min(StartPoint.X, EndPoint.X);
				var y = Math.Min(StartPoint.Y, EndPoint.Y);
				var mx = Math.Max(StartPoint.X, EndPoint.X);
				var my = Math.Max(StartPoint.Y, EndPoint.Y);
				return new Rect(x, y, mx - x, my - y);
			}
		}

		#endregion

		protected LinkBase()
		{
			UpdatePath();
		}

		protected override Adorner CreateSelectionAdorner()
		{
			return new SelectionAdorner(this, new RelinkControl());
		}

		public abstract void UpdatePath();

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(name));
		}
		#endregion
	}
}
