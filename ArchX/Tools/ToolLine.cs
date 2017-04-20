using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using ArchX.Controls;
using ArchX.Models;
using ArchX.Components.Controls;


namespace ArchX.Tools
{
    /// <summary>
    /// Line tool
    /// </summary>
    class ToolLine : ToolObject
    {
        public ToolLine()
        {
			ToolCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Create new object
        /// </summary>
		public override void OnMouseDown(DrawingCanvas drawingCanvas, Point position)
        {
            AddNewObject(drawingCanvas,
                new GraphicsLine(
				position,
				new Point(position.X + 1, position.Y + 1),
                1,
                Colors.Black,
                drawingCanvas.ActualScale));

        }

        /// <summary>
        /// Set cursor and resize new object.
        /// </summary>
		public override void OnMouseMove(DrawingCanvas drawingCanvas, Point position, MouseButtonState leftButton, MouseButtonState rightButton)
        {
            drawingCanvas.Cursor = ToolCursor;

            if (leftButton == MouseButtonState.Pressed)
            {
                drawingCanvas[drawingCanvas.Count - 1].MoveMouse(position);
            }
        }
    }
}
