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
    /// Rectangle tool
    /// </summary>
	class ToolRectangle : ToolObject
    {
        public ToolRectangle()
        {
			ToolCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Create new rectangle
        /// </summary>
		public override void OnMouseDown(DrawingCanvas drawingCanvas, Point position)
        {
            AddNewObject(drawingCanvas, 
                new GraphicsRectangle(
				position.X,
				position.Y,
				position.X + 1,
				position.Y + 1,
                1,
                Colors.Black,
                drawingCanvas.ActualScale));
        }

		public override void OnMouseMove(DrawingCanvas drawingCanvas, Point position, MouseButtonState leftButton, MouseButtonState rightButton)
		{
			drawingCanvas.Cursor = ToolCursor;

			if (leftButton == MouseButtonState.Pressed)
			{
				if (drawingCanvas.IsMouseCaptured)
				{
					if (drawingCanvas.Count > 0)
					{
						GraphicsRectangle grect = drawingCanvas[drawingCanvas.Count - 1] as GraphicsRectangle;
						grect.MoveMouse( position);
					}
				}
			}
		}
    }
}
