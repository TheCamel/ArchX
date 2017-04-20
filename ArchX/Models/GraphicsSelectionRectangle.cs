using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArchX.Models
{
	/// <summary>
	/// Selection Rectangle graphics object, used for group selection.
	/// 
	/// Instance of this class should be created only for group selection
	/// and removed immediately after group selection finished.
	/// </summary>
	class GraphicsSelectionRectangle : GraphicsBase
	{
		#region Constructors

		public GraphicsSelectionRectangle(double left, double top, double right, double bottom, double actualScale)
		{
			Normalize(ref left, ref top, ref right, ref bottom);
			Left = left;
			Top = top;
			Bottom = bottom;
			Right= right;
			this.graphicsLineWidth = 1.0;
			this.graphicsActualScale = actualScale;
		}

		public GraphicsSelectionRectangle()
			:
			this(0.0, 0.0, 100.0, 100.0, 1.0)
		{
		}

		#endregion Constructors

		#region Overrides

		/// <summary>
		/// Draw graphics object
		/// </summary>
		public override void Draw(DrawingContext drawingContext)
		{
			drawingContext.DrawRectangle(
				null,
				new Pen(Brushes.White, ActualLineWidth),
				Frame);

			DashStyle dashStyle = new DashStyle();
			dashStyle.Dashes.Add(4);

			Pen dashedPen = new Pen(Brushes.Black, ActualLineWidth);
			dashedPen.DashStyle = dashStyle;


			drawingContext.DrawRectangle(
				null,
				dashedPen,
				Frame);
		}

		public override bool Contains(Point point)
		{
			return this.Frame.Contains(point);
		}

		public override PropertiesGraphicsBase CreateSerializedObject()
		{
			return null;        // not used
		}

		override public void UpdateFrame()
		{
		}

		#endregion Overrides
	}
}
