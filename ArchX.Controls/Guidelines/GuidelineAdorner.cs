using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ArchX.Controls.Guidelines
{
	public class GuidelineAdorner : Adorner
	{
		// Be sure to call the base class constructor. 
		public GuidelineAdorner(Ruler parent, UIElement adornedElement)
			: base(adornedElement)
		{
			IsHitTestVisible = false;
			_RenderPen.DashStyle = DashStyles.Dot;

			_RulerParent = parent;
		}

		private Ruler _RulerParent = null;
		private Pen _RenderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.0);

		// A common way to implement an adorner's rendering behavior is to override the OnRender 
		// method, which is called by the layout system as part of a rendering pass. 
		protected override void OnRender(DrawingContext drawingContext)
		{
			if (_RulerParent.ShowGuides)
			{
				foreach (Guideline guide in _RulerParent.GuideManager.Guides)
				{
					guide.Render(drawingContext, _RenderPen, this.AdornedElement as FrameworkElement);
				}
			}
		}
	}
}
