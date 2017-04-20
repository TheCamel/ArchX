using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using ArchX.Controls;
using ArchX.Components.Controls;

namespace ArchX.Tools
{
    /// <summary>
    /// Base class for rectangle-based tools
    /// </summary>
    abstract class ToolRectangleBase : ToolObject
    {
        /// <summary>
        /// Set cursor and resize new object.
        /// </summary>
		public override void OnMouseMove(DrawingCanvas drawingCanvas, Point position, MouseButtonState leftButton, MouseButtonState rightButton)
        {
            drawingCanvas.Cursor = ToolCursor;

			if (leftButton == MouseButtonState.Pressed)
            {
                if (drawingCanvas.IsMouseCaptured)
                {
                    if ( drawingCanvas.Count > 0 )
                    {
                        drawingCanvas[drawingCanvas.Count - 1].MoveHandle(
							position, 5);
                    }
                }

            }
        }

    }
}
