using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;
using ArchX.Controls;
using ArchX.Commands;
using ArchX.Models;
using ArchX.Components.Controls;
using System.Runtime.InteropServices;



namespace ArchX.Helpers
{

	

    /// <summary>
    /// Helper class which contains general helper functions and properties.
    /// 
    /// Most functions in this class replace VisualCollection-derived class
    /// methods, because I cannot derive from VisualCollection.
    /// They make different operations with GraphicsBase list.
    /// </summary>
    static class HelperFunctions
    {
		[DllImport("User32.dll")]
		public static extern bool SetCursorPos(int x, int y);

		//public static Point ConvertToScreen(Ruler rule, Point local, GuideOrientation orien)
		//{
		//	double toscreenX = local.X, toscreenY = local.Y;
		//	if (orien == GuideOrientation.Vertical || orien == GuideOrientation.Diagonal)
		//		rule.PageManager.XLogicToDot(ref toscreenX, local.X);

		//	if (orien == GuideOrientation.Horizontal || orien == GuideOrientation.Diagonal)
		//		rule.PageManager.YLogicToDot(ref toscreenY, local.Y);

		//	gl.GetNearestPos(ref hitPoint, _Container.dPicCapture);
		//	return new Point(toscreenX, toscreenY);
		//}

		//public static void ConvertToScreen( Ruler rule, ref Point local, UIElement relativto, GuideOrientation orien )
		//{
		//	double toscreenX = local.X, toscreenY = local.Y;
		//	if (orien == GuideOrientation.Vertical || orien == GuideOrientation.Diagonal)
		//		rule.PageManager.XLogicToDot(ref toscreenX, local.X);

		//	if (orien == GuideOrientation.Horizontal || orien == GuideOrientation.Diagonal)
		//		rule.PageManager.YLogicToDot(ref toscreenY, local.Y);

		//	Point result = relativto.PointToScreen(new Point(toscreenX, toscreenY));
		//	HelperFunctions.SetCursorPos((int)result.X, (int)result.Y);
		//}

        /// <summary>
        /// Select all graphic objects
        /// </summary>
        public static void SelectAll(DrawingCanvas drawingCanvas)
        {
            for(int i = 0; i < drawingCanvas.Count; i++)
            {
                drawingCanvas[i].IsSelected = true;
            }
        }

        /// <summary>
        /// Unselect all graphic objects
        /// </summary>
        public static void UnselectAll(DrawingCanvas drawingCanvas)
        {
            for (int i = 0; i < drawingCanvas.Count; i++)
            {
                drawingCanvas[i].IsSelected = false;
            }
        }

        /// <summary>
        /// Delete selected graphic objects
        /// </summary>
        public static void DeleteSelection(DrawingCanvas drawingCanvas)
        {
            CommandDelete command = new CommandDelete(drawingCanvas);
            bool wasChange = false;

            for (int i = drawingCanvas.Count - 1; i >= 0; i--)
            {
                if ( drawingCanvas[i].IsSelected )
                {
                    drawingCanvas.GraphicsList.RemoveAt(i);
                    wasChange = true;
                }
            }

            if ( wasChange )
            {
                drawingCanvas.AddCommandToHistory(command);
            }
        }

        /// <summary>
        /// Delete all graphic objects
        /// </summary>
        public static void DeleteAll(DrawingCanvas drawingCanvas)
        {
            if (drawingCanvas.GraphicsList.Count > 0 )
            {
                drawingCanvas.AddCommandToHistory(new CommandDeleteAll(drawingCanvas));

                drawingCanvas.GraphicsList.Clear();
            }

        }

        /// <summary>
        /// Move selection to front
        /// </summary>
        public static void MoveSelectionToFront(DrawingCanvas drawingCanvas)
        {
            // Moving to front of z-order means moving
            // to the end of VisualCollection.

            // Read GraphicsList in the reverse order, and move every selected object
            // to temporary list.

            List<GraphicsBase> list = new List<GraphicsBase>();

            CommandChangeOrder command = new CommandChangeOrder(drawingCanvas);

            for(int i = drawingCanvas.Count - 1; i >= 0; i--)
            {
                if ( drawingCanvas[i].IsSelected )
                {
                    list.Insert(0, drawingCanvas[i]);
                    drawingCanvas.GraphicsList.RemoveAt(i);
                }
            }

            // Add all items from temporary list to the end of GraphicsList
            foreach(GraphicsBase g in list)
            {
                drawingCanvas.GraphicsList.Add(g);
            }

            if ( list.Count > 0 )
            {
                command.NewState(drawingCanvas);
                drawingCanvas.AddCommandToHistory(command);
            }
        }

        /// <summary>
        /// Move selection to back
        /// </summary>
        public static void MoveSelectionToBack(DrawingCanvas drawingCanvas)
        {
            // Moving to back of z-order means moving
            // to the beginning of VisualCollection.

            // Read GraphicsList in the reverse order, and move every selected object
            // to temporary list.

            List<GraphicsBase> list = new List<GraphicsBase>();

            CommandChangeOrder command = new CommandChangeOrder(drawingCanvas);

            for (int i = drawingCanvas.Count - 1; i >= 0; i--)
            {
                if (drawingCanvas[i].IsSelected)
                {
                    list.Add(drawingCanvas[i]);
                    drawingCanvas.GraphicsList.RemoveAt(i);
                }
            }

            // Add all items from temporary list to the beginning of GraphicsList
            foreach (GraphicsBase g in list)
            {
                drawingCanvas.GraphicsList.Insert(0, g);
            }

            if (list.Count > 0)
            {
                command.NewState(drawingCanvas);
                drawingCanvas.AddCommandToHistory(command);
            }
        }

        /// <summary>
        /// Apply new line width
        /// </summary>
        public static bool ApplyLineWidth(DrawingCanvas drawingCanvas, double value, bool addToHistory)
        {
            CommandChangeState command = new CommandChangeState(drawingCanvas);
            bool wasChange = false;


            // LineWidth is set for all objects except of GraphicsText.
            // Though GraphicsText has this property, it should remain constant.

            foreach(GraphicsBase g in drawingCanvas.Selection)
            {
                if (g is GraphicsRectangle ||
                     g is GraphicsLine ||
                     g is GraphicsPolyLine)
                {
                    if ( g.LineWidth != value )
                    {
                        g.LineWidth = value;
                        wasChange = true;
                    }
                }
            }

            if (wasChange  && addToHistory)
            {
                command.NewState(drawingCanvas);
                drawingCanvas.AddCommandToHistory(command);
            }

            return wasChange;
        }

        /// <summary>
        /// Apply new color
        /// </summary>
        public static bool ApplyColor(DrawingCanvas drawingCanvas, Color value, bool addToHistory)
        {
            CommandChangeState command = new CommandChangeState(drawingCanvas);
            bool wasChange = false;

            foreach (GraphicsBase g in drawingCanvas.Selection)
            {
                if (g.ObjectColor != value)
                {
                    g.ObjectColor = value;
                    wasChange = true;
                }
            }

            if ( wasChange && addToHistory )
            {
                command.NewState(drawingCanvas);
                drawingCanvas.AddCommandToHistory(command);
            }

            return wasChange;
        }

        


        /// <summary>
        /// Dump graphics list (for debugging)
        /// </summary>
        [Conditional("DEBUG")]
        public static void Dump(VisualCollection graphicsList, string header)
        {
            Trace.WriteLine("");
            Trace.WriteLine(header);
            Trace.WriteLine("");

            foreach(GraphicsBase g in graphicsList)
            {
                g.Dump();
            }
        }

        /// <summary>
        /// Dump graphics list overload
        /// </summary>
        [Conditional("DEBUG")]
        public static void Dump(VisualCollection graphicsList)
        {
            Dump(graphicsList, "Graphics List");
        }

        /// <summary>
        /// Return true if currently active properties (line width, color etc.)
        /// can be applied to selected items.
        /// 
        /// If at least one selected object has property different from currently
        /// active property value, properties can be applied.
        /// </summary>
		//public static bool CanApplyProperties(DrawingCanvas drawingCanvas)
		//{
		//	foreach(GraphicsBase graphicsBase in drawingCanvas.GraphicsList)
		//	{
		//		if ( ! graphicsBase.IsSelected )
		//		{
		//			continue;
		//		}

		//		// ObjectColor - used in all graphics objects
		//		if ( graphicsBase.ObjectColor != drawingCanvas.ObjectColor )
		//		{
		//			return true;
		//		}

                

		//	return false;
		//}

        /// <summary>
        /// Apply currently active properties to selected objects
        /// </summary>
        public static void ApplyProperties(DrawingCanvas drawingCanvas)
        {
            // Apply every property.
            // Call every Apply* function with addToHistory = false.
            // History is updated here and not in called functions.

            CommandChangeState command = new CommandChangeState(drawingCanvas);
            bool wasChange = false;

            

            if ( wasChange )
            {
                command.NewState(drawingCanvas);
                drawingCanvas.AddCommandToHistory(command);
            }
        }

    }
}
