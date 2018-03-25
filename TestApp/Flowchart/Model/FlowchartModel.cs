using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TestApp.Flowchart
{
	class FlowchartModel
	{
		private ObservableCollection<FlowNode> _nodes = new ObservableCollection<FlowNode>();
		internal ObservableCollection<FlowNode> Nodes
		{
			get { return _nodes; }
		}

		private ObservableCollection<Link> _links = new ObservableCollection<Link>();
		internal ObservableCollection<Link> Links
		{
			get { return _links; }
		}
	}
}
