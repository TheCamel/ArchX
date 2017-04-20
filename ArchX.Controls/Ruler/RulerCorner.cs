using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ArchX.Controls.Guidelines;

namespace ArchX.Controls
{
	public class RulerCorner : FrameworkElement
	{
		#region Constructor

		static RulerCorner()
		{
		}

		public RulerCorner()
		{
			this.MouseLeftButtonDown += Ruler_MouseLeftButtonDown;
			this.MouseLeftButtonUp+= Ruler_MouseLeftButtonUp;
			this.MouseMove += Ruler_MouseMove;
		}


		#region -------------properties--------------

		public Ruler Container { get { return TemplatedParent as Ruler; } }

		protected readonly SolidColorBrush normalBack = new SolidColorBrush(Colors.White);
		protected readonly SolidColorBrush disableBack = new SolidColorBrush(Colors.LightGray);
		protected readonly Pen BorderPen = new Pen(Brushes.Gray, 0.5);

		protected Guideline _localGuide;

		#endregion

		protected override void OnRender(DrawingContext drawingContext)
		{
			if( Container.IsProtected)
				drawingContext.DrawRectangle(disableBack, null, new Rect(new Point(0.0, 0.0), new Point(ActualWidth, ActualHeight)));
			else
				drawingContext.DrawRectangle(normalBack, null, new Rect(new Point(0.0, 0.0), new Point(ActualWidth, ActualHeight)));
		}

		void Ruler_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (!Container.IsProtected)
			{
				Point p = Container.GetRelativPosition(e);
				CaptureMouse();
				_localGuide = Container.CreateGuide(p, RulerOrientation.Diagonal);
			}
		}

		void Ruler_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (_localGuide != null && !Container.IsProtected)
			{
				Point p = Container.GetRelativPosition(e);
				Container.MoveGuide(_localGuide, p);
			}
		}

		void Ruler_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (_localGuide != null && !Container.IsProtected)
			{
				Point p = Container.GetRelativPosition(e);
				ReleaseMouseCapture();
				Container.FinalizeGuide(_localGuide, p);
				_localGuide = null;
			}
		}
		#endregion

	}
}
