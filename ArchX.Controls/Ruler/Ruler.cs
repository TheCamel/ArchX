using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ArchX.Controls.Guidelines;
using ArchX.Controls.Layers;
using GalaSoft.MvvmLight.CommandWpf;

namespace ArchX.Controls
{
	public enum UnitType
	{
		/// <summary>
		/// the unit is Centimeter.
		/// </summary>
		Cm,

		/// <summary>
		/// The unit is Inch.
		/// </summary>
		Inch
	};


	public enum MarkOrientation
	{
		Up, Down
	}

	[TemplatePart(Name = "PART_CRuler", Type = typeof(RulerCorner))]
	[TemplatePart(Name = "PART_HRuler", Type = typeof(RulerBarHorizontal))]
	[TemplatePart(Name = "PART_VRuler", Type = typeof(RulerBarVertical))]
	[TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
	public class Ruler : Control
	{
		#region --------------CONSTRUCTOR--------------
		/// <summary>
		/// Initializes the metadata for the window
		/// </summary>
		public Ruler()
		{
			//FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Ruler),
			//	new FrameworkPropertyMetadata(typeof(Ruler)));
		}

		#endregion

		#region --------------DEPENDENCY PROPERTIES--------------

		#region DrawingSurface

		/// <summary>
		/// DrawingSurface Dependency Property
		/// </summary>
		public static readonly DependencyProperty DrawingSurfaceProperty = DependencyProperty.Register(
			"DrawingSurface", typeof(FrameworkElement), typeof(Ruler),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(OnDrawingSurfaceChanged)));

		/// <summary>
		/// Gets or sets where the marks are shown in the ruler.
		/// </summary>
		public FrameworkElement DrawingSurface
		{
			get { return (FrameworkElement)GetValue(DrawingSurfaceProperty); }
			set { SetValue(DrawingSurfaceProperty, value); }
		}

		private static void OnDrawingSurfaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterDrawingSurfaceChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
		}

		protected void AfterDrawingSurfaceChanged(FrameworkElement before, FrameworkElement after)
		{
			if (before != null)
				before.Loaded -= DrawingSurface_Loaded;

			if (after != null)
				after.Loaded += DrawingSurface_Loaded;
		}

		void DrawingSurface_Loaded(object sender, RoutedEventArgs e)
		{
			//if ((sender as FrameworkElement).IsLoaded && GuideAdorner == null)
			//{
			//	AdornerLayer layer = AdornerLayer.GetAdornerLayer(DrawingSurface);
			//	GuideAdorner = new GuidelineAdorner(this, DrawingSurface);
			//	layer.Add(GuideAdorner);
			//}

			AfterShowGuidesChanged(ShowGuides);
			AfterShowMousePositionChanged(ShowMousePosition);
			AfterIsSnappingChanged(IsSnapping);
		}

		#endregion

		#region CurrentPosition

		private static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register(
				"CurrentPosition", typeof(Point), typeof(Ruler),
				new FrameworkPropertyMetadata(new Point(-1, -1), FrameworkPropertyMetadataOptions.AffectsRender,
					new PropertyChangedCallback(OnCurrentPositionChanged)));

		public Point CurrentPosition
		{
			get
			{
				return (Point)GetValue(CurrentPositionProperty);
			}
			set
			{
				SetValue(CurrentPositionProperty, value);
			}
		}

		private static void OnCurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterCurrentPositionChanged((Point)e.NewValue);
		}

		protected void AfterCurrentPositionChanged(Point surface)
		{
			if (PositionAdorner != null)
				PositionAdorner.InvalidateVisual();
		}

		#endregion

		#region CurrentSnapPosition

		private static readonly DependencyProperty CurrentSnapPositionProperty = DependencyProperty.Register(
				"CurrentSnapPosition", typeof(Point), typeof(Ruler),
				new FrameworkPropertyMetadata(new Point(-1, -1), FrameworkPropertyMetadataOptions.AffectsRender,
					new PropertyChangedCallback(OnCurrentSnapPositionChanged)));

		public Point CurrentSnapPosition
		{
			get
			{
				return (Point)GetValue(CurrentSnapPositionProperty);
			}
			set
			{
				SetValue(CurrentSnapPositionProperty, value);
			}
		}

		private static void OnCurrentSnapPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterCurrentSnapPositionChanged((Point)e.NewValue);
		}

		protected void AfterCurrentSnapPositionChanged(Point surface)
		{
			if (PositionAdorner != null)
				PositionAdorner.InvalidateVisual();
		}

		#endregion

		#region ShowMousePosition

		private static readonly DependencyProperty ShowMousePositionProperty = DependencyProperty.Register(
				"ShowMousePosition", typeof(bool), typeof(Ruler),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender & FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					new PropertyChangedCallback(OnShowMousePositionChanged)));

		public bool ShowMousePosition
		{
			get { return (bool)GetValue(ShowMousePositionProperty); }
			set { SetValue(ShowMousePositionProperty, value); }
		}

		private static void OnShowMousePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterShowMousePositionChanged((bool)e.NewValue);
		}

		protected void AfterShowMousePositionChanged(bool after)
		{
			if (DrawingSurface != null && DrawingSurface.IsLoaded)
			{
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(DrawingSurface);

				if (PositionAdorner == null)
					PositionAdorner = new MousePositionAdorner(this, DrawingSurface);

				if (after)
					layer.Add(PositionAdorner);
				else
					layer.Remove(PositionAdorner);
			}
			//if (PositionAdorner != null)
			//{
			//	PositionAdorner.Visibility = after ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

			//	if (after)
			//		PositionAdorner.InvalidateVisual();
			//}
		}
		#endregion

		#region ShowGuides

		private static readonly DependencyProperty ShowGuidesProperty = DependencyProperty.Register(
				"ShowGuides", typeof(bool), typeof(Ruler),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender & FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					new PropertyChangedCallback(OnShowGuidesChanged)));

		public bool ShowGuides
		{
			get { return (bool)GetValue(ShowGuidesProperty); }
			set { SetValue(ShowGuidesProperty, value); }
		}

		private static void OnShowGuidesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterShowGuidesChanged((bool)e.NewValue);
		}

		protected void AfterShowGuidesChanged(bool after)
		{
			//if (GuideAdorner != null)
			//{
			//	GuideAdorner.Visibility = after ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

			//	if (after)
			//		GuideAdorner.InvalidateVisual();
			//}

			if (DrawingSurface != null && DrawingSurface.IsLoaded)
			{
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(DrawingSurface);

				if (GuideAdorner == null)
					GuideAdorner = new GuidelineAdorner(this, DrawingSurface);

				if (after)
					layer.Add(GuideAdorner);
				else
					layer.Remove(GuideAdorner);
			}
		}


		#endregion

		#region is protected

		private static readonly DependencyProperty IsProtectedProperty = DependencyProperty.Register(
				"IsProtected", typeof(bool), typeof(Ruler),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
					new PropertyChangedCallback(OnIsProtectedChanged)));

		public bool IsProtected
		{
			get
			{
				return (bool)GetValue(IsProtectedProperty);
			}
			set
			{
				SetValue(IsProtectedProperty, value);
			}
		}

		private static void OnIsProtectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterIsProtectedChanged((bool)e.NewValue);
		}

		protected void AfterIsProtectedChanged(bool after)
		{
			if (_hRuler != null)
				_hRuler.InvalidateVisual();
			if (_vRuler != null)
				_vRuler.InvalidateVisual();
			if (_cRuler != null)
				_cRuler.InvalidateVisual();
		}

		#endregion

		#region is snapping

		private static readonly DependencyProperty IsSnappingProperty = DependencyProperty.Register(
				"IsSnapping", typeof(bool), typeof(Ruler),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
					new PropertyChangedCallback(OnIsSnappingChanged)));

		public bool IsSnapping
		{
			get { return (bool)GetValue(IsSnappingProperty); }
			set { SetValue(IsSnappingProperty, value); }
		}

		private static void OnIsSnappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterIsSnappingChanged((bool)e.NewValue);
		}

		protected void AfterIsSnappingChanged(bool after)
		{
			if (DrawingSurface != null && DrawingSurface.IsLoaded)
			{
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(DrawingSurface);

				if (SnapAdorner == null)
					SnapAdorner = new SnapPositionAdorner(this, DrawingSurface);
				
				if( after )
					layer.Add(SnapAdorner);
				else
					layer.Remove(SnapAdorner);
			}
		}

		#endregion

		#region MarksLocation

		/// <summary>
		/// ShowMarks Dependency Property
		/// </summary>
		public static readonly DependencyProperty MarksLocationProperty =
			DependencyProperty.Register("MarksLocation", typeof(MarkOrientation), typeof(Ruler),
				  new FrameworkPropertyMetadata(MarkOrientation.Down, FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		/// Gets or sets where the marks are shown in the ruler.
		/// </summary>
		public MarkOrientation MarksLocation
		{
			get { return (MarkOrientation)GetValue(MarksLocationProperty); }
			set { SetValue(MarksLocationProperty, value); }
		}

		#endregion

		#region Zoom

		/// <summary>
		/// Identifies the Zoom dependency property.
		/// </summary>
		public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register("Zoom", typeof(double), typeof(Ruler),
			new FrameworkPropertyMetadata((double)1.0, FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		/// Gets or sets the zoom factor for the ruler. The default value is 1.0. 
		/// </summary>
		public double Zoom
		{
			get
			{
				return (double)GetValue(ZoomProperty);
			}
			set
			{
				SetValue(ZoomProperty, value);
				RefreshGuides();
				this.InvalidateVisual();
			}
		}

		#endregion

		#region Unit

		/// <summary>
		/// Identifies the Unit dependency property.
		/// </summary>
		public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(UnitType), typeof(Ruler),
				  new FrameworkPropertyMetadata(UnitType.Cm, FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		/// Gets or sets the unit of the ruler.
		/// Default value is Unit.Cm.
		/// </summary>
		public UnitType Unit
		{
			get { return (UnitType)GetValue(UnitProperty); }
			set { SetValue(UnitProperty, value); }
		}

		#endregion

		#region Active surface

		private static readonly DependencyProperty ActiveSurfaceProperty = DependencyProperty.Register(
				"ActiveSurface", typeof(Rect), typeof(Ruler),
				new FrameworkPropertyMetadata(new Rect(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender,
					new PropertyChangedCallback(OnActiveSurfaceChanged)));

		public Rect ActiveSurface
		{
			get
			{
				return (Rect)GetValue(ActiveSurfaceProperty);
			}
			set
			{
				SetValue(ActiveSurfaceProperty, value);
			}
		}

		private static void OnActiveSurfaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;

			Ruler element = d as Ruler;
			element.AfterActiveSurfaceChanged((Rect)e.NewValue);
		}

		protected void AfterActiveSurfaceChanged(Rect after)
		{
			PageManager.SetPage(after.Left, after.Top, after.Width, after.Height);
			_hRuler.InvalidateVisual();
			_vRuler.InvalidateVisual();
			RefreshGuides();
		}
		#endregion

		#endregion

		#region ----------------COMMANDS----------------

		#region edit guides command
		private ICommand editGuidesCommand;
		public ICommand EditGuidesCommand
		{
			get
			{
				if (editGuidesCommand == null)
					editGuidesCommand = new RelayCommand(ExecuteEditGuidesCommand, CanExecuteEditGuidesCommand);
				return editGuidesCommand;
			}
		}

		private void ExecuteEditGuidesCommand()
		{
			GuidelineEditionView gle = new GuidelineEditionView(GuideManager.Guides.Select(p => p.Info).ToList());
			gle.ShowDialog();
		}

		private bool CanExecuteEditGuidesCommand()
		{
			return GuideManager.Guides.Count > 0;
		}

		#endregion

		#endregion

		#region -------------PROPERTIES--------------

		public double dPicCapture = 20;

		private LayerItemManager _LayerManager = null;
		public LayerItemManager LayerManager
		{
			get
			{
				if (_LayerManager == null) _LayerManager = new LayerItemManager(this);
				return _LayerManager;
			}
		}

		private GuideLineManager _GuideManager = null;
		public GuideLineManager GuideManager
		{
			get
			{
				if (_GuideManager == null) _GuideManager = new GuideLineManager(this);
				return _GuideManager;
			}
		}

		public PagingManager PageManager { get; private set; }

		internal SnapPositionAdorner SnapAdorner { get; private set; }
		internal MousePositionAdorner PositionAdorner { get; private set; }
		internal GuidelineAdorner GuideAdorner { get; private set; }
		internal GuideAnchorAdorner HorizontalAnchorAdorner { get; private set; }
		internal GuideAnchorAdorner VerticalAnchorAdorner { get; private set; }

		private RulerBar _hRuler = null;
		private RulerBar _vRuler = null;
		private RulerCorner _cRuler = null;
		private ScrollViewer _ScrollContainer;

		#endregion

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			//	return;

			this.SizeChanged += Ruler_SizeChanged;
			_ScrollContainer = (ScrollViewer)GetTemplateChild("PART_ScrollViewer");

			_hRuler = (RulerBar)GetTemplateChild("PART_HRuler");
			_vRuler = (RulerBar)GetTemplateChild("PART_VRuler");
			_cRuler = (RulerCorner)GetTemplateChild("PART_CRuler");

			AdornerLayer layer = AdornerLayer.GetAdornerLayer(_hRuler);
			HorizontalAnchorAdorner = new GuideAnchorAdorner(this, _hRuler);
			layer.Add(HorizontalAnchorAdorner);

			layer = AdornerLayer.GetAdornerLayer(_vRuler);
			VerticalAnchorAdorner = new GuideAnchorAdorner(this, _vRuler);
			layer.Add(VerticalAnchorAdorner);

			if (_ScrollContainer != null)
				_ScrollContainer.Content = DrawingSurface;

			PageManager = new PagingManager();
			PageManager.DrawingSurface = DrawingSurface;
		}

		void Ruler_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_hRuler.InvalidateVisual();
			_vRuler.InvalidateVisual();
			RefreshGuides();
		}

		#region ----------refresh----------

		public void RefreshGuides()
		{
			if (GuideAdorner != null)
				GuideAdorner.InvalidateVisual();
			if (HorizontalAnchorAdorner != null)
				HorizontalAnchorAdorner.InvalidateVisual();
			if (VerticalAnchorAdorner != null)
				VerticalAnchorAdorner.InvalidateVisual();
		}

		#endregion

		#region ----------guidelines----------

		private bool? oldShowMousePosition = null;

		public Guideline CreateGuide(Point p, RulerOrientation typ)
		{
			Guideline local;
			switch (typ)
			{
				case RulerOrientation.Horizontal:
					local = new GuidelineVertical(this);
					break;
				case RulerOrientation.Vertical:
					local = new GuidelineHorizontal(this);
					break;
				case RulerOrientation.Diagonal:
					local = new GuidelineDiagonal(this);
					local.Info.Angle = 45;
					break;
				default:
					return null;
			}

			if (!oldShowMousePosition.HasValue)
			{
				oldShowMousePosition = ShowMousePosition;
				ShowMousePosition = false;
			}

			local.SetPixel(p);
			GuideManager.Guides.Add(local);
			RefreshGuides();

			return local;
		}

		public void MoveGuide(Guideline line, Point p)
		{
			if (!oldShowMousePosition.HasValue)
			{
				oldShowMousePosition = ShowMousePosition;
				ShowMousePosition = false;
			}

			line.SetPixel(p);
			RefreshGuides();
		}

		public void FinalizeGuide(Guideline line, Point p)
		{
			line.SetPixel(p);
			GuideManager.Intersection_Calculate();
			RefreshGuides();

			if (oldShowMousePosition.HasValue)
			{
				ShowMousePosition = oldShowMousePosition.Value;
				oldShowMousePosition = null;
			}
		}
		#endregion

		#region ----------mouse and cursors----------



		#endregion

		#region ----------layers----------

		public void LayerAdd( LayerItem item )
		{
			LayerManager.Add(item);
		}

		public void LayerRemove(LayerItem item)
		{
			LayerManager.Remove(item);
		}

		public void LayerSelect(LayerItem item)
		{
			LayerManager.Select(item);
		}
		public void LayerModify(LayerItem item)
		{
			LayerManager.Modify(item);
		}

		#endregion

		#region ----------helpers----------

		public Point GetRelativPosition(System.Windows.Input.MouseEventArgs e)
		{
			return e.GetPosition(DrawingSurface);
		}

		public double ConvertToUnity(double p)
		{
			return Unit == UnitType.Cm ? DipHelper.DipToCm(p) : DipHelper.DipToInch(p);
		}

		public Point ConvertToUnity(Point p)
		{
			return new Point()
			{
				X = Unit == UnitType.Cm ? DipHelper.DipToCm(p.X) : DipHelper.DipToInch(p.X),
				Y = Unit == UnitType.Cm ? DipHelper.DipToCm(p.Y) : DipHelper.DipToInch(p.Y),
			};
		}

		#endregion
	}
}
