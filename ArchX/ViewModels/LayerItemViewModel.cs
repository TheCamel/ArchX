using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ArchX.Controls.Layers;
using GalaSoft.MvvmLight.Messaging;

namespace ArchX.ViewModels
{
	public class LayerItemViewModel : ViewModelBaseExtended
	{
		public LayerItemViewModel( LayerItem data )
		{
			//if (string.IsNullOrEmpty(data.Name))
			//	data.Name = "Default";

			Data = data;
		}

		new public LayerItem Data { get; set; }

		//public Int64 Identifier { get; set; }
		public string Name
		{
			get { return Data.Name; }
			set
			{
				if (Data.Name != value)
				{
					Data.Name = value;
					RaisePropertyChanged("Name");

					Messenger.Default.Send<NotificationMessage<LayerItemViewModel>>(
						new NotificationMessage<LayerItemViewModel>(this, ViewModelActions.Modified),
						ViewModelMessages.UpdateLayers);
				}
			}
		}

		public bool IsVisible
		{
			get { return Data.IsVisible; }
			set
			{
				if (Data.IsVisible != value)
				{
					Data.IsVisible = value;
					RaisePropertyChanged("IsVisible");

					Messenger.Default.Send<NotificationMessage<LayerItemViewModel>>(
						new NotificationMessage<LayerItemViewModel>(this, ViewModelActions.Modified),
						ViewModelMessages.UpdateLayers);
				}
			}
		}

		//private bool _IsLocked;
		public bool IsLocked
		{
			get { return Data.IsLocked; }
			set
			{
				if (Data.IsLocked != value)
				{
					Data.IsLocked = value;
					RaisePropertyChanged("IsLocked");

					Messenger.Default.Send<NotificationMessage<LayerItemViewModel>>(
						new NotificationMessage<LayerItemViewModel>(this, ViewModelActions.Modified),
						ViewModelMessages.UpdateLayers);
				}
			}
		}
		public Color Color { get; set; }
	}
}
