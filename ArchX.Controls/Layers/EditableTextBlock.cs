using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArchX.Controls.Layers
{
	[TemplatePart(Name = "PART_Text", Type = typeof(TextBlock))]
	[TemplatePart(Name = "PART_Edit", Type = typeof(TextBox))]
	public class EditableTextBlock : Control
	{
		 static EditableTextBlock()
        {
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBlock),
				new FrameworkPropertyMetadata(typeof(EditableTextBlock)));
        }

		#region Content

		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
				"Content", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(""));

		public string Content
		{
			get
			{
				return (string)GetValue(ContentProperty);
			}
			set
			{
				SetValue(ContentProperty, value);
			}
		}

		#endregion

		#region IsInEditMode

		private static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register(
				"IsInEditMode", typeof(bool), typeof(EditableTextBlock),
				new PropertyMetadata(false, new PropertyChangedCallback(OnEditModeChanged)));

		public bool IsInEditMode
		{
			get
			{
				return (bool)GetValue(IsInEditModeProperty);
			}
			set
			{
				SetValue(IsInEditModeProperty, value);
			}
		}

		private static void OnEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			EditableTextBlock element = d as EditableTextBlock;
			element.AfterOnEditModeChanged((bool)e.NewValue);
		}

		protected void AfterOnEditModeChanged(bool after)
		{
			if( after )
			{
				_oldText = _Edit.Text;
				_Block.Visibility = System.Windows.Visibility.Collapsed;
				_Edit.Visibility = System.Windows.Visibility.Visible;
				//_Edit.Focus();
				//_Edit.SelectAll();
			}
			else
			{
				_Block.Visibility = System.Windows.Visibility.Visible;
				_Edit.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		#endregion

		private TextBlock _Block = null;
		private TextBox _Edit = null;
		private string _oldText = null;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_Block = (TextBlock)GetTemplateChild("PART_Text");
			_Edit = (TextBox)GetTemplateChild("PART_Edit");

			_Block.MouseLeftButtonDown += _Block_MouseLeftButtonDown;

			_Edit.LostFocus += _Edit_LostFocus;
			_Edit.KeyUp += _Edit_KeyUp;
		}

		void _Edit_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Content = _Edit.Text;
				IsInEditMode = false;
				e.Handled = true;
			}
			if (e.Key == Key.Escape)
			{
				Content = _oldText;
				IsInEditMode = false;
				e.Handled = true;
			}
		}

		void _Edit_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!_Edit.IsFocused && IsInEditMode)
			{
				Content = _Edit.Text;
				IsInEditMode = false;
			}
		}

		void _Block_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				_Edit.Text = Content;
				IsInEditMode = true;
			}
		}

	}
}
