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
	public class GuidelineVertical : Guideline
	{
		public GuidelineVertical(Ruler container) : base(container)
		{
			Info.Orientation = GuideOrientation.Vertical;
			base.Cursor = Cursors.SizeWE;
		}

		override public void Render(DrawingContext drawingContext, Pen renderPen, FrameworkElement parent)
		{
			Container.PageManager.XLogicToDot(ref PixelPosX, Info.RealPositionX);
			drawingContext.DrawLine(renderPen, new Point(PixelPosX, 0), new Point(PixelPosX, parent.ActualHeight));
		}

		override public void RenderAnchor(DrawingContext drawingContext, FrameworkElement parent)
		{
			Container.PageManager.XLogicToDot(ref PixelPosX, Info.RealPositionX);
			drawingContext.DrawLine(AnchorPen, new Point(PixelPosX, 0), new Point(PixelPosX, 10));
		}

		override public void SetPixel(Point pt)
		{
			PixelPosX = pt.X;
			Container.PageManager.XDotToLogic(PixelPosX, ref Info.RealPositionX);
		}

		override public void SetReal(Vector vt)
		{
			Info.RealPositionX = vt.X;
			Container.PageManager.XLogicToDot(ref PixelPosX, Info.RealPositionX);
		}

		override public Rect GetHitRect()
		{
			return new Rect( new Point( PixelPosX-2, PixelPosY ), new Size(4,10));
		}

		override public bool IsOnGuide(ref Vector realVector, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;

			Container.PageManager.XDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionX;
			dMin -= dDelta;
			dMax += dDelta;

			if ((realVector.X > dMin) && (realVector.X < dMax))
				return true;

			return false;
		}

		override public bool IsOnGuide(Point point, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;
			Vector tempReal = new Vector();

			Container.PageManager.XDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionX;
			dMin -= dDelta;
			dMax += dDelta;

			Container.PageManager.DotToLogic(point, ref tempReal, Info.Orientation);

			if ((tempReal.X > dMin) && (tempReal.X < dMax))
				return true;

			return false;
		}


		override public void GetNearestPos(ref Point point, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;
			Vector tempReal = new Vector();

			Container.PageManager.XDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionX;
			dMin -= dDelta;
			dMax += dDelta;

			Container.PageManager.DotToLogic(point, ref tempReal, Info.Orientation);

			if ((tempReal.X > dMin) && (tempReal.X < dMax))
				point.X = PixelPosX;
		}


		override public void GetNearestPos(ref Vector realVector, double delta)
		{
			double dMin, dMax;
			double dDelta = 0;

			Container.PageManager.XDotToLogicLength(delta, ref dDelta);

			dMin = dMax = Info.RealPositionX;
			dMin -= dDelta;
			dMax += dDelta;

			if ((realVector.X > dMin) && (realVector.X < dMax))
				realVector.X = Info.RealPositionX;
		}

		override public void LogicToDot()
		{
			Container.PageManager.XLogicToDot(ref PixelPosX, Info.RealPositionX);
		}
	}
}
