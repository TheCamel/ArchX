using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ArchX.Controls
{
	public class MousePositionAdorner : Adorner
	{
		// Be sure to call the base class constructor. 
		public MousePositionAdorner(Ruler parent, UIElement adornedElement): base(adornedElement)
		{
			IsHitTestVisible = false;
			RulerParent = parent;
		}

		private Ruler RulerParent { get; set; }
		private Pen _RenderPen = new Pen(new SolidColorBrush(Colors.Black), 0.4);

		// A common way to implement an adorner's rendering behavior is to override the OnRender 
		// method, which is called by the layout system as part of a rendering pass. 
		protected override void OnRender(DrawingContext drawingContext)
		{
			if (RulerParent.CurrentPosition.X > 0 && RulerParent.CurrentPosition.Y > 0
				&& RulerParent.ShowMousePosition)
			{
				drawingContext.DrawLine(_RenderPen, new Point(RulerParent.CurrentPosition.X, 0),
					new Point(RulerParent.CurrentPosition.X, RulerParent.ActualHeight));

				drawingContext.DrawLine(_RenderPen, new Point(0, RulerParent.CurrentPosition.Y),
					new Point(RulerParent.ActualWidth, RulerParent.CurrentPosition.Y));
			}

			if (RulerParent.CurrentSnapPosition.X > 0 && RulerParent.CurrentSnapPosition.Y > 0
				&& RulerParent.IsSnapping)
			{
				drawingContext.DrawEllipse(null, _RenderPen, 
					new Point(RulerParent.CurrentSnapPosition.X, RulerParent.CurrentSnapPosition.Y), 10, 10 );
			}
		}
	}
}
