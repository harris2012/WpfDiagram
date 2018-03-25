using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using Aga.Diagrams.Controls;

namespace Aga.Diagrams
{
	public class Selection : INotifyPropertyChanged, IEnumerable<DiagramItem>
	{
		private DiagramItem _primary;
		public DiagramItem Primary
		{
			get { return _primary; }
		}

		private Dictionary<DiagramItem, object> _items = new Dictionary<DiagramItem, object>();
		public IEnumerable<DiagramItem> Items
		{
			get { return _items.Keys; }
		}

		public int Count
		{
			get { return _items.Count; }
		}

		internal Selection()
		{
		}

		public bool Contains(DiagramItem item)
		{
			return _items.ContainsKey(item);
		}

		public void Add(DiagramItem item)
		{
			if (!_items.ContainsKey(item))
			{
				bool isPrimary = Count == 0;
				_items.Add(item, null);
				item.IsSelected = true;
				item.IsPrimarySelection = isPrimary;
				if (isPrimary)
				{
					_primary = item;
					OnPropertyChanged("Primary");
				}
				OnPropertyChanged("Items");
			}
		}

		public void Remove(DiagramItem item)
		{
			if (_items.ContainsKey(item))
			{
				item.IsSelected = false;
				_items.Remove(item);
			}
			if (_primary == item)
			{
				_primary = _items.Keys.FirstOrDefault();
				if (_primary != null)
					_primary.IsPrimarySelection = true;
				OnPropertyChanged("Primary");
			}
			OnPropertyChanged("Items");
		}

		public void Set(DiagramItem item)
		{
			SetRange(new DiagramItem[] { item });
		}

		public void SetRange(IEnumerable<DiagramItem> items)
		{
			DoClear();
			bool isPrimary = true;
			foreach (var item in items)
			{
				_items.Add(item, null);
				item.IsSelected = true;
				if (isPrimary)
				{
					_primary = item;
					item.IsPrimarySelection = true;
					isPrimary = false;
				}
			}
			OnPropertyChanged("Primary");
			OnPropertyChanged("Items");
		}

		public void Clear()
		{
			DoClear();
			OnPropertyChanged("Primary");
			OnPropertyChanged("Items");
		}

		private void DoClear()
		{
			foreach (var item in Items)
				item.IsSelected = false;
			_items.Clear();
			_primary = null;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		#endregion

		#region IEnumerable<object> Members

		IEnumerator<DiagramItem> IEnumerable<DiagramItem>.GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		#endregion
	}
}
