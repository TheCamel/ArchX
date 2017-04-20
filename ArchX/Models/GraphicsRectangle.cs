using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArchX.Models
{
	/// <summary>
	///  Rectangle graphics object.
	/// </summary>
	public class GraphicsRectangle : GraphicsBase
	{
		#region Constructors

		public GraphicsRectangle(double left, double top, double right, double bottom,
			double lineWidth, Color objectColor, double actualScale)
		{
			Normalize(ref left, ref top, ref right, ref bottom);
			this.TopLeftX = left;
			this.TopLeftY = top;
			this.BottomRightX = right;
			this.BottomRightY = bottom;

			this.graphicsLineWidth = lineWidth;
			this.graphicsObjectColor = objectColor;
			this.graphicsActualScale = actualScale;

			//RefreshDrawng();
		}

		public GraphicsRectangle()
			:
			this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0)
		{
		}

		#endregion

		private double _TopLeftX;
		public double TopLeftX { get { return _TopLeftX; } set { _TopLeftX = value; } }

		private double _TopLeftY;
		public double TopLeftY { get { return _TopLeftY; } set { _TopLeftY = value; } }

		private double _BottomRightX;
		public double BottomRightX { get { return _BottomRightX; } set { _BottomRightX = value; } }

		private double _BottomRightY;
		public double BottomRightY { get { return _BottomRightY; } set { _BottomRightY = value; } }

		public Rect Rectangle { get { return new Rect(_TopLeftX, _TopLeftY, _BottomRightX - _TopLeftX, _BottomRightY - _TopLeftY); } }

		#region Overrides

		/// <summary>
		/// Draw object
		/// </summary>
		public override void Draw(DrawingContext drawingContext)
		{
			if (drawingContext == null)
			{
				throw new ArgumentNullException("drawingContext");
			}

			drawingContext.DrawRectangle(
				null,
				new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth),
				Rectangle);

			base.Draw(drawingContext);
		}

		/// <summary>
		/// Serialization support
		/// </summary>
		public override PropertiesGraphicsBase CreateSerializedObject()
		{
			return new PropertiesGraphicsRectangle(this);
		}

		override public bool HasDelta()
		{
			return _TopLeftX + 1 < _BottomRightX || _TopLeftY + 1 < _BottomRightY;
		}

		/// <summary>
		/// Move handle to the point
		/// </summary>
		override public void MoveHandle(Point point, int handleNumber)
		{
			switch (handleNumber)
			{
				case 1:
					_TopLeftX = point.X;
					_TopLeftY = point.Y;
					break;
				case 2:
					_TopLeftY = point.Y;
					break;
				case 3:
					_BottomRightX = point.X;
					_TopLeftY = point.Y;
					break;
				case 4:
					_BottomRightX = point.X;
					break;
				case 5:
					_BottomRightX = point.X;
					_BottomRightY = point.Y;
					break;
				case 6:
					_BottomRightY = point.Y;
					break;
				case 7:
					_TopLeftX = point.X;
					_BottomRightY = point.Y;
					break;
				case 8:
					_TopLeftX = point.X;
					break;
			}

			Normalize();
			UpdateFrame();
			RefreshDrawing();
		}

		override public void MoveBy(double deltaX, double deltaY)
		{
			_BottomRightX += deltaX;
			_BottomRightY += deltaY;
			_TopLeftX += deltaX;
			_TopLeftY += deltaY;

			UpdateFrame();
			RefreshDrawing();
		}

		override public void MoveMouse(Point point)
		{
			_BottomRightX = point.X;
			_BottomRightY = point.Y;

			Normalize();
			UpdateFrame();
			RefreshDrawing();
		}

		override public void Normalize()
		{
			//base.Normalize(ref _frameLeft, ref _frameTop, ref _frameRight, ref _frameBottom);
			Normalize(ref _TopLeftX, ref _TopLeftY, ref _BottomRightX, ref _BottomRightY);
		}

		override public void UpdateFrame()
		{
			base.SetFrame(_TopLeftX, _TopLeftY, _BottomRightX, _BottomRightY);
		}
		#endregion

	}
}
