using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Controls;
using System.Windows;

namespace Aga.Diagrams
{
	public interface IDiagramController
	{
		/// <summary>
		/// Is called when user move/resize an item
		/// </summary>
		/// <param name="items">Selected items</param>
		/// <param name="bounds">New item bounds</param>
		void UpdateItemsBounds(DiagramItem[] items, Rect[] bounds);

		/// <summary>
		/// Is called when user create a link between items
		/// </summary>
		/// <param name="initialState">the state of the link before user action</param>
		/// <param name="link">Link in the current state</param>
		void UpdateLink(LinkInfo initialState, ILink link);

		void ExecuteCommand(System.Windows.Input.ICommand command, object parameter);

		bool CanExecuteCommand(System.Windows.Input.ICommand command, object parameter);
	}
}
