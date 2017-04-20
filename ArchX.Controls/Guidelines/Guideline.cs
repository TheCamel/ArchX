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
	public abstract class Guideline
	{
		public Ruler Container { get; internal set; }

		public Cursor Cursor { get; set; }

		public bool IsDisplayed
		{
			get
			{
				if (!Info.IsVisible) return false;

				if (!Container.IsVisible) return false;

				//if( ((CRuler*)m_pRuler)->m_pRulerInfo->bDrawLineWithLayer )
				//	if( !((CRuler*)m_pRuler)->Layer_IsVisible(m_LineInfo.dOwnLayer) )
				//		return false;

				return true;
			}
		}

		protected Pen AnchorPen = new Pen(new SolidColorBrush(Colors.Blue), 4.0);
		protected double PixelPosX;
		protected double PixelPosY;

		public GuideInfo Info { get; internal set; }
	
		public Guideline(Ruler container)
		{
			Info = new GuideInfo() { IsLocked = false, IsVisible = true, IsMoving = false, IsSnap = true, LayerID = 1 };
			Container = container;
		}

		virtual public void Render(DrawingContext drawingContext, Pen renderPen, FrameworkElement parent)
		{
		}

		virtual public void RenderAnchor(DrawingContext drawingContext, FrameworkElement parent)
		{
		}

		virtual public void SetPixel(Point pt)
		{
		}

		virtual public void SetReal(Vector vt)
		{
		}

		public bool HitTest(Point pt)
		{
			return GetHitRect().Contains(pt);
		}

		virtual public Rect GetHitRect()
		{
			return new Rect(0, 0, 0, 0);
		}

		virtual public bool IsOnGuide(ref Vector realVector, double delta)
		{
			return false;
		}
		virtual public bool IsOnGuide(Point point, double delta)
		{
			return false;
		}
		virtual public void GetNearestPos(ref Point point, double delta)
		{}
		virtual public void GetNearestPos(ref Vector realVector, double delta)
		{}
		virtual public void LogicToDot()
		{}

		virtual public double GetXfor(double Y)	{ return Info.RealPositionX; }
		virtual public double GetYfor(double X)	{ return Info.RealPositionY; }
	}
}
