using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArchX.Tools;
using ArchX.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace ArchX.Views
{
	/// <summary>
	/// Interaction logic for PlanView.xaml
	/// </summary>
	public partial class PlanView : UserControl
	{
		public PlanView()
		{
			InitializeComponent();

			Messenger.Default.Register<NotificationMessage<LayerItemViewModel>>(this, ViewModelMessages.UpdateLayers,
				(o) =>
				{
					if( o.Notification == ViewModelActions.Added)
						ctrlRuler.LayerAdd(o.Content.Data);
					if( o.Notification == ViewModelActions.Removed)
						ctrlRuler.LayerRemove(o.Content.Data);
					if (o.Notification == ViewModelActions.Selected)
						ctrlRuler.LayerSelect(o.Content.Data);
					if (o.Notification == ViewModelActions.Modified)
						ctrlRuler.LayerModify(o.Content.Data);
				});

			this.IsVisibleChanged += PlanView_IsVisibleChanged;
			this.Unloaded += PlanView_Unloaded;
		}

		void PlanView_Unloaded(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Unregister(this);
		}

	
		void PlanView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			(this.DataContext as PlanViewModel).IsVisible = (bool)e.NewValue;
		}
	}
}
