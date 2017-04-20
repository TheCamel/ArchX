using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;
using ArchX.Controls.Guidelines;

namespace ArchX.Controls
{
	public class GuideAnchorAdorner : Adorner
	{
		public GuideAnchorAdorner(Ruler parent, UIElement adornedElement)
			: base(adornedElement)
		{
			IsHitTestVisible = false;
			RulerParent = parent;
		}

		private Ruler RulerParent { get; set; }

		// A common way to implement an adorner's rendering behavior is to override the OnRender 
		// method, which is called by the layout system as part of a rendering pass. 
		protected override void OnRender(DrawingContext drawingContext)
		{
			RulerBar bar = this.AdornedElement as RulerBar;

			foreach (Guideline guide in RulerParent.GuideManager.Guides.Where(p => (int)p.Info.Orientation == (int)bar.Orientation))
			{
				guide.RenderAnchor( drawingContext, RulerParent);
			}
		}
	}
}
