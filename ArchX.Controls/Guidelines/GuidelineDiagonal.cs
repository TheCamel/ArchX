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
	public class GuidelineDiagonal : Guideline
	{
		public GuidelineDiagonal(Ruler container)
			: base(container)
		{
			Info.Orientation = GuideOrientation.Diagonal;
			base.Cursor = Cursors.SizeNWSE;
		}

		override public void Render(DrawingContext drawingContext, Pen renderPen, FrameworkElement parent)
		{
			CalculateEquation();

			GetMinMax(ref _TopLeft, ref _BottomRight);

			drawingContext.DrawLine(renderPen, _TopLeft, _BottomRight);
		}
		override public void RenderAnchor(DrawingContext drawingContext, FrameworkElement parent)
		{
		}

		override public void SetPixel(Point pt)
		{
			PixelPosX = pt.X;
			PixelPosY = pt.Y;

			Container.PageManager.XDotToLogic(PixelPosX, ref Info.RealPositionX);
			Container.PageManager.YDotToLogic(PixelPosY, ref Info.RealPositionY);

			CalculateEquation();
		}

		override public void SetReal(Vector vt)
		{
			Info.RealPositionX = vt.X;
			Info.RealPositionY = vt.Y;
			Container.PageManager.XLogicToDot(ref PixelPosX, Info.RealPositionX);
			Container.PageManager.YLogicToDot(ref PixelPosY, Info.RealPositionY);

			CalculateEquation();
		}

		public void SetAngle(Point point)
		{
			Vector temp = new Vector();

			Container.PageManager.DotToLogic(point, ref temp, Info.Orientation);

			temp.X -= Info.RealPositionX;
			temp.Y -= Info.RealPositionY;

			if ((temp.X != 0.0))//&& (tgeVectTmp.dY != 0.0)
			{
				Info.Angle = RadianToDegree(Math.Atan2(temp.Y, temp.X));
			}
			else
			{
				if (temp.X == 0.0)
					Info.Angle = 90.0;
				else
					Info.Angle = 0.0;
			}

			Info.Angle = (int)((Info.Angle * 1000) / 1000.0);

			CalculateEquation();
		}

		public void SetAngle(ref Vector realVector)
		{
			Vector temp = new Vector();

			temp.X = realVector.X - Info.RealPositionX;
			temp.Y = realVector.Y - Info.RealPositionY;

			if ((temp.X != 0.0))//&&(tgeVectTmp.dY != 0.0)
			{
				Info.Angle = RadianToDegree(Math.Atan2(temp.Y, temp.X));
			}
			else
			{
				if (temp.X == 0.0)
					Info.Angle = 90.0;
				else
					Info.Angle = 0.0;
			}

			Info.Angle = (int)((Info.Angle * 1000) / 1000.0);

			CalculateEquation();
		}

		public Rect GetHitRectReference()
		{
			Container.PageManager.XLogicToDot(ref PixelPosX, Info.RealPositionX);
			Container.PageManager.YLogicToDot(ref PixelPosY, Info.RealPositionY);

			Rect rcRef = new Rect(new Point(PixelPosX - 4, PixelPosY - 4),
				new Point(PixelPosX + 4, PixelPosY + 4));

			return rcRef;
		}

		internal Point _TopLeft, _BottomRight;
		private double _coefA, _coefB, _coefC;

		public double CoefC
		{
			get { return _coefC; }
			set { _coefC = value; }
		}

		public double CoefB
		{
			get { return _coefB; }
			set { _coefB = value; }
		}

		public double CoefA
		{
			get { return _coefA; }
			set { _coefA = value; }
		}
		double _OriginX, _ExtendX;
		double _OriginY, _ExtendY;

		private void GetMinMax(ref Point topleft, ref Point bottomright)
		{
			Vector temp = new Vector();

			Container.PageManager.GetPageX(ref _OriginX, ref _ExtendX);
			Container.PageManager.GetPageY(ref _OriginY, ref _ExtendY);

			if (_coefA != 0)
			{
				temp.X = _OriginX;
				temp.Y = GetYfor(_OriginX);
				Container.PageManager.LogicToDot(ref topleft, temp, GuideOrientation.Diagonal);

				temp.X = _OriginX + _ExtendX;
				temp.Y = GetYfor(temp.X);
				Container.PageManager.LogicToDot(ref bottomright, temp, GuideOrientation.Diagonal);
			}
			else
			{
				if ((Info.Angle == 180.0) || (Info.Angle == 0.0))
				{
					temp.X = _OriginX;
					temp.Y = GetYfor(_OriginX);
					Container.PageManager.LogicToDot(ref topleft, temp, GuideOrientation.Diagonal);

					temp.X = _OriginX + _ExtendX;
					temp.Y = GetYfor(bottomright.X);
					Container.PageManager.LogicToDot(ref bottomright, temp, GuideOrientation.Diagonal);
				}
				else
					if ((Info.Angle == 90.0) || (Info.Angle == -90.0))
					{
						temp.Y = _OriginY;
						temp.X = GetXfor(_OriginY);
						Container.PageManager.LogicToDot(ref topleft, temp, GuideOrientation.Diagonal);

						temp.Y = _OriginY - _ExtendY;
						temp.X = GetXfor(bottomright.Y);
						Container.PageManager.LogicToDot(ref bottomright, temp, GuideOrientation.Diagonal);
					}
			}
		}

		override public double GetXfor(double Y)
		{
			if ((_coefA < 1.0E-15) && (_coefA > -1.0E-15))
				return (_coefB);
			else
				return (Y - _coefB) / _coefA;
		}

		override public double GetYfor(double X)
		{
			return _coefA * X + _coefB;
		}


		private void CalculateEquation()
		{
			double x, y;
			x = Math.Cos(DegreeToRadian(Info.Angle));	//calcul DY/DX
			y = Math.Sin(DegreeToRadian(Info.Angle));

			if ((y < 1.0E-15) && (y > -1.0E-15))
				y = 0.0;

			if ((x < 1.0E-15) && (x > -1.0E-15))
				_coefA = 0.0;
			else
				_coefA = y/x;

			if ((_coefA < 1.0E-15) && (_coefA > -1.0E-15))
				_coefA = 0.0;

			_coefB = Info.RealPositionY - _coefA * Info.RealPositionX;
			_coefC = -_coefA * Info.RealPositionX - _coefB * Info.RealPositionY;

			if ((Info.Angle == 180.0) || (Info.Angle == 0.0))
				_coefB = Info.RealPositionY;

			if ((Info.Angle == 90.0) || (Info.Angle == -90.0))
				_coefB = Info.RealPositionX;
		}

		override public void GetNearestPos(ref Point point, double delta)
		{
			GetMinMax(ref _TopLeft, ref _BottomRight);

			double A1 = 0.0; double B1 = 0.0; double C1 = 0.0;
			double A2 = 0.0; double B2 = 0.0; double C2 = 0.0;

			if (CalculCoeff(_TopLeft, _BottomRight, ref A1, ref B1, ref C1))
			{
				// Orthogonal
				A2 = -B1; B2 = A1;
				C2 = -A2 * point.X - B2 * point.Y;

				//La position de l'intersection
				if ((Info.Angle == 90.0) || (Info.Angle == -90.0))
					point.X = ((B1 * C2 - B2 * C1) / (A1 * B2 - A2 * B1));
				else
				{
					point.X = ((B1 * C2 - B2 * C1) / (A1 * B2 - A2 * B1));
					point.Y = ((C1 * A2 - C2 * A1) / (A1 * B2 - A2 * B1));
				}
			}
		}

		override public void GetNearestPos(ref Vector realVector, double delta)
		{
			double A1 = 0.0; double B1 = 0.0; double C1 = 0.0;
			double A2 = 0.0; double B2 = 0.0; double C2 = 0.0;

			A1 = _coefA; B1 = -1.0; C1 = _coefB;

			// Orthogonal
			A2 = -B1; B2 = A1;
			C2 = -A2 * realVector.X - B2 * realVector.Y;

			//La position de l'intersection
			if ((Info.Angle == 90.0) || (Info.Angle == -90.0))
				realVector.X = ((C1 * A2 - C2 * A1) / (A1 * B2 - A2 * B1));
			else
			{
				realVector.X = ((B1 * C2 - B2 * C1) / (A1 * B2 - A2 * B1));
				realVector.Y = ((C1 * A2 - C2 * A1) / (A1 * B2 - A2 * B1));
			}
		}

		bool CalculCoeff(Point P1, Point P2, ref double A, ref double B, ref double C)
		{
			double det = (P1.X * P2.Y - P1.Y * P2.X);
			if ((det < 1.0E-15) && (det > -1.0E-15))
			{
				if ((P1.Y == 0.0) && (P2.Y == 0.0))
				{
					A = 0; B = 1; C = 0;
					return true;
				}
				else
				{
					if ((P1.X == 0.0) && (P2.X == 0.0))
					{
						A = 1; B = 0; C = 0;
						return true;
					}
					else return false;
				}
			}
			else
			{
				A = ((P1.Y - P2.Y) / det);
				B = ((P2.X - P1.X) / det);
				C = (-(A * P1.X + B * P1.Y));
				return true;
			}
		}

		override public bool IsOnGuide(ref Vector realVector, double delta)
		{
			Vector tgeNearVect = new Vector(), tgeVectTmp = new Vector();
			tgeNearVect.X = realVector.X;
			tgeNearVect.Y = realVector.Y;
			GetNearestPos(ref tgeNearVect, delta);

			double dDelta = 0;
			Container.PageManager.XDotToLogicLength((int)delta, ref dDelta);
			delta = dDelta;

			double d;
			d = (tgeNearVect.X - tgeVectTmp.X) * (tgeNearVect.X - tgeVectTmp.X) +
				(tgeNearVect.Y - tgeVectTmp.Y) * (tgeNearVect.Y - tgeVectTmp.Y);
			d = Math.Sqrt(d);

			if (d <= delta)
				return true;

			if (_coefA > 0)
			{
				if ((_coefA * realVector.X + _coefB <= realVector.Y + delta) &&
					(_coefA * realVector.X + _coefB >= realVector.Y - delta) &&
					((realVector.Y - _coefB) / _coefA >= realVector.X - delta) &&
					((realVector.Y - _coefB) / _coefA <= realVector.X + delta))
					return true;
			}
			else
			{
				if ((Info.Angle == 180.0) || (Info.Angle == 0.0))
				{
					if ((_coefB <= realVector.Y + delta) &&
						(_coefB >= realVector.Y - delta))
						return true;
				}
				else
				{
					if ((Info.Angle == 90.0) || (Info.Angle == -90.0))
						if ((_coefB >= realVector.X - delta) &&
							(_coefB <= realVector.X + delta))
							return true;
				}
			}

			return false;
		}

		//------------------------------------------------------------------

		override public bool IsOnGuide(Point point, double delta)
		{
			Vector tgeNearVect = new Vector(), tgeVectTmp = new Vector();
			Container.PageManager.DotToLogic(point, ref tgeVectTmp, Info.Orientation);

			tgeNearVect.X = tgeVectTmp.X;
			tgeNearVect.Y = tgeVectTmp.Y;
			GetNearestPos(ref tgeNearVect, delta);

			double dDelta = 0;
			Container.PageManager.XDotToLogicLength((int)delta, ref dDelta);
			delta = dDelta;

			double d;
			d = (tgeNearVect.X - tgeVectTmp.X) * (tgeNearVect.X - tgeVectTmp.X) +
				(tgeNearVect.Y - tgeVectTmp.Y) * (tgeNearVect.Y - tgeVectTmp.Y);
			d = Math.Sqrt(d);

			if (d <= delta)
				return true;

			if (_coefA > 0)
			{
				if ((_coefA * tgeVectTmp.X + _coefB <= tgeVectTmp.Y + delta) &&
					(_coefA * tgeVectTmp.X + _coefB >= tgeVectTmp.Y - delta) &&
					((tgeVectTmp.Y - _coefB) / _coefA >= tgeVectTmp.X - delta) &&
					((tgeVectTmp.Y - _coefB) / _coefA <= tgeVectTmp.X + delta))
					return true;
			}
			else
			{
				if ((Info.Angle == 180.0) || (Info.Angle == 0.0))
				{
					if ((_coefB <= tgeVectTmp.Y + delta) &&
						(_coefB >= tgeVectTmp.Y - delta))
						return true;
				}
				else
				{
					if ((Info.Angle == 90.0) || (Info.Angle == -90.0))
						if ((_coefB >= tgeVectTmp.X - delta) &&
							(_coefB <= tgeVectTmp.X + delta))
							return true;
				}
			}

			Rect rct;
			rct = GetHitRectReference();		//Hit the reference position
			if (rct.Contains(point))
				return true;

			return false;
		}

		private double RadianToDegree(double angle)
		{
			return angle * (180.0 / Math.PI);
		}

		private double DegreeToRadian(double angle)
		{
			return Math.PI * angle / 180.0;
		}
	}
}
