using System;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ArchX.Controls.Guidelines;

namespace ArchX.Controls
{
	public enum RulerOrientation { Vertical, Horizontal, Diagonal };

	public class RulerBar : FrameworkElement
	{
		#region ----------PROPERTIES----------

		protected double SegmentHeight;

		public RulerOrientation Orientation;

		protected readonly SolidColorBrush normalBack = new SolidColorBrush(Colors.White);
		protected readonly SolidColorBrush disableBack = new SolidColorBrush(Colors.LightGray);
		protected readonly Pen BorderPen = new Pen(Brushes.Gray, 0.5);

		protected readonly Pen p = new Pen(Brushes.Black, 1.0);
		protected readonly Pen ThinPen = new Pen(Brushes.Black, 0.5);
		protected readonly Pen RedPen = new Pen(Brushes.Red, 2.0);

		protected Guideline _localGuide;

		public Ruler Container { get { return TemplatedParent as Ruler; } }

		protected const int RULER_MAXSTEP = 11;
		protected double[] m_dCnstLogicStepArray = new double[RULER_MAXSTEP];//logic step of graduation

		protected double[] m_dLogicStepArray = new double[RULER_MAXSTEP];//logic step of graduation
		protected int[] m_dPixStepArray = new int[RULER_MAXSTEP];//pixel step of graduation

		protected int m_nStepCount;			//how much graduation type
		protected int m_nPixelDelta;			//use to test text size = dPixStepTab[k]
		protected double m_dRealStep;			//to calculate = dLogicStepTab[k]
		protected int m_nMidCounter;		//to draw type2 every 5 graduation

		protected Pen Activpen = null;
		protected Color m_crActivColor;
		protected double m_nPixelPos;		//to draw at this pixel position

		#endregion

		#region ----------CONSTRUCTOR----------

		public RulerBar()
		{
			SegmentHeight = this.Height - 10;
			this.ClipToBounds = true;

			this.PreviewMouseLeftButtonDown += Ruler_MouseLeftButtonDown;
			this.PreviewMouseLeftButtonUp += Ruler_MouseLeftButtonUp;
			this.MouseMove += Ruler_MouseMove;

			m_dCnstLogicStepArray[0] = 100000.0;
			m_dCnstLogicStepArray[1] = 10000.0;
			m_dCnstLogicStepArray[2] = 1000.0;
			m_dCnstLogicStepArray[3] = 100.0;
			m_dCnstLogicStepArray[4] = 10.0;
			m_dCnstLogicStepArray[5] = 1.0;
			m_dCnstLogicStepArray[6] = 0.1;
			m_dCnstLogicStepArray[7] = 0.01;
			m_dCnstLogicStepArray[8] = 0.001;
			m_dCnstLogicStepArray[9] = 0.0001;
			m_dCnstLogicStepArray[10] = 0.00001;

		}
		#endregion



		#region ----------METHODS----------

		#region ----------size & render----------
		/// <summary>
		/// Participates in rendering operations.
		/// </summary>
		/// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			//double xDest = (Unit == Unit.Cm ? DipHelper.CmToDip(this.Width) : DipHelper.InchToDip(this.Width)) * 1.0;
			if( Container.IsProtected)
				drawingContext.DrawRectangle(disableBack, null, new Rect(new Point(0.0, 0.0), new Point(this.ActualWidth, this.ActualHeight)));
			else
				drawingContext.DrawRectangle(normalBack, null, new Rect(new Point(0.0, 0.0), new Point(this.ActualWidth, this.ActualHeight)));
		}

		protected int GetFirstSignificantPos(double dRealStep)
		{
			int nCounter = 0;

			string szSig = dRealStep.ToString("f5");	//work with 5 digit

			char[] digit = szSig.ToCharArray();
			int nPoint = szSig.IndexOf('.');

			if (nPoint == -1)
				return 0;
			do
			{
				nPoint++;
				nCounter++;
			}
			while ((digit[nPoint] == '0') && (digit[nPoint] != '\0'));

			if (digit[nPoint] == '\0')
				return 0;
			else
				return nCounter;
		}

		protected FormattedText GetFormatedText(double realX, double realStep)
		{
			string szFormat = "F";

			int nSigni = GetFirstSignificantPos(realStep);

			string szSig = nSigni.ToString();
			szFormat += szSig;

			FormattedText ft = new FormattedText(
						realX.ToString(szFormat, CultureInfo.CurrentCulture),
						 CultureInfo.CurrentCulture,
						 FlowDirection.LeftToRight,
						 new Typeface("Arial"),
						 DipHelper.PtToDip(6),
						 Brushes.DimGray);
			ft.SetFontWeight(FontWeights.Regular);
			ft.TextAlignment = TextAlignment.Center;

			return ft;
		}
		#endregion

		#region ----------Mouse events----------

		void Ruler_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (Container.IsProtected) return;

			//Point p = Container.GetRelativPosition(e);
			CaptureMouse();

			Point pt = e.GetPosition(this);
			_localGuide = GetGuide(pt);
		}

		void Ruler_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (Container.IsProtected) return;

			//Point p = Container.GetRelativPosition(e);
			if (_localGuide != null)
			{
				Point pt = e.GetPosition(this);
				if( PtInDelta(pt))
					Container.MoveGuide(_localGuide, pt);
				else
					Container.MoveGuide(_localGuide, new Point(-110, -110));
			}
			else
			{
				HitTestGuide(e.GetPosition(this));
			}
		}

		void Ruler_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (Container.IsProtected) return;

			if (_localGuide != null)
			{
				ReleaseMouseCapture();

				Point pt = e.GetPosition(this);

				if (PtInDelta(pt))
					Container.FinalizeGuide(_localGuide, pt);
				//else
				_localGuide = null;
			}
		}

		#endregion

		#region ----------hit and test----------

		virtual protected bool PtInDelta(Point p)
		{
			return false;
		}

		private Guideline GetGuide(Point p)
		{
			return this.Container.GuideManager.GetGuide(p, this.Orientation);
		}

		private void HitTestGuide(Point p)
		{
			Cursor = this.Container.GuideManager.HitTestGuide(p, this.Orientation);
		}

		#endregion

		#endregion
	}
}
