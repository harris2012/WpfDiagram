using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Aga.Diagrams.Controls
{
	public class SelectionFrame : Control
	{
		static SelectionFrame()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionFrame), new FrameworkPropertyMetadata(typeof(SelectionFrame)));
		}
	}
}
