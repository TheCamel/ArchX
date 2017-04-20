using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ArchX.Controls;

namespace ArchX.Components.Controls
{
	public class PlanAdorner : Adorner
	{
		// Be sure to call the base class constructor. 
		public PlanAdorner(UIElement adornedElement)
			: base(adornedElement)
		{
			IsHitTestVisible = false;
		}

		private Pen _RenderPen = new Pen(new SolidColorBrush(Colors.Red), 0.5) { DashStyle = DashStyles.DashDotDot };
		private Pen _LineRenderPen = new Pen(new SolidColorBrush(Colors.Red), 0.5) { DashStyle = DashStyles.DashDotDot };

		// A common way to implement an adorner's rendering behavior is to override the OnRender 
		// method, which is called by the layout system as part of a rendering pass. 
		protected override void OnRender(DrawingContext drawingContext)
		{
			if( System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
				return;

			DrawingCanvas fz = base.AdornedElement as DrawingCanvas;
			PagingManager pm = fz.CurrentRuler.PageManager;

			double originX = 0, originY = 0, widthX = 0, heightY = 0;

			pm.XLogicToDot(ref originX, 0);
			pm.YLogicToDot(ref originY, 0);
			drawingContext.DrawLine(_LineRenderPen, new Point(originX, 0), new Point(originX , fz.ActualHeight));
			drawingContext.DrawLine(_LineRenderPen, new Point(0, originY), new Point(fz.ActualWidth, originY));

			pm.XLogicToDot(ref originX, -100);
			pm.YLogicToDot(ref originY, -100);
			pm.XLogicToDot(ref widthX, 100);
			pm.YLogicToDot(ref heightY, 100);
			drawingContext.DrawRectangle(null, _RenderPen, new Rect(new Point(originX, originY), new Point(widthX, heightY)));
		}
	}
}
