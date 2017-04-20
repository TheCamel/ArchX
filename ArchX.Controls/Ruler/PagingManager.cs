using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ArchX.Controls.Guidelines;

namespace ArchX.Controls
{
	public class PagingManager
	{
		#region --------------CONSTRUCTOR--------------

		public PagingManager()
		{
		}

		#endregion


		#region --------------PROPERTIES--------------

		public FrameworkElement DrawingSurface { get; set; }

		private Point _Resolution = new Point(0, 0);
		public Point Resolution { get { return _Resolution; } set { _Resolution = value; } }

		private Point _Origin = new Point(0, 0);
		public Point Origin { get { return _Origin; } set { _Origin = value; } }

		private Point _Extend = new Point(0, 0);
		public Point Extend { get { return _Extend; } set { _Extend = value; } }


		private double _PixelTemp;

		#endregion

		#region --------------METHODS--------------

		public void SetResolution(double dResX, double dResY)
		{
			_Resolution.X = dResX;
			_Resolution.Y = dResY;
		}

		public void SetPage(double dOrgX, double dOrgY, double dExdX, double dExdY)
		{
			_Origin.X = dOrgX;
			_Origin.Y = dOrgY;
			_Extend.X = dExdX;
			_Extend.Y = dExdY;

			Update();
		}

		public void GetPageX(ref double dOrgX, ref double dExdX)
		{
			dOrgX = _Origin.X;
			dExdX = _Extend.X;
		}

		public void GetPageY(ref double dOrgY, ref double dExdY)
		{
			dOrgY = _Origin.Y;
			dExdY = _Extend.Y;
		}

		bool IsInViewX(double dRealPos)
		{
			if ((dRealPos >= Origin.X) && (dRealPos <= Origin.X + Extend.X))
				return true;
			else
				return false;
		}
		bool IsInViewY(double dRealPos)
		{
			if ((dRealPos >= Origin.Y) && (dRealPos <= Origin.Y + Extend.Y))
				return true;
			else
				return false;
		}
		//bool IsInViewXPix(int nPos)
		//{
		//	if ((nPos >= DrawingSurface.left) && (nPos <= DrawingSurface.right))
		//		return true;
		//	else
		//		return false;
		//}
		//bool IsInViewYPix(int nPos)
		//{
		//	if ((nPos >= DrawingSurface.top) && (nPos <= DrawingSurface.bottom))
		//		return true;
		//	else
		//		return false;
		//}

		#region --------------logic to dot--------------

		public void XLogicToDot(ref double Pixel, double dRealPos)
		{
			if (Resolution.X > 0)
			{
				_PixelTemp = (dRealPos - Origin.X) / Resolution.X;
				Pixel = RoundUp(_PixelTemp);
			}
			else
				Pixel = 0;
		}

		public void YLogicToDot(ref double Pixel, double dRealPos)
		{
			if (Resolution.Y > 0)
			{
				_PixelTemp = (dRealPos - Origin.Y) / Resolution.Y;
				Pixel = RoundUp(_PixelTemp);
			}
			else
				Pixel = 0;
		}

		public void LogicToDot(ref Point point, Vector RealPos, GuideOrientation nTyp)
		{
			if (nTyp == GuideOrientation.Horizontal || nTyp == GuideOrientation.Diagonal)
			{
				if (Resolution.Y > 0)
				{
					_PixelTemp = (RealPos.Y - Origin.Y) / Resolution.Y;
					point.Y = RoundUp(_PixelTemp);
				}
				else
					point.Y = 0;
			}

			if (nTyp == GuideOrientation.Vertical || nTyp == GuideOrientation.Diagonal)
			{
				if (Resolution.X > 0)
				{
					_PixelTemp = (RealPos.X - Origin.X) / Resolution.X;
					point.X = RoundUp(_PixelTemp);
				}
				else
					point.X = 0;
			}
		}

		public void LogicToDot(ref double ptX, ref double ptY, double realPosX, double realPosY, GuideOrientation nTyp)
		{
			if (nTyp == GuideOrientation.Horizontal || nTyp == GuideOrientation.Diagonal)
			{
				if (Resolution.Y > 0)
				{
					_PixelTemp = (realPosY - Origin.Y) / Resolution.Y;
					ptY = RoundUp(_PixelTemp);
				}
				else
					ptY = 0;
			}

			if (nTyp == GuideOrientation.Vertical || nTyp == GuideOrientation.Diagonal)
			{
				if (Resolution.X > 0)
				{
					_PixelTemp = (realPosX - Origin.X) / Resolution.X;
					ptX = RoundUp(_PixelTemp);
				}
				else
					ptX = 0;
			}
		}

		#endregion


		#region --------------dot to logic--------------

		public void XDotToLogic(double Pixel, ref double dRealPos)
		{
			if (Resolution.X > 0)
				dRealPos = ((double)(Pixel)) * Resolution.X + Origin.X;
			else
				dRealPos = Origin.X;
		}

		public void YDotToLogic(double Pixel, ref double dRealPos)
		{
			if (Resolution.Y > 0)
				dRealPos = ((double)(Pixel)) * Resolution.Y + Origin.Y;
			else
				dRealPos = Origin.X;
		}

		public void XDotToLogicLength(double Pixel, ref double dRealPos)
		{
			if (Resolution.X > 0)
				dRealPos = ((double)(Pixel)) * Resolution.X;
			else
				dRealPos = 0;
		}

		public void YDotToLogicLength(double Pixel, ref double dRealPos)
		{
			if (Resolution.Y > 0)
				dRealPos = ((double)(Pixel)) * Resolution.Y;
			else
				dRealPos = 0;
		}

		public void DotToLogic(Point point, ref Vector realPos, GuideOrientation orientation)
		{
			if ((orientation == GuideOrientation.Horizontal) ||
				(orientation == GuideOrientation.Diagonal))
			{
				if (Resolution.Y > 0)
					realPos.Y = ((double)(point.Y)) * Resolution.Y + Origin.Y;
				else
					realPos.Y = Origin.Y;
			}

			if ((orientation == GuideOrientation.Vertical) ||
				(orientation == GuideOrientation.Diagonal))
			{
				if (Resolution.X > 0)
					realPos.X = ((double)(point.X)) * Resolution.X + Origin.X;
				else
					realPos.X = Origin.X;
			}
		}

		public void DotToLogicLength(Point point, ref Vector realPos, GuideOrientation orientation)
		{
			if ((orientation == GuideOrientation.Horizontal) ||
				(orientation == GuideOrientation.Diagonal))
			{

				if (Resolution.Y > 0)
					realPos.Y = ((double)(point.Y)) * Resolution.Y;
				else
					realPos.Y = 0;
			}

			if ((orientation == GuideOrientation.Vertical) ||
				(orientation == GuideOrientation.Diagonal))
			{
				if (Resolution.X > 0)
					realPos.X = ((double)(point.X)) * Resolution.X;
				else
					realPos.X = 0;
			}
		}

		#endregion
		#endregion

		#region --------------INTERNALS--------------

		private void Update()
		{
			//DrawingSurface.RenderSize

			if (DrawingSurface.ActualWidth > 0)
				_Resolution.X = _Extend.X / DrawingSurface.ActualWidth;
			else
				_Resolution.X = _Extend.X;

			if (DrawingSurface.ActualHeight > 0)
				_Resolution.Y = _Extend.Y / DrawingSurface.ActualHeight;
			else
				_Resolution.Y = _Extend.Y;
		}

		private int RoundUp(double dTobeRounded)
		{
			int nRetVal;
			double dTemp;

			dTemp = dTobeRounded - Math.Floor(dTobeRounded);

			if (dTemp >= 0.5)
				nRetVal = (int)Math.Floor(dTobeRounded) + 1;
			else
				nRetVal = (int)Math.Floor(dTobeRounded);

			return nRetVal;
		}

		private int RoundDn(double dTobeRounded)
		{
			int nRetVal;
			double dTemp;

			dTemp = dTobeRounded - Math.Floor(dTobeRounded);

			if (dTemp >= 0.5)
				nRetVal = (int)Math.Floor(dTobeRounded) - 1;
			else
				nRetVal = (int)Math.Floor(dTobeRounded);

			return nRetVal;
		}
		#endregion

	}
}
