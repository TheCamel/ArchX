﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace ArchX.ViewModels
{
	public abstract class PaneViewModel : ViewModelBaseExtended
	{
		#region -----------------CONSTRUCTOR-----------------

		protected PaneViewModel()
		{
		}

		#endregion

		#region -----------------PROPERTIES-----------------

		#region Icon

		private string _Icon;
		public string Icon
		{
			get { return _Icon; }
			set
			{
				if (_Icon != value)
				{
					_Icon = value;
					RaisePropertyChanged("Icon");
				}
			}
		}
		#endregion

		#region ContentId

		private string _contentId = null;
		public string ContentId
		{
			get { return _contentId; }
			set
			{
				if (_contentId != value)
				{
					_contentId = value;
					RaisePropertyChanged("ContentId");
				}
			}
		}
		#endregion

		#region IsSelected
		private bool _isSelected = false;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					RaisePropertyChanged("IsSelected");
				}
			}
		}
		#endregion

		#region IsActive
		private bool _isActive = false;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					RaisePropertyChanged("IsActive");
				}
			}

		}
		#endregion

		#region IsVisible

		private bool _isVisible;
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				if (_isVisible != value)
				{
					_isVisible = value;
					RaisePropertyChanged("IsVisible");
				}
			}
		}

		#endregion

		#endregion
    }
}
