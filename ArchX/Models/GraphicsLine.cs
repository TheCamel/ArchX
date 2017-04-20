using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArchX.Models
{
	/// <summary>
	///  Line graphics object.
	/// </summary>
	public class GraphicsLine : GraphicsBase
	{
		#region Constructors

		public GraphicsLine(Point start, Point end, double lineWidth, Color objectColor, double actualScale)
		{
			this.lineStart = start;
			this.lineEnd = end;
			this.graphicsLineWidth = lineWidth;
			this.graphicsObjectColor = objectColor;
			this.graphicsActualScale = actualScale;

			//RefreshDrawng();
		}

		public GraphicsLine()
			:
			this(new Point(0.0, 0.0), new Point(100.0, 100.0), 1.0, Colors.Black, 1.0)
		{
		}

		#endregion Constructors

		#region Properties

		protected Point lineStart;
		public Point Start
		{
			get { return lineStart; }
			set { lineStart = value; }
		}

		protected Point lineEnd;
		public Point End
		{
			get { return lineEnd; }
			set { lineEnd = value; }
		}

		#endregion Properties

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

			drawingContext.DrawLine(
				new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth),
				lineStart,
				lineEnd);

			base.Draw(drawingContext);
		}

		///// <summary>
		///// Test whether object contains point
		///// </summary>
		//public override bool Contains(Point point)
		//{
		//	LineGeometry g = new LineGeometry(lineStart, lineEnd);

		//	return g.StrokeContains(new Pen(Brushes.Black, LineHitTestWidth), point);
		//}

		/// <summary>
		/// XML serialization support
		/// </summary>
		/// <returns></returns>
		public override PropertiesGraphicsBase CreateSerializedObject()
		{
			return new PropertiesGraphicsLine(this);
		}

		///// <summary>
		///// Hit test.
		///// Return value: -1 - no hit
		/////                0 - hit anywhere
		/////                > 1 - handle number
		///// </summary>
		//public override int MakeHitTest(Point point)
		//{
		//	if (IsSelected)
		//	{
		//		for (int i = 1; i <= HandleCount; i++)
		//		{
		//			if (GetHandleRectangle(i).Contains(point))
		//				return i;
		//		}
		//	}

		//	if (Contains(point))
		//		return 0;

		//	return -1;
		//}

		override public bool HasDelta()
		{
			return lineStart.X + 1 < lineEnd.X || lineStart.Y + 1 < lineEnd.Y;
		}

		/// <summary>
		/// Test whether object intersects with rectangle
		/// </summary>
		public override bool IntersectsWith(Rect rectangle)
		{
			RectangleGeometry rg = new RectangleGeometry(rectangle);

			LineGeometry lg = new LineGeometry(lineStart, lineEnd);
			PathGeometry widen = lg.GetWidenedPathGeometry(new Pen(Brushes.Black, LineHitTestWidth));

			PathGeometry p = Geometry.Combine(rg, widen, GeometryCombineMode.Intersect, null);

			return (!p.IsEmpty());
		}

		///// <summary>
		///// Get cursor for the handle
		///// </summary>
		//public override Cursor GetHandleCursor(int handleNumber)
		//{
		//	switch (handleNumber)
		//	{
		//		case 1:
		//		case 2:
		//			return Cursors.SizeAll;
		//		default:
		//			return Cursors.Arrow;
		//	}
		//}

		/// <summary>
		/// Move handle to new point (resizing)
		/// </summary>
		public override void MoveHandle(Point point, int handleNumber)
		{
			switch (handleNumber)
			{
				case 1:
					lineStart.X = point.X;
					lineStart.Y = point.Y;
					break;
				case 2:
					lineStart.Y = point.Y;
					break;
				case 3:
					lineEnd.X = point.X;
					lineStart.Y = point.Y;
					break;
				case 4:
					lineEnd.X = point.X;
					break;
				case 5:
					lineEnd.X = point.X;
					lineEnd.Y = point.Y;
					break;
				case 6:
					lineEnd.Y = point.Y;
					break;
				case 7:
					lineStart.X = point.X;
					lineEnd.Y = point.Y;
					break;
				case 8:
					lineStart.X = point.X;
					break;
			}

			UpdateFrame();
			RefreshDrawing();
		}

		/// <summary>
		/// Move object
		/// </summary>
		public override void MoveBy(double deltaX, double deltaY)
		{
			lineStart.X += deltaX;
			lineStart.Y += deltaY;

			lineEnd.X += deltaX;
			lineEnd.Y += deltaY;

			UpdateFrame();
			RefreshDrawing();
		}

		override public void MoveMouse(Point point)
		{
			lineEnd.X = point.X;
			lineEnd.Y = point.Y;

			UpdateFrame();
			RefreshDrawing();
		}

		override public void UpdateFrame()
		{
			base.SetFrame(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
		}

		#endregion Overrides
	}
}
