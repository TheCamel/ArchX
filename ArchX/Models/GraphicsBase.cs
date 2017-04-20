using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Globalization;

namespace ArchX.Models
{
	/// <summary>
	/// Base class for all graphics objects.
	/// </summary>
	public abstract class GraphicsBase : DrawingVisual
	{
		#region -----------------Class Members-----------------

		protected const double HitTestWidth = 8.0;
		protected const double HandleSize = 12.0;



		// external rectangle
		static SolidColorBrush handleExternalBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
		// middle
		static SolidColorBrush handleMiddleBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
		// internal
		static SolidColorBrush handleInternalBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));

		#endregion

		#region -----------------Constructor-----------------

		protected GraphicsBase()
		{
			objectId = this.GetHashCode();
		}

		#endregion

		#region -----------------Properties-----------------

		protected double _frameLeft;
		public double Left
		{
			get { return _frameLeft; }
			set { _frameLeft = value; }
		}

		protected double _frameTop;
		public double Top
		{
			get { return _frameTop; }
			set { _frameTop = value; }
		}

		protected double _frameRight;
		public double Right
		{
			get { return _frameRight; }
			set { _frameRight = value; }
		}

		protected double _frameBottom;
		public double Bottom
		{
			get { return _frameBottom; }
			set { _frameBottom = value; }
		}

		public Rect Frame
		{
			get { return new Rect(_frameLeft, _frameTop, _frameRight - _frameLeft, _frameBottom - _frameTop); }
		}

		protected bool selected;
		public bool IsSelected
		{
			get { return selected; }
			set
			{
				selected = value;
				RefreshDrawing();
			}
		}

		protected double graphicsLineWidth;
		public double LineWidth
		{
			get { return graphicsLineWidth; }
			set
			{
				graphicsLineWidth = value;
				RefreshDrawing();
			}
		}

		protected Color graphicsObjectColor;
		public Color ObjectColor
		{
			get { return graphicsObjectColor; }
			set
			{
				graphicsObjectColor = value;
				RefreshDrawing();
			}
		}

		protected double graphicsActualScale;
		public double ActualScale
		{
			get { return graphicsActualScale; }
			set
			{
				graphicsActualScale = value;
				RefreshDrawing();
			}
		}

		protected double ActualLineWidth
		{
			get { return graphicsActualScale <= 0 ? graphicsLineWidth : graphicsLineWidth / graphicsActualScale; }
		}

		protected double LineHitTestWidth
		{
			get
			{
				// Ensure that hit test area is not too narrow
				return Math.Max(8.0, ActualLineWidth);
			}
		}

		// Allows to write Undo - Redo functions and don't care about
		// objects order in the list.
		int objectId;
		/// <summary>
		/// Object ID
		/// </summary>
		public int Id
		{
			get { return objectId; }
			set { objectId = value; }
		}

		#endregion

		#region Abstract Methods and Properties

		/// <summary>
		/// Returns number of handles
		/// </summary>
		public virtual int HandleCount
		{
			get { return 8; }
		}


		/// <summary>
		/// Hit test, should be overwritten in derived classes.
		/// </summary>
		public virtual bool Contains(Point point)
		{
			return this.Frame.Contains(point);
		}

		/// <summary>
		/// Create object for serialization
		/// </summary>
		public abstract PropertiesGraphicsBase CreateSerializedObject();

		/// <summary>
		/// Get handle point by 1-based number
		/// </summary>
		public virtual Point GetHandle(int handleNumber)
		{
			double x, y, xCenter, yCenter;

			xCenter = _frameLeft + (_frameRight - _frameLeft) / 2;
			yCenter = _frameTop + (_frameBottom - _frameTop) / 2;
			x = _frameLeft;
			y = _frameTop;

			switch (handleNumber)
			{
				case 1:
					x = _frameLeft;
					y = _frameTop;
					break;
				case 2:
					x = xCenter;
					y = _frameTop;
					break;
				case 3:
					x = _frameRight;
					y = _frameTop;
					break;
				case 4:
					x = _frameRight;
					y = yCenter;
					break;
				case 5:
					x = _frameRight;
					y = _frameBottom;
					break;
				case 6:
					x = xCenter;
					y = _frameBottom;
					break;
				case 7:
					x = _frameLeft;
					y = _frameBottom;
					break;
				case 8:
					x = _frameLeft;
					y = yCenter;
					break;
			}

			return new Point(x, y);
		}

		/// <summary>
		/// Hit test.
		/// Return value: -1 - no hit
		///                0 - hit anywhere
		///                > 1 - handle number
		/// </summary>
		public virtual int MakeHitTest(Point point)
		{
			if (IsSelected)
			{
				for (int i = 1; i <= HandleCount; i++)
				{
					if (GetHandleRectangle(i).Contains(point))
						return i;
				}
			}

			if (Contains(point))
				return 0;

			return -1;
		}

		/// <summary>
		/// Get cursor for the handle
		/// </summary>
		public virtual Cursor GetHandleCursor(int handleNumber)
		{
			switch (handleNumber)
			{
				case 1:
					return Cursors.SizeNWSE;
				case 2:
					return Cursors.SizeNS;
				case 3:
					return Cursors.SizeNESW;
				case 4:
					return Cursors.SizeWE;
				case 5:
					return Cursors.SizeNWSE;
				case 6:
					return Cursors.SizeNS;
				case 7:
					return Cursors.SizeNESW;
				case 8:
					return Cursors.SizeWE;
				default:
					return Cursors.Arrow;
			}
		}

		/// <summary>
		/// Test whether object intersects with rectangle
		/// </summary>
		public virtual bool IntersectsWith(Rect rectangle)
		{
			return Frame.IntersectsWith(rectangle);
		}

		public virtual bool HasDelta()
		{
			return false;
		}

		/// <summary>
		/// Move object
		/// </summary>
		public virtual void MoveBy(double deltaX, double deltaY)
		{
			//rectangleLeft += deltaX;
			//rectangleRight += deltaX;

			//rectangleTop += deltaY;
			//rectangleBottom += deltaY;

			//RefreshDrawing();
		}

		public virtual void MoveMouse(Point point)
		{
		}

		/// <summary>
		/// Move handle to the point
		/// </summary>
		public virtual void MoveHandle(Point point, int handleNumber)
		{

		}

		public abstract void UpdateFrame();

		internal void SetFrame(double left, double top, double right, double bottom)
		{
			Normalize(ref left, ref top, ref right, ref bottom);
			Top = top;
			Left = left;
			Bottom = bottom;
			Right = right;
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Normalize object.
		/// Call this function in the end of object resizing,
		/// </summary>
		public virtual void Normalize()
		{
			Normalize(ref _frameLeft, ref _frameTop, ref _frameRight, ref _frameBottom);
		}

		public void Normalize(ref double left, ref double top, ref double right, ref double bottom)
		{
			if (left > right)
			{
				double tmp = left;
				left = right;
				right = tmp;
			}

			if (top > bottom)
			{
				double tmp = top;
				top = bottom;
				bottom = tmp;
			}
		}

		/// <summary>
		/// Implements actual drawing code.
		/// 
		/// Call GraphicsBase.Draw in the end of every derived class Draw 
		/// function to draw tracker if necessary.
		/// </summary>
		public virtual void Draw(DrawingContext drawingContext)
		{
			if (IsSelected)
			{
				DrawHandles(drawingContext);
			}
		}

		/// <summary>
		/// Draw tracker for selected object.
		/// </summary>
		public void DrawHandles(DrawingContext drawingContext)
		{
			for (int i = 1; i <= HandleCount; i++)
			{
				DrawHandleRectangle(drawingContext, GetHandleRectangle(i));
			}
		}

		/// <summary>
		/// Draw tracker rectangle
		/// </summary>
		void DrawHandleRectangle(DrawingContext drawingContext, Rect rectangle)
		{
			// External rectangle
			drawingContext.DrawRectangle(handleExternalBrush, null, rectangle);

			// Middle
			drawingContext.DrawRectangle(handleMiddleBrush, null,
				new Rect(rectangle.Left + rectangle.Width / 8,
						 rectangle.Top + rectangle.Height / 8,
						 rectangle.Width * 6 / 8,
						 rectangle.Height * 6 / 8));

			// Internal
			drawingContext.DrawRectangle(handleInternalBrush, null,
				new Rect(rectangle.Left + rectangle.Width / 4,
				 rectangle.Top + rectangle.Height / 4,
				 rectangle.Width / 2,
				 rectangle.Height / 2));
		}


		/// <summary>
		/// Refresh drawing.
		/// Called after change if any object property.
		/// </summary>
		public void RefreshDrawing()
		{
			DrawingContext dc = this.RenderOpen();
			Draw(dc);
			dc.Close();
		}

		/// <summary>
		/// Get handle rectangle by 1-based number
		/// </summary>
		public Rect GetHandleRectangle(int handleNumber)
		{
			Point point = GetHandle(handleNumber);

			// Handle rectangle should have constant size, except of the case
			// when line is too width.
			double size = Math.Max(HandleSize / graphicsActualScale, ActualLineWidth * 1.1);

			return new Rect(point.X - size / 2, point.Y - size / 2,
				size, size);
		}


		/// <summary>
		/// Dump (for debugging)
		/// </summary>
		[Conditional("DEBUG")]
		public virtual void Dump()
		{
			Trace.WriteLine(this.GetType().Name);

			Trace.WriteLine("ID = " + objectId.ToString(CultureInfo.InvariantCulture) +
				"   Selected = " + selected.ToString(CultureInfo.InvariantCulture));

			Trace.WriteLine("objectColor = " + ColorToDisplay(graphicsObjectColor) +
				"  lineWidth = " + DoubleForDisplay(graphicsLineWidth));
		}

		/// <summary>
		/// Helper function used for Dump
		/// </summary>
		static string DoubleForDisplay(double value)
		{
			return ((float)value).ToString("f2", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Helper function used for Dump
		/// </summary>
		static string ColorToDisplay(Color value)
		{
			//return "A:" + value.A.ToString() +
			return "R:" + value.R.ToString(CultureInfo.InvariantCulture) +
				   " G:" + value.G.ToString(CultureInfo.InvariantCulture) +
				   " B:" + value.B.ToString(CultureInfo.InvariantCulture);
		}


		#endregion Other Methods
	}
}
