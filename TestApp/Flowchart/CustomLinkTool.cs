using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Tools;
using Aga.Diagrams.Controls;
using System.Windows;
using Aga.Diagrams;
using System.Windows.Controls;

namespace TestApp.Flowchart
{
	class CustomLinkTool: LinkTool
	{
		public CustomLinkTool(DiagramView view)
			: base(view)
		{
		}

		protected override ILink CreateNewLink(IPort port)
		{
			var link = new OrthogonalLink();
			BindNewLinkToPort(port, link);
			return link;
		}

		protected override void UpdateLink(Point point, IPort port)
		{
			base.UpdateLink(point, port);
			var link = Link as OrthogonalLink;
		}
	}
}
