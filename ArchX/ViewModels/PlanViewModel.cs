using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ArchX.Controls;
using ArchX.Controls.Layers;
using ArchX.Tools;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace ArchX.ViewModels
{
	public class PlanViewModel : DocumentViewModel
	{
		#region ----------------CONSTRUCTOR----------------

		public PlanViewModel()
		{
			this.ContentId = "PlanViewModel";
			//this.Icon = "pack://application:,,,/Resources/Images/32x32/icon/home.png";

			//CultureManager.Instance.UICultureChanged += new CultureEventArrived(Instance_UICultureChanged);
			DisplayName = "noname";

			//Messenger.Default.Register<ITool>(this, "Tools",
			//	(ITool o) =>
			//{
			//	Tool = o;
			//});

			//Messenger.Default.Register<NotificationMessage>(this, "Options",
			//	(o) =>
			//	{
			//		if( o.Notification == "Mouse")
			//			ShowMousePosition = !ShowMousePosition;
			//	});
		}

		override public void Cleanup()
		{
			base.Cleanup();
			Messenger.Default.Unregister(this);
		}

		#endregion

		private Rect _surface = new Rect(-500,-500,1000,700);
		public Rect Surface
		{
			get { return _surface; }
			set
			{
				if (_surface != value)
				{
					_surface = value;
					RaisePropertyChanged("Surface");
				}
			}
		}

		private ITool _tool = new ToolPointer();
		public ITool Tool
		{
			get { return _tool; }
			set
			{
				if (_tool != value)
				{
					_tool = value;
					RaisePropertyChanged("Tool");
				}
			}
		}

		private bool _ShowMousePosition = false;
		public bool ShowMousePosition
		{
			get { return _ShowMousePosition; }
			set
			{
				if (_ShowMousePosition != value)
				{
					_ShowMousePosition = value;
					RaisePropertyChanged("ShowMousePosition");
				}
			}
		}

		private bool _ShowGuides = false;
		public bool ShowGuides
		{
			get { return _ShowGuides; }
			set
			{
				if (_ShowGuides != value)
				{
					_ShowGuides = value;
					RaisePropertyChanged("ShowGuides");
				}
			}
		}

		private bool _IsProtected = false;
		public bool IsProtected
		{
			get { return _IsProtected; }
			set
			{
				if (_IsProtected != value)
				{
					_IsProtected = value;
					RaisePropertyChanged("IsProtected");
				}
			}
		}

		private bool _IsSnapping = false;
		public bool IsSnapping
		{
			get { return _IsSnapping; }
			set
			{
				if (_IsSnapping != value)
				{
					_IsSnapping = value;
					RaisePropertyChanged("IsSnapping");
				}
			}
		}

		private Point _CurrentPosition;
		public Point CurrentPosition
		{
			get { return _CurrentPosition; }
			set
			{
				if (_CurrentPosition != value)
				{
					_CurrentPosition = value;
					RaisePropertyChanged("CurrentPosition");
				}
			}
		}

		//private PlanInfo _info = new PlanInfo();
		//public PlanInfo PlanInformations
		//{
		//	get { return _info; }
		//	set
		//	{
		//		if (_info != value)
		//		{
		//			_info = value;
		//			RaisePropertyChanged("PlanInformations");
		//		}
		//	}
		//}

		private ObservableCollection<LayerItemViewModel> _Layers = new ObservableCollection<LayerItemViewModel>();
		public ICollectionView Layers
		{
			get { return CollectionViewSource.GetDefaultView(_Layers); }
			//set
			//{
			//	if (value != null)
			//	{
			//		_Layers = new ObservableCollection<LayerItem>(value.SourceCollection.Cast<LayerItemViewModel>());
			//		RaisePropertyChanged("Layers");
			//	}
			//}
		}

		new public bool IsVisible
		{
			get { return base.IsVisible; }
			set
			{
				if (base.IsVisible != value)
				{
					base.IsVisible = value;
					
					if( _Layers.Count <= 0)
						LayerCommand.Execute("New");
				}
			}
		}
		
		#region ----------------COMMANDS----------------

		#region tool command
		private ICommand toolCommand;
		public ICommand ToolCommand
		{
			get
			{
				if (toolCommand == null)
					toolCommand = new RelayCommand<string>(ExecuteToolCommand, CanExecuteToolCommand);
				return toolCommand;
			}
		}

		private void ExecuteToolCommand(string param)
		{
			if (param.Equals("Select"))
				Tool = new ToolPointer();

			if (param.Equals("Rectangle"))
				Tool = new ToolRectangle();

			if (param.Equals("Line"))
				Tool = new ToolLine();

			if (param.Equals("Polyline"))
				Tool = new ToolPolyLine();
		}

		private bool CanExecuteToolCommand(string param)
		{
			return true;
		}

		#endregion

		#region layer command
		private ICommand layerCommand;
		public ICommand LayerCommand
		{
			get
			{
				if (layerCommand == null)
					layerCommand = new RelayCommand<string>(ExecuteLayerCommand, CanExecuteLayerCommand);
				return layerCommand;
			}
		}

		private void ExecuteLayerCommand(string param)
		{
			if (param.Equals("New"))
			{
				LayerItemViewModel item = new LayerItemViewModel(
					new LayerItem() { Identifier = _Layers.Count > 0 ? _Layers.Max(p => p.Data.Identifier) + 1 : 1 ,
					Name = _Layers.Count > 0 ? "New": "Default" });
				_Layers.Add(item);
	
				Messenger.Default.Send<NotificationMessage<LayerItemViewModel>>(
						new NotificationMessage<LayerItemViewModel>(item, ViewModelActions.Added),
						ViewModelMessages.UpdateLayers);
			}
			if (param.Equals("Delete"))
			{
				Messenger.Default.Send<NotificationMessage<LayerItemViewModel>>(
						new NotificationMessage<LayerItemViewModel>(Layers.CurrentItem as LayerItemViewModel, ViewModelActions.Removed),
						ViewModelMessages.UpdateLayers);

				_Layers.Remove(Layers.CurrentItem as LayerItemViewModel);
			}
		}

		private bool CanExecuteLayerCommand(string param)
		{
			return true;
		}

		#endregion

		#endregion
	}
}
