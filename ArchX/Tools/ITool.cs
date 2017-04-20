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
    public interface ITool
    {
        void OnMouseDown(DrawingCanvas drawingCanvas, Point position);

		void OnMouseMove(DrawingCanvas drawingCanvas, Point position, MouseButtonState leftButton, MouseButtonState rightButton);

        void OnMouseUp(DrawingCanvas drawingCanvas, Point position);

        void SetCursor(DrawingCanvas drawingCanvas);
    }
}
