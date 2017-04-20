using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchX.ViewModels
{
	public class ViewModelMessages
	{
		/// <summary>
		/// from explorer and home to main
		/// </summary>
		public const string ContextCommand = "ContextCommand";

		/// <summary>
		/// Active document changed
		/// </summary>
		public const string DocumentRequestClose = "DocumentRequestClose";

		/// <summary>
		/// Active document changed
		/// </summary>
		public const string DocumentActivChanged = "DocumentActivChanged";

		/// <summary>
		/// 
		/// </summary>
		public const string MenuItemCommand = "MenuItemCommand";

		/// <summary>
		/// 
		/// </summary>
		public const string UpdateLayers = "UpdateLayers";

	
	}

	public class ViewModelActions
	{
		public const string Added = "Added";
		public const string Removed = "Removed";
		public const string Selected = "Selected";
		public const string Modified = "Modified";
	}
}
