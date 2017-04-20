using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using ArchX.Models;
using ArchX.Controls;
using ArchX.Components.Controls;

namespace ArchX.Tools
{
    /// <summary>
    /// Polyline tool
    /// </summary>
    class ToolPolyLine : ToolObject
    {
        private double lastX;
        private double lastY;
        private GraphicsPolyLine newPolyLine;
        private const double minDistance = 15;


        public ToolPolyLine()
        {
			ToolCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Create new object
        /// </summary>
        public override void OnMouseDown(DrawingCanvas drawingCanvas, Point position)
        {
            newPolyLine = new GraphicsPolyLine(
                new Point[]
                {
                    position,
                    new Point(position.X + 1, position.Y + 1)
                },
                1,
                Colors.Black,
                drawingCanvas.ActualScale);

            AddNewObject(drawingCanvas, newPolyLine);

			lastX = position.X;
			lastY = position.Y;
        }

        /// <summary>
        /// Set cursor and resize new polyline
        /// </summary>
		public override void OnMouseMove(DrawingCanvas drawingCanvas, Point position, MouseButtonState leftButton, MouseButtonState rightButton)
        {
            drawingCanvas.Cursor = ToolCursor;

            if (leftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if ( ! drawingCanvas.IsMouseCaptured )
            {
                return;
            }

            if ( newPolyLine == null )
            {
                return;         // precaution
            }


			double distance = (position.X - lastX) * (position.X - lastX) + (position.Y - lastY) * (position.Y - lastY);

            double d = drawingCanvas.ActualScale <= 0 ? 
                minDistance * minDistance :
                minDistance * minDistance / drawingCanvas.ActualScale;

            if ( distance < d)
            {
                // Distance between last two points is less than minimum -
                // move last point
                newPolyLine.MoveHandle(position, newPolyLine.HandleCount);
            }
            else
            {
                // Add new segment
                newPolyLine.AddPoint(position);

                lastX = position.X;
                lastY = position.Y;
            }
        }

		public override void OnMouseUp(DrawingCanvas drawingCanvas, Point position)
        {
            newPolyLine = null;

            base.OnMouseUp(drawingCanvas, position);
        }
    }
}
