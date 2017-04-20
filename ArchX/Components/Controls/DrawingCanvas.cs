using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Documents;
using ArchX.Tools;
using ArchX.Models;
using ArchX.Commands;
using ArchX.Controls;
using ArchX.Helpers;
using ArchX.Controls.Guidelines;

namespace ArchX.Components.Controls
{
	/// <summary>
	/// Canvas used as host for DrawingVisual objects.
	/// Allows to draw graphics objects using mouse.
	/// </summary>
	public class DrawingCanvas : Canvas
	{
		#region --------------CONSTRUCTOR--------------

		public DrawingCanvas()
			: base()
		{
			graphicsList = new VisualCollection(this);

			this.FocusVisualStyle = null;
			this.Focusable = true;      // to handle keyboard messages

			this.MouseDown += new MouseButtonEventHandler(DrawingCanvas_MouseDown);
			this.MouseMove += new MouseEventHandler(DrawingCanvas_MouseMove);
			this.MouseUp += new MouseButtonEventHandler(DrawingCanvas_MouseUp);
			this.KeyDown += new KeyEventHandler(DrawingCanvas_KeyDown);
			this.LostMouseCapture += new MouseEventHandler(DrawingCanvas_LostMouseCapture);
			this.MouseLeave += new MouseEventHandler(DrawingCanvas_MouseLeave);
			this.Loaded += DrawingCanvas_Loaded;
			this.MouseWheel += DrawingCanvas_MouseWheel;

			// Create undo manager
			undoManager = new UndoManager(this);
			undoManager.StateChanged += new EventHandler(undoManager_StateChanged);

			CurrentTool = new ToolPointer();
		}

		#endregion

		#region --------------DEPENDENCY PROPERTIES--------------

		#region --------------Current ruler --------------

		public static readonly DependencyProperty CurrentRulerProperty = DependencyProperty.Register(
				"CurrentRuler", typeof(Ruler), typeof(DrawingCanvas),
				 new PropertyMetadata(null, new PropertyChangedCallback(CurrentRulerChanged)));

		public Ruler CurrentRuler
		{
			get { return (Ruler)GetValue(CurrentRulerProperty); }
			set { SetValue(CurrentRulerProperty, value); }
		}

		static void CurrentRulerChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			DrawingCanvas myself = property as DrawingCanvas;

		}
		#endregion

		#region --------------is snapping--------------

		private static readonly DependencyProperty IsSnappingProperty = DependencyProperty.Register(
				"IsSnapping", typeof(bool), typeof(DrawingCanvas),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
					new PropertyChangedCallback(OnIsSnappingChanged)));

		public bool IsSnapping
		{
			get
			{
				return (bool)GetValue(IsSnappingProperty);
			}
			set
			{
				SetValue(IsSnappingProperty, value);
			}
		}

		private static void OnIsSnappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			DrawingCanvas element = d as DrawingCanvas;
			element.AfterIsSnappingChanged((bool)e.NewValue);
		}

		protected void AfterIsSnappingChanged(bool after)
		{
		}

		#endregion

		#region --------------Current tool --------------

		public static readonly DependencyProperty CurrentToolProperty = DependencyProperty.Register(
				"CurrentTool", typeof(ITool), typeof(DrawingCanvas),
				 new PropertyMetadata(null, new PropertyChangedCallback(CurrentToolChanged)));

		public ITool CurrentTool
		{
			get { return (ITool)GetValue(CurrentToolProperty); }
			set { SetValue(CurrentToolProperty, value); }
		}

		static void CurrentToolChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			DrawingCanvas myself = property as DrawingCanvas;

		}
		#endregion

		#region --------------activ surface --------------

		private static readonly DependencyProperty ActiveSurfaceProperty = DependencyProperty.Register(
				"ActiveSurface", typeof(Rect), typeof(DrawingCanvas), new PropertyMetadata(new Rect(0, 0, 0, 0)));

		public Rect ActiveSurface
		{
			get { return (Rect)GetValue(ActiveSurfaceProperty); }
			set { SetValue(ActiveSurfaceProperty, value); }
		}
		#endregion

		#region --------------current position --------------

		private static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register(
				"CurrentPosition", typeof(Point), typeof(DrawingCanvas), new PropertyMetadata(new Point(-1, -1)));

		public Point CurrentPosition
		{
			get { return (Point)GetValue(CurrentPositionProperty); }
			set { SetValue(CurrentPositionProperty, value); }
		}
		#endregion

		#region --------------current position --------------

		private static readonly DependencyProperty CurrentSnapPositionProperty = DependencyProperty.Register(
				"CurrentSnapPosition", typeof(Point), typeof(DrawingCanvas), new PropertyMetadata(new Point(-1, -1)));

		public Point CurrentSnapPosition
		{
			get { return (Point)GetValue(CurrentSnapPositionProperty); }
			set { SetValue(CurrentSnapPositionProperty, value); }
		}
		#endregion

		#region --------------current position --------------
		private static readonly DependencyProperty CurrentUnityPositionProperty = DependencyProperty.Register(
				"CurrentUnityPosition", typeof(Point), typeof(DrawingCanvas), new PropertyMetadata(new Point(-1, -1)));

		public Point CurrentUnityPosition
		{
			get { return (Point)GetValue(CurrentUnityPositionProperty); }
			set { SetValue(CurrentUnityPositionProperty, value); }
		}
		#endregion

		#region --------------scale --------------

		public static readonly DependencyProperty ActualScaleProperty = DependencyProperty.Register(
				"ActualScale", typeof(double), typeof(DrawingCanvas),
				 new PropertyMetadata( 1.0,
					new PropertyChangedCallback(ActualScaleChanged)));

		/// <summary>
		/// Dependency property ActualScale.
		/// </summary>
		public double ActualScale
		{
			get { return (double)GetValue(ActualScaleProperty); }
			set { SetValue(ActualScaleProperty, value); }
		}

		/// <summary>
		/// Callback function called when ActualScale dependency property is changed.
		/// </summary>
		static void ActualScaleChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			DrawingCanvas d = property as DrawingCanvas;

			double scale = d.ActualScale;
		}
		#endregion

		#region --------------dirty --------------

		public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
				"IsDirty", typeof(bool), typeof(DrawingCanvas),
				new PropertyMetadata(false));

		public bool IsDirty
		{
			get { return (bool)GetValue(IsDirtyProperty); }
			internal set {
				SetValue(IsDirtyProperty, value);
				RoutedEventArgs newargs = new RoutedEventArgs(IsDirtyChangedEvent);
				RaiseEvent(newargs);
			}
		}
		#endregion

		#region --------------can undo --------------

		public static readonly DependencyProperty CanUndoProperty = DependencyProperty.Register(
				"CanUndo", typeof(bool), typeof(DrawingCanvas),
				new PropertyMetadata(false));

		public bool CanUndo
		{
			get { return (bool)GetValue(CanUndoProperty); }
			internal set { SetValue(CanUndoProperty, value); }
		}

		#endregion

		#region --------------can redo--------------

		public static readonly DependencyProperty CanRedoProperty = DependencyProperty.Register(
				"CanRedo", typeof(bool), typeof(DrawingCanvas),
				new PropertyMetadata(false));

		public bool CanRedo
		{
			get { return (bool)GetValue(CanRedoProperty); }
			internal set { SetValue(CanRedoProperty, value); }
		}

		#endregion

		#region --------------IsDirtyChangedEvent --------------

		public static readonly RoutedEvent IsDirtyChangedEvent = EventManager.RegisterRoutedEvent("IsDirtyChangedChanged",
				RoutingStrategy.Bubble, typeof(DependencyPropertyChangedEventHandler), typeof(DrawingCanvas));

		public event RoutedEventHandler IsDirtyChanged
		{
			add { AddHandler(IsDirtyChangedEvent, value); }
			remove { RemoveHandler(IsDirtyChangedEvent, value); }
		}

		#endregion

		#endregion

		#region --------------PROPERTIES--------------

		// Collection contains instances of GraphicsBase-derived classes.
		private VisualCollection graphicsList;

		/// <summary>
		/// Return list of graphics
		/// </summary>
		internal VisualCollection GraphicsList
		{
			get { return graphicsList; }
		}

		/// <summary>
		/// Get number of graphic objects
		/// </summary>
		internal int Count
		{
			get { return graphicsList.Count; }
		}

		/// <summary>
		/// Returns INumerable which may be used for enumeration
		/// of selected objects.
		/// </summary>
		internal IEnumerable<GraphicsBase> Selection
		{
			get
			{
				foreach (GraphicsBase o in graphicsList)
				{
					if (o.IsSelected)
					{
						yield return o;
					}
				}
			}
		}

		/// <summary>
		/// Get number of selected graphic objects
		/// </summary>
		internal int SelectionCount
		{
			get
			{
				int n = 0;

				foreach (GraphicsBase g in this.graphicsList)
				{
					if (g.IsSelected)
					{
						n++;
					}
				}
				return n;
			}
		}

		/// <summary>
		/// Get graphic object by index
		/// </summary>
		internal GraphicsBase this[int index]
		{
			get
			{
				if (index >= 0 && index < Count)
				{
					return (GraphicsBase)graphicsList[index];
				}

				return null;
			}
		}


		//public DrawingContainer Container { get { return TemplatedParent as DrawingContainer; } }

		private PlanAdorner _planAD = null;
		private UndoManager undoManager;
		#endregion

		#region --------------OVERRIDES--------------

		/// <summary>
		/// Get number of children: VisualCollection count.
		/// If in-place editing textbox is active, add 1.
		/// </summary>
		protected override int VisualChildrenCount
		{
			get { return graphicsList.Count; }
		}

		/// <summary>
		/// Get visual child - one of GraphicsBase objects
		/// or in-place editing textbox, if it is active.
		/// </summary>
		protected override Visual GetVisualChild(int index)
		{
			if (index < 0 || index >= graphicsList.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			return graphicsList[index];
		}

		protected override Size MeasureOverride(Size constraint)
		{
			double bottomMost = 0d;
			double rightMost = 0d;

			foreach (object obj in graphicsList)
			{
				GraphicsBase child = obj as GraphicsBase;

				if (child != null)
				{
					bottomMost = Math.Max(bottomMost, child.Bottom);
					rightMost = Math.Max(rightMost, child.Right);
				}
			}
			return new Size(rightMost, bottomMost);
		}


		#endregion

		#region --------------EVENTS--------------

		void DrawingCanvas_Loaded(object sender, RoutedEventArgs e)
		{
			if (_planAD == null)
			{
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
				_planAD = new PlanAdorner(this);
				layer.Add(_planAD);
			}
		}

		void DrawingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			ActualScale += (e.Delta > 0) ? 0.1 : -0.1;
			_planAD.InvalidateVisual();
			ActiveSurface = new Rect(-500 * 1 / ActualScale, -500 * 1 / ActualScale, 
				1000 * 1 / ActualScale, 1000 * 1 / ActualScale);
		}

		void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.Focus();

			if (e.ChangedButton == MouseButton.Left)
			{
				if (e.ClickCount == 2)
				{
					HandleDoubleClick(e);        // special case for GraphicsText
				}
				else
				{
					Point loc = e.GetPosition(this);

					if (CurrentRuler.IsSnapping)
					{
						CurrentSnapPosition = new Point(-1, -1);

						Point snap = new Point(loc.X, loc.Y);

						if (CurrentRuler.GuideManager.Intersection_GetNearst(ref snap))
						{
							CurrentSnapPosition = snap;
							CurrentPosition = loc;
							CurrentTool.OnMouseDown(this, CurrentSnapPosition);
							return;
						}

						Guideline gl = CurrentRuler.GuideManager.GetSnapGuide(loc);
						if (gl != null)
						{
							gl.GetNearestPos(ref snap, CurrentRuler.dPicCapture);
							CurrentSnapPosition = snap;
							CurrentPosition = loc;
							CurrentTool.OnMouseDown(this, CurrentSnapPosition);
							return;
						}
					}
					CurrentPosition = loc;
					CurrentTool.OnMouseDown(this, CurrentPosition);
				}
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				//CurrentRuler.IsOnGuide();
				//show guide menu
				//CurrentRuler.ContextMenu.PlacementTarget = CurrentRuler;
				//CurrentRuler.ContextMenu.IsOpen = true;
				this.ContextMenu.IsOpen = true;
			}

			e.Handled = true;
		}

		void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			Point loc = e.GetPosition(this);
			//CurrentUnityPosition = Container.ConvertToUnity(CurrentPosition);

			if (e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
			{
				if (CurrentRuler.IsSnapping)
				{
					CurrentSnapPosition = new Point(-1, -1);

					Point snap = new Point(loc.X, loc.Y);

					if (CurrentRuler.GuideManager.Intersection_GetNearst(ref snap))
					{
						CurrentSnapPosition = snap;
						CurrentPosition = loc;
						CurrentTool.OnMouseMove(this, CurrentSnapPosition, e.LeftButton, e.RightButton);
						return;
					}

					Guideline gl = CurrentRuler.GuideManager.GetSnapGuide(loc);
					if (gl != null)
					{
						gl.GetNearestPos(ref snap, CurrentRuler.dPicCapture);
						CurrentSnapPosition = snap;
						CurrentPosition = loc;
						CurrentTool.OnMouseMove(this, CurrentSnapPosition, e.LeftButton, e.RightButton);
						return;
					}
				}
				CurrentPosition = loc; 
				CurrentTool.OnMouseMove(this, CurrentPosition, e.LeftButton, e.RightButton);
			}
			else
			{
				this.Cursor = Cursors.Arrow;
			}
		}

		void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				Point loc = e.GetPosition(this);

				if (CurrentRuler.IsSnapping)
				{
					CurrentSnapPosition = new Point(-1, -1);

					Point snap = new Point(loc.X, loc.Y);

					if (CurrentRuler.GuideManager.Intersection_GetNearst(ref snap))
					{
						CurrentSnapPosition = snap;
						CurrentPosition = loc;
						CurrentTool.OnMouseUp(this, CurrentSnapPosition);
						return;
					}

					Guideline gl = CurrentRuler.GuideManager.GetSnapGuide(loc);
					if (gl != null)
					{
						gl.GetNearestPos(ref snap, CurrentRuler.dPicCapture);
						CurrentSnapPosition = snap;
						CurrentPosition = loc;
						CurrentTool.OnMouseUp(this, CurrentSnapPosition);
						return;
					}
				}
				CurrentPosition = loc;
				CurrentTool.OnMouseUp(this, CurrentPosition);

				//CurrentTool.OnMouseUp(this, e);
			}
		}

		void DrawingCanvas_LostMouseCapture(object sender, MouseEventArgs e)
		{
			if (this.IsMouseCaptured)
			{
				CancelCurrentOperation();
			}

			CurrentPosition = new Point(-1, -1);
			CurrentSnapPosition = new Point(-1, -1);
		}

		void DrawingCanvas_MouseLeave(object sender, MouseEventArgs e)
		{
			CurrentUnityPosition = new Point(-1, -1); //DrawingManager.Instance.ConvertToUnity(e.GetPosition(this));
			CurrentPosition = new Point(-1, -1);
			CurrentSnapPosition = new Point(-1, -1);
			//Container.RefreshPosition();
			//Container.RefreshChip();
		}

		void DrawingCanvas_KeyDown(object sender, KeyEventArgs e)
		{
			// Esc key stops currently active operation
			if (e.Key == Key.Escape)
			{
				if (this.IsMouseCaptured)
				{
					CancelCurrentOperation();
				}
			}

			if (e.Key == Key.A)
				HelperFunctions.SelectAll(this);
		}

		void undoManager_StateChanged(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Cancel currently executed operation:
		/// add new object or group selection.
		/// 
		/// Called when mouse capture is lost or Esc is pressed.
		/// </summary>
		void CancelCurrentOperation()
		{
			//if (Tool == ToolType.Pointer)
			//{
			//	if (graphicsList.Count > 0)
			//	{
			//		if (graphicsList[graphicsList.Count - 1] is GraphicsSelectionRectangle)
			//		{
			//			// Delete selection rectangle if it exists
			//			graphicsList.RemoveAt(graphicsList.Count - 1);
			//		}
			//		else
			//		{
			//			// Pointer tool moved or resized graphics object.
			//			// Add this action to the history
			//			toolPointer.AddChangeToHistory(this);
			//		}
			//	}
			//}
			//else if (Tool > ToolType.Pointer && Tool < ToolType.Max)
			//{
			//	// Delete last graphics object which is currently drawn
			//	if (graphicsList.Count > 0)
			//	{
			//		graphicsList.RemoveAt(graphicsList.Count - 1);
			//	}
			//}

			this.ReleaseMouseCapture();
			this.Cursor = Cursors.Arrow;
		}

		/// <summary>
		/// Open in-place edit box if GraphicsText is clicked
		/// </summary>
		void HandleDoubleClick(MouseButtonEventArgs e)
		{
			Point point = e.GetPosition(this);

			// Enumerate all text objects
			for (int i = graphicsList.Count - 1; i >= 0; i--)
			{
				//GraphicsText t = graphicsList[i] as GraphicsText;

				//if ( t != null )
				//{
				//	if ( t.Contains(point) )
				//	{
				//		toolText.CreateTextBox(t, this);
				//		return;
				//	}
				//}
			}
		}

		/// <summary>
		/// Add command to history.
		/// </summary>
		internal void AddCommandToHistory(CommandBase command)
		{
			undoManager.AddCommandToHistory(command);
		}

		/// <summary>
		/// Clear Undo history.
		/// </summary>
		void ClearHistory()
		{
			undoManager.ClearHistory();
		}

		#endregion

		/// <summary>
		/// Set clip for all graphics objects.
		/// </summary>
		public void RefreshClip()
		{
			foreach (GraphicsBase b in graphicsList)
			{
				//b.Clip = new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));

				// Good chance to refresh actual scale
				b.ActualScale = this.ActualScale;
			}
		}

		/// <summary>
		/// Remove clip for all graphics objects.
		/// </summary>
		public void RemoveClip()
		{
			foreach (GraphicsBase b in graphicsList)
			{
				b.Clip = null;
			}
		}
	}
}
