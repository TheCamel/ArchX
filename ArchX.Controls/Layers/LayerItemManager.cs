using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchX.Controls.Layers
{
	public class LayerItemManager
	{

		#region --------------CONSTRUCTOR--------------
		internal LayerItemManager(Ruler parent)
		{
			_Container = parent;
			//Layers.Add(new LayerItem() { Name = "Default" });
		}

		#endregion

		#region --------------PROPERTIES--------------

		private Ruler _Container;

		private List<LayerItem> _Layers = new List<LayerItem>();
		internal List<LayerItem> Layers
		{
			get { return _Layers; }
			set { _Layers = value; }
		}

		private LayerItem _CurrentLayer = null;
		internal LayerItem CurrentLayer
		{
			get { return _CurrentLayer; }
			set { _CurrentLayer = value; }
		}

		#endregion

		#region --------------METHODS--------------

		public void Add(LayerItem item)
		{
			CurrentLayer = item;
			Layers.Add(CurrentLayer);
		}

		public void Remove(LayerItem item)
		{
			if (CurrentLayer == item)
			{
				CurrentLayer = Layers.First();
			}

			Layers.Remove(item);
		}

		public void Select(LayerItem item)
		{
			CurrentLayer = item;
		}

		public void Modify(LayerItem item)
		{
			LayerItem source = Layers.Single(p=>p.Identifier == item.Identifier );
			source.IsLocked = item.IsLocked;
			source.IsVisible = item.IsVisible;
			source.Color = item.Color;
			source.Name = item.Name;
		}

		#endregion
	}
}
