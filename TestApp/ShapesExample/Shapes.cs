using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace TestApp.ShapesExample
{
	class ShapeBase: INotifyPropertyChanged
	{
		private Point _location;
		public Point Location 
		{
			get { return _location; }
			set 
			{ 
				_location = value;
				OnPropertyChanged("Location");
			}
		}

		private Size _size;
		public Size Size
		{
			get { return _size; }
			set
			{
				_size = value;
				OnPropertyChanged("Size");
			}
		}

		private List<ShapeBase> _links = new List<ShapeBase>();
		[Browsable(false)]
		public List<ShapeBase> Links
		{
			get { return _links; }
		}

		public override string ToString()
		{
			return GetType().Name;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}

	class RectangleShape : ShapeBase
	{
	}

	class EllipseShape : ShapeBase
	{
	}
}
