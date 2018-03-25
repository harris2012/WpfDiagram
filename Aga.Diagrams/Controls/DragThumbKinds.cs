using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aga.Diagrams
{
	[Flags]
	public enum DragThumbKinds
	{
		None = 0,
		Top = 1, 
		Left = 2, 
		Bottom = 4, 
		Right = 8,
		Center = 16,
		TopLeft = Top | Left,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right
	}
}
