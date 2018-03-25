using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Aga.Diagrams.Controls;
using System.Windows.Documents;
using System.Windows.Controls;
using Aga.Diagrams.Adorners;

namespace Aga.Diagrams.Tools
{
	public class LinkTool: ILinkTool
	{
		protected DiagramView View { get; private set; }
		protected IDiagramController Controller { get { return View.Controller; } }
		protected Point DragStart { get; set; }
		protected ILink Link { get; set; }
		protected LinkThumbKind Thumb { get; set; }
		protected LinkInfo InitialState { get; set; }
		protected LinkAdorner Adorner { get; set; }
		private bool _isNewLink;

		public LinkTool(DiagramView view)
		{
			View = view;
		}

		public void BeginDrag(Point start, ILink link, LinkThumbKind thumb)
		{
			BeginDrag(start, link, thumb, false);
		}

		protected virtual void BeginDrag(Point start, ILink link, LinkThumbKind thumb, bool isNew)
		{
			_isNewLink = isNew;
			DragStart = start;
			Link = link;
			Thumb = thumb;
			InitialState = new LinkInfo(link);
			Adorner = CreateAdorner();
			View.DragAdorner = Adorner;
		}

		public virtual void DragTo(Vector vector)
		{
			vector = UpdateVector(vector);
			var point = DragStart + vector;
			var port = View.Children.OfType<INode>().SelectMany(p => p.Ports)
				.Where(p => p.IsNear(point) && CanLinkTo(p))
				.OrderBy(p => GeometryHelper.Length(p.Center, point))
				.FirstOrDefault();

			UpdateLink(point, port);

			Adorner.Port = port;
			Link.UpdatePath();
		}

		protected virtual void UpdateLink(Point point, IPort port)
		{
			if (Thumb == LinkThumbKind.Source)
			{
				Link.Source = port;
				Link.SourcePoint = port == null ? point : (Point?)null;
			}
			else
			{
				Link.Target = port;
				Link.TargetPoint = port == null ? point : (Point?)null;
			}
		}

		protected virtual bool CanLinkTo(IPort port)
		{
			var pb = port as PortBase;
			if (pb != null)
			{
				if (Thumb == LinkThumbKind.Source)
					return pb.CanAcceptOutgoingLinks;
				else
					return pb.CanAcceptIncomingLinks;
			}
			else
				return true;
		}

		public virtual bool CanDrop()
		{
			return Adorner.Port != null;
		}

		public virtual void EndDrag(bool doCommit)
		{
			if (doCommit)
			{
				Controller.UpdateLink(InitialState, Link);
			}
			else
			{
				if (_isNewLink)
					View.Children.Remove((Control)Link);
				else
					InitialState.UpdateLink(Link);
			}
			Link.UpdatePath();
			Link = null;
			Adorner = null;
		}

		public virtual void BeginDragNewLink(Point start, IPort port)
		{
			var link = CreateNewLink(port);
			if (link != null && link is Control)
			{
				var thumb = (link.Source != null) ? LinkThumbKind.Target : LinkThumbKind.Source;
				View.Children.Add((Control)link);
				BeginDrag(start, link, thumb, true);
			}
		}

		protected virtual ILink CreateNewLink(IPort port)
		{
			var link = new SegmentLink();
			BindNewLinkToPort(port, link);
			return link;
		}

		protected virtual void BindNewLinkToPort(IPort port, LinkBase link)
		{
			link.EndCap = true;
			var portBase = port as PortBase;
			if (portBase != null)
			{
				if (portBase.CanAcceptIncomingLinks && !portBase.CanAcceptOutgoingLinks)
					link.Target = port;
				else
					link.Source = port;
			}
			else
				link.Source = port;
		}

		protected virtual LinkAdorner CreateAdorner()
		{
			return new LinkAdorner(View, DragStart) { Cursor = Cursors.Cross };
		}

		protected virtual Vector UpdateVector(Vector vector)
		{
			return vector;
		}
	}
}
