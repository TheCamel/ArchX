using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ArchX.Commands;
using ArchX.Components.Controls;
using ArchX.Controls;
using ArchX.Helpers;
using ArchX.Models;

namespace ArchX.Tools
{
    /// <summary>
    /// Base class for all tools which create new graphic object
    /// </summary>
    abstract class ToolObject : ToolBase
    {
        private Cursor toolCursor;

        /// <summary>
        /// Tool cursor.
        /// </summary>
        protected Cursor ToolCursor
        {
            get
            {
                return toolCursor;
            }
            set
            {
                toolCursor = value;
            }
        }


        /// <summary>
        /// Left mouse is released.
        /// New object is created and resized.
        /// </summary>
        public override void OnMouseUp(DrawingCanvas drawingCanvas, Point position)
        {
			if (drawingCanvas.Count > 0)
			{
				GraphicsBase o = drawingCanvas[drawingCanvas.Count - 1] as GraphicsBase;
				o.MoveMouse(position);

				if (!o.HasDelta())
					drawingCanvas.GraphicsList.Remove(o);
			}

            if (drawingCanvas.Count > 0)
            {
                drawingCanvas[drawingCanvas.Count - 1].Normalize();
                drawingCanvas.AddCommandToHistory(new CommandAdd(drawingCanvas[drawingCanvas.Count - 1]));
            }

            drawingCanvas.Cursor = Cursors.Arrow;
            drawingCanvas.ReleaseMouseCapture();
			drawingCanvas.InvalidateMeasure();
        }

        /// <summary>
        /// Add new object to drawing canvas.
        /// Function is called when user left-clicks drawing canvas,
        /// and one of ToolObject-derived tools is active.
        /// </summary>
        protected static void AddNewObject(DrawingCanvas drawingCanvas, GraphicsBase o)
        {
            HelperFunctions.UnselectAll(drawingCanvas);

            o.IsSelected = true;
            //o.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));

            drawingCanvas.GraphicsList.Add(o);
            drawingCanvas.CaptureMouse();
        }

        /// <summary>
        /// Set cursor
        /// </summary>
        public override void SetCursor(DrawingCanvas drawingCanvas)
        {
            drawingCanvas.Cursor = this.toolCursor;
        }

    }
}
