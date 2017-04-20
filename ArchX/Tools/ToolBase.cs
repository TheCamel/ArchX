using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ArchX.Components.Controls;
using ArchX.Controls;

namespace ArchX.Tools
{
    /// <summary>
    /// Base class for all drawing tools
    /// </summary>
    abstract class ToolBase : ITool
    {
        public abstract void OnMouseDown(DrawingCanvas drawingCanvas, Point position);

		public abstract void OnMouseMove(DrawingCanvas drawingCanvas, Point position, MouseButtonState leftButton, MouseButtonState rightButton);

        public abstract void OnMouseUp(DrawingCanvas drawingCanvas, Point position);

        public abstract void SetCursor(DrawingCanvas drawingCanvas);
    }
}
