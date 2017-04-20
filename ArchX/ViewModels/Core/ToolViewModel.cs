using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchX.ViewModels
{
	public abstract class ToolViewModel : PaneViewModel
	{
		#region -----------------CONSTRUCTOR-----------------

		protected ToolViewModel( string title )
		{
			DisplayName = title;
		}

		#endregion

		#region -----------------PROPERTIES-----------------


		#endregion
	}
}
