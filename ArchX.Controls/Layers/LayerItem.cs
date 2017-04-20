using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArchX.Controls.Layers
{
	public class LayerItem
	{
		public LayerItem()
		{
			IsVisible = true;
			IsLocked = false;
		}

		public Int64 Identifier { get; set; }
		public string Name { get; set; }
		public bool IsVisible { get; set; }
		public bool IsLocked { get; set; }
		public string Color { get; set; }
	}
}
