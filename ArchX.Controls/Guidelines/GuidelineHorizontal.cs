using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ArchX.Controls.Guidelines
{
	public class GuidelineHorizontal : Guideline
	{
		public GuidelineHorizontal(Ruler container)
			: base(container)
		{
			Info.Orientation = GuideOrientation.Horizontal;
			base.Cursor = Cursors.SizeNS;
		}

		override public void Render(DrawingContext drawingContext, Pen renderPen, FrameworkElement parent)
		{
			Container.PageManager.YLogicToDot(ref PixelPosY, Info.RealPositionY);
			drawingContext.DrawLine(renderPen, new Point(0, PixelPosY), new Point(parent.ActualWidth, PixelPosY));
		}
		override public void RenderAnchor(DrawingContext drawingContext, FrameworkElement parent)
		{
			Container.PageManager.YLogicToDot(ref PixelPosY, Info.RealPositionY);
			drawingContext.DrawLine(AnchorPen, new Point(0, PixelPosY), new Point(10, PixelPosY));
		}

		override public void SetPixel(Point pt)
		{
			PixelPosY = pt.Y;
			Container.PageManager.YDotToLogic(PixelPosY, ref Info.RealPositionY);
		}

		override public void SetReal(Vector vt)
		{
			Info.RealPositionY = vt.Y;
			Container.PageManager.YLogicToDot(ref PixelPosY, Info.RealPositionY);
		}

		override public Rect GetHitRect()
		{
			return new Rect(new Point(PixelPosX, PixelPosY-4), new Size(10,4));
		}

		override public bool IsOnGuide(ref Vector realVector, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;

			Container.PageManager.YDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionY;
			dMin -= dDelta;
			dMax += dDelta;

			if ((realVector.Y > dMin) && (realVector.Y < dMax))
				return true;

			return false;
		}

		override public bool IsOnGuide(Point point, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;
			Vector tempReal = new Vector();

			Container.PageManager.YDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionY;
			dMin -= dDelta;
			dMax += dDelta;

			Container.PageManager.DotToLogic(point, ref tempReal, Info.Orientation);

			if ((tempReal.Y > dMin) && (tempReal.Y < dMax))
				return true;

			return false;
		}


		override public void GetNearestPos(ref Point point, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;
			Vector tempReal = new Vector();

			Container.PageManager.YDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionY;
			dMin -= dDelta;
			dMax += dDelta;

			Container.PageManager.DotToLogic(point, ref tempReal, Info.Orientation);

			if ((tempReal.Y > dMin) && (tempReal.Y < dMax))
				point.Y = PixelPosY;
		}


		override public void GetNearestPos(ref Vector realVector, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;

			Container.PageManager.YDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionY;
			dMin -= dDelta;
			dMax += dDelta;

			if ((realVector.Y > dMin) && (realVector.Y < dMax))
				realVector.Y = Info.RealPositionY;
		}

		override public void LogicToDot()
		{
			Container.PageManager.YLogicToDot(ref PixelPosY, Info.RealPositionY);
		}

	}
}
