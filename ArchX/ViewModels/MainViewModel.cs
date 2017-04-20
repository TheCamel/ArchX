using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ArchX.Components.Dialogs;
using ArchX.Tools;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace ArchX.ViewModels
{
	public class MainViewModel : DocumentViewModel
	{
		#region ----------------CONSTRUCTOR----------------

		public MainViewModel(string param)
		{
			Messenger.Default.Register<DocumentViewModel>(this, ViewModelMessages.DocumentRequestClose,
				(DocumentViewModel o) =>
				{
					this.Documents.Remove(o);
					o.Cleanup();

					if( this.Documents.Count == 0)
						ActiveDocument = null;
				});

			BackStageIsOpen = false;

			//this.Tools.Add(new LayerPaneViewModel());
		}

		/// <summary>
		/// Child classes can override this method to perform clean-up logic, such as removing event handlers.
		/// </summary>
		override public void Cleanup()
		{
			base.Cleanup();
			Messenger.Default.Unregister(this);
		}

		#endregion

		#region ----------------PROPERTIES----------------

		public string Title
		{
			get
			{
				return "test";
			}
		}

		private bool _backStageIsOpen = false;
		public bool BackStageIsOpen
		{
			get { return _backStageIsOpen; }
			set
			{
				if (_backStageIsOpen != value)
				{
					_backStageIsOpen = value;
					RaisePropertyChanged("BackStageIsOpen");
				}
			}
		}


		private int _backStageIndex = -1;
		public int BackStageIndex
		{
			get { return _backStageIndex; }
			set
			{
				if (_backStageIndex != value)
				{
					_backStageIndex = value;
					RaisePropertyChanged("BackStageIndex");
				}
			}
		}

		private ObservableCollection<DocumentViewModel> _Documents;
		public ObservableCollection<DocumentViewModel> Documents
		{
			get
			{
				if (_Documents == null)
				{
					_Documents = new ObservableCollection<DocumentViewModel>();
				}
				return _Documents;
			}
		}

		private DocumentViewModel _ActiveDocument = null;
		public DocumentViewModel ActiveDocument
		{
			get { return _ActiveDocument; }
			set
			{
				if (_ActiveDocument != value)
				{
					_ActiveDocument = value;
					RaisePropertyChanged("ActiveDocument");
					RaisePropertyChanged("HasDocument");

					//notify the view about activation for tools update
					Messenger.Default.Send<DocumentViewModel>(_ActiveDocument, ViewModelMessages.DocumentActivChanged);
				}
			}
		}

		public bool HasDocument
		{
			get { return _ActiveDocument != null && _ActiveDocument is DocumentViewModel; }
		}

		private ObservableCollection<ToolViewModel> _Tools = new ObservableCollection<ToolViewModel>();
		public ObservableCollection<ToolViewModel> Tools
		{
			get
			{
				return _Tools;
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

		#endregion

		#region ----------------COMMANDS----------------

		#region about command
		private ICommand aboutCommand;
		public ICommand AboutCommand
		{
			get
			{
				if (aboutCommand == null)
					aboutCommand = new RelayCommand(
						delegate()
						{
							AboutView dlg = new AboutView();
							dlg.Owner = Application.Current.MainWindow;
							dlg.ShowDialog();
						},
						delegate() { return true; } );
				return aboutCommand;
			}
		}
		#endregion

		#region exit command
		private ICommand sysExitCommand;
		public ICommand SysExitCommand
		{
			get
			{
				if (sysExitCommand == null)
					sysExitCommand = new RelayCommand(
						delegate() {  Application.Current.MainWindow.Close(); },
						delegate()
						{
							if (Application.Current != null && Application.Current.MainWindow != null)
								return true;
							return false;
						});
				return sysExitCommand;
			}
		}
		#endregion

		#region new doc command
		private ICommand newDocumentCommand;
		public ICommand NewDocumentCommand
		{
			get
			{
				if (newDocumentCommand == null)
					newDocumentCommand = new RelayCommand(ExecuteNewDocumentCommand, CanExecuteNewDocumentCommand);
				return newDocumentCommand;
			}
		}

		private void ExecuteNewDocumentCommand()
		{
			PlanViewModel pvm = new PlanViewModel();
			this.Documents.Add(pvm);
			ActiveDocument = pvm;

			//pvm.LayerCommand.Execute("New");
		}

		private bool CanExecuteNewDocumentCommand()
		{
			return true;
		}

		#endregion

		#endregion
	}
}
