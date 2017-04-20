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
	/// Pointer tool
	/// </summary>
	class ToolPointer : ToolBase
	{
		private enum SelectionMode
		{
			None,
			Move,           // object(s) are moved
			Size,           // object is resized
			GroupSelection
		}

		private SelectionMode selectMode = SelectionMode.None;

		// Object which is currently resized:
		private GraphicsBase resizedObject;
		private int resizedObjectHandle;

		// Keep state about last and current point (used to move and resize objects)
		private Point lastPoint = new Point(0, 0);

		private CommandChangeState commandChangeState;
		bool wasMove;

		public ToolPointer()
		{
		}

		/// <summary>
		/// Handle mouse down.
		/// Start moving, resizing or group selection.
		/// </summary>
		public override void OnMouseDown(DrawingCanvas drawingCanvas, Point position)
		{
			commandChangeState = null;
			wasMove = false;

			selectMode = SelectionMode.None;

			GraphicsBase o;
			GraphicsBase movedObject = null;
			int handleNumber;

			// Test for resizing (only if control is selected, cursor is on the handle)
			for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
			{
				o = drawingCanvas[i];

				if (o.IsSelected)
				{
					handleNumber = o.MakeHitTest(position);

					if (handleNumber > 0)
					{
						selectMode = SelectionMode.Size;

						// keep resized object in class member
						resizedObject = o;
						resizedObjectHandle = handleNumber;

						// Since we want to resize only one object, unselect all other objects
						HelperFunctions.UnselectAll(drawingCanvas);
						o.IsSelected = true;

						commandChangeState = new CommandChangeState(drawingCanvas);

						break;
					}
				}
			}

			// Test for move (cursor is on the object)
			if (selectMode == SelectionMode.None)
			{
				for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
				{
					o = drawingCanvas[i];

					if (o.MakeHitTest(position) == 0)
					{
						movedObject = o;
						break;
					}
				}

				if (movedObject != null)
				{
					selectMode = SelectionMode.Move;

					// Unselect all if Ctrl is not pressed and clicked object is not selected yet
					if (Keyboard.Modifiers != ModifierKeys.Control && !movedObject.IsSelected)
					{
						HelperFunctions.UnselectAll(drawingCanvas);
					}

					// Select clicked object
					movedObject.IsSelected = true;

					// Set move cursor
					drawingCanvas.Cursor = Cursors.SizeAll;

					commandChangeState = new CommandChangeState(drawingCanvas);
				}
			}

			// Click on background
			if (selectMode == SelectionMode.None)
			{
				// Unselect all if Ctrl is not pressed
				if (Keyboard.Modifiers != ModifierKeys.Control)
				{
					HelperFunctions.UnselectAll(drawingCanvas);
				}

				// Group selection. Create selection rectangle.
				GraphicsSelectionRectangle r = new GraphicsSelectionRectangle(
					position.X, position.Y,
					position.X + 1, position.Y + 1,
					drawingCanvas.ActualScale);

				//r.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));

				drawingCanvas.GraphicsList.Add(r);

				selectMode = SelectionMode.GroupSelection;
			}


			lastPoint = position;

			// Capture mouse until MouseUp event is received
			drawingCanvas.CaptureMouse();
		}

		/// <summary>
		/// Handle mouse move.
		/// Se cursor, move/resize, make group selection.
		/// </summary>
		public override void OnMouseMove(DrawingCanvas drawingCanvas, Point position, MouseButtonState leftButton, MouseButtonState rightButton)
		{
			// Exclude all cases except left button on/off.
			if (rightButton == MouseButtonState.Pressed)
			{
				drawingCanvas.Cursor = Cursors.Arrow;
				return;
			}

			// Set cursor when left button is not pressed
			if (leftButton == MouseButtonState.Released)
			{
				Cursor cursor = null;

				for (int i = 0; i < drawingCanvas.Count; i++)
				{
					int n = drawingCanvas[i].MakeHitTest(position);

					if (n > 0)
					{
						cursor = drawingCanvas[i].GetHandleCursor(n);
						break;
					}
				}

				if (cursor == null)
					cursor = Cursors.Arrow;

				drawingCanvas.Cursor = cursor;

				return;

			}

			if (!drawingCanvas.IsMouseCaptured)
			{
				return;
			}

			wasMove = true;

			// Find difference between previous and current position
			double dx = position.X - lastPoint.X;
			double dy = position.Y - lastPoint.Y;

			lastPoint = position;

			// Resize
			if (selectMode == SelectionMode.Size)
			{
				if (resizedObject != null)
				{
					resizedObject.MoveHandle(position, resizedObjectHandle);
				}
			}

			// Move
			if (selectMode == SelectionMode.Move)
			{
				foreach (GraphicsBase o in drawingCanvas.Selection)
				{
					o.MoveBy(dx, dy);
				}
			}

			// Group selection
			if (selectMode == SelectionMode.GroupSelection)
			{
				// Resize selection rectangle
				drawingCanvas[drawingCanvas.Count - 1].MoveHandle(
					position, 5);
			}
		}

		/// <summary>
		/// Handle mouse up.
		/// Return to normal state.
		/// </summary>
		public override void OnMouseUp(DrawingCanvas drawingCanvas, Point position)
		{
			if (!drawingCanvas.IsMouseCaptured)
			{
				drawingCanvas.Cursor = Cursors.Arrow;
				selectMode = SelectionMode.None;
				return;
			}

			if (resizedObject != null)
			{
				// after resizing
				resizedObject.Normalize();

				resizedObject = null;
			}

			if (selectMode == SelectionMode.GroupSelection)
			{
				GraphicsSelectionRectangle r = (GraphicsSelectionRectangle)drawingCanvas[drawingCanvas.Count - 1];
				r.Normalize();
				Rect rect = r.Frame;

				drawingCanvas.GraphicsList.Remove(r);

				foreach (GraphicsBase g in drawingCanvas.GraphicsList)
				{
					if (g.IntersectsWith(rect))
					{
						g.IsSelected = true;
					}
				}
			}

			drawingCanvas.ReleaseMouseCapture();
			drawingCanvas.InvalidateMeasure();
			drawingCanvas.Cursor = Cursors.Arrow;

			selectMode = SelectionMode.None;

			AddChangeToHistory(drawingCanvas);
		}

		/// <summary>
		/// Set cursor
		/// </summary>
		public override void SetCursor(DrawingCanvas drawingCanvas)
		{
			drawingCanvas.Cursor = Cursors.Arrow;
		}


		/// <summary>
		/// Add change to history.
		/// Called after finishing moving/resizing.
		/// </summary>
		public void AddChangeToHistory(DrawingCanvas drawingCanvas)
		{
			if (commandChangeState != null && wasMove)
			{
				// Keep state after moving/resizing and add command to history
				commandChangeState.NewState(drawingCanvas);
				drawingCanvas.AddCommandToHistory(commandChangeState);
				commandChangeState = null;
			}
		}

	}
}
