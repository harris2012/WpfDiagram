using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TestApp.Flowchart
{
	class FlowNode: INotifyPropertyChanged
	{
		[Browsable(false)]
		public NodeKinds Kind { get; private set; }

		private int _column;
		public int Column
		{
			get { return _column; }
			set 
			{ 
				_column = value;
				OnPropertyChanged("Column");
			}
		}

		private int _row;
		public int Row
		{
			get { return _row; }
			set 
			{ 
				_row = value;
				OnPropertyChanged("Row");
			}
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public FlowNode(NodeKinds kind)
		{
			Kind = kind;
		}

		public IEnumerable<PortKinds> GetPorts()
		{
			switch(Kind)
			{
				case NodeKinds.Start:
					yield return PortKinds.Bottom;
					break;
				case NodeKinds.End:
					yield return PortKinds.Top;
					break;
				case NodeKinds.Action:
					yield return PortKinds.Top;
					yield return PortKinds.Bottom;
					break;
				case NodeKinds.Condition:
					yield return PortKinds.Top;
					yield return PortKinds.Bottom;
					yield return PortKinds.Left;
					yield return PortKinds.Right;
					break;
			}
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

	enum NodeKinds { Start, End, Action, Condition }
}
