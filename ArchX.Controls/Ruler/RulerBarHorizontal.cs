using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArchX.Controls
{
	public class RulerBarHorizontal : RulerBar
	{
		#region Constructor

		public RulerBarHorizontal()
		{
			Orientation = RulerOrientation.Horizontal;
			Height = 30;
		}

		#endregion

		#region ----------METHODS----------

		#region ----------size & render----------
		double m_dExtendX;
		double m_dOriginX;
		double m_dResolutionX;
		int m_nPositionX;
		double m_dRealX;

		/// <summary>
		/// Participates in rendering operations.
		/// </summary>
		/// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			m_dExtendX = Container.PageManager.Extend.X;
			m_dOriginX = Container.PageManager.Origin.X;
			m_dResolutionX = Container.PageManager.Resolution.X;

			int m_nStepCount = FindStep(m_dLogicStepArray, m_dPixStepArray);

			for (int k = m_nStepCount - 1; k >= 0; k--)	//from little step to big
			{
				m_nPixelDelta = m_dPixStepArray[k];
				m_dRealStep = m_dLogicStepArray[k];
				m_nMidCounter = 0;

				if (k == 0) //BIG - standard black pen
				{
					m_crActivColor = (IsEnabled ? Color.FromRgb(0, 0, 0) : Colors.Gray);

					Activpen = new Pen(new SolidColorBrush(m_crActivColor), 1.0);
					//m_pOldFont = dc.SelectObject(&m_HFontBig);
				}
				if (k == 1) //MIDDLE
				{
					m_crActivColor = (IsEnabled ? Color.FromRgb(0, 0, 250) : Colors.Gray);

					Activpen = new Pen(new SolidColorBrush(m_crActivColor), 0.6);
					//m_pOldFont = dc.SelectObject(&m_HFontMid);
				}
				if (k == 2) //LITTLE
				{
					m_crActivColor = (IsEnabled ? Color.FromRgb(250, 0, 0) : Colors.Gray);
					Activpen = new Pen(new SolidColorBrush(m_crActivColor), 0.4);
					//m_pOldFont = dc.SelectObject(&m_HFontMid);
				}

				//	m_nPositionX = (RULER_1_6_HEIGHT)+(2+k)*(RULER_1_6_HEIGHT);
				m_nPositionX = Convert.ToInt32((3 + k) * (this.ActualHeight / 6) + 2);

				//FROM ORIGIN TO POSITIF
				m_dRealX = 0.0;
				while (m_dRealX < m_dExtendX + m_dOriginX)
				{
					if (m_dRealX < m_dOriginX)
						goto OutsideViewPlus;

					//perform convertion from double to int for better interpolation
					Container.PageManager.XLogicToDot(ref m_nPixelPos, m_dRealX);

					//test if in the activ view
					if ((m_nPixelPos < 0) && (m_nPixelPos > this.ActualWidth))
						goto OutsideViewPlus;

					if (k == 0)
					{
						FormattedText ft = GetFormatedText(m_dRealX, m_dRealStep);

						if (ft.Width < m_nPixelDelta)
						{
							drawingContext.DrawText(ft, new Point(m_nPixelPos, m_nPositionX - ft.Height));
						}
					}

					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 6)
						{
							FormattedText ft = GetFormatedText(m_dRealX, m_dRealStep);

							if (ft.Width < m_nPixelDelta * 4)
							{
								drawingContext.DrawText(ft, new Point(m_nPixelPos, m_nPositionX - ft.Height));
							}
						}
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}

					drawingContext.DrawLine(Activpen,
						new Point(m_nPixelPos, this.ActualHeight),
						new Point(m_nPixelPos, m_nPositionX));
					goto endPlus;

				OutsideViewPlus:
					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}
				endPlus:
					m_dRealX += m_dRealStep;
				}//while

				m_nMidCounter = 0;

				//FROM ORIGIN TO NEGATIF
				m_dRealX = 0.0;
				while (m_dRealX >= m_dOriginX)
				{
					if (m_dRealX > m_dExtendX + m_dOriginX)
						goto OutsideViewMinus;

					//perform convertion from double to int for better interpolation
					Container.PageManager.XLogicToDot(ref m_nPixelPos, m_dRealX);

					//test if in the activ view
					if ((m_nPixelPos < 0) && (m_nPixelPos > this.ActualWidth))
						goto OutsideViewMinus;

					if (k == 0)
					{
						FormattedText ft = GetFormatedText(m_dRealX, m_dRealStep);

						if (ft.Width < m_nPixelDelta)
						{
							drawingContext.DrawText(ft, new Point(m_nPixelPos, m_nPositionX - ft.Height));
						}
					}

					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 6)
						{
							FormattedText ft = GetFormatedText(m_dRealX, m_dRealStep);

							if (ft.Width < m_nPixelDelta * 4)
							{
								drawingContext.DrawText(ft, new Point(m_nPixelPos, m_nPositionX - ft.Height));
							}
						}
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}

					drawingContext.DrawLine(Activpen,
						new Point(m_nPixelPos, this.ActualHeight),
						new Point(m_nPixelPos, m_nPositionX));
					goto endMinus;

				OutsideViewMinus:
					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}
				endMinus:
					m_dRealX -= m_dRealStep;
				}//while


			}//for(int k 
		}

		private int FindStep(double[] lpLogStepArray, int[] lpPixStepArray)
		{
			double dLogicStep = m_dCnstLogicStepArray[0];
			int nLoop = 0;
			int nCounter = 0;
			int nPixelDelta;

			do
			{
				if (dLogicStep < m_dExtendX)
				{
					nPixelDelta = (int)Math.Ceiling(dLogicStep / m_dResolutionX);

					if (nPixelDelta >= 4)
					{
						lpPixStepArray[nCounter] = nPixelDelta;
						lpLogStepArray[nCounter] = m_dCnstLogicStepArray[nLoop];
						nCounter++;
					}
					else goto TooLittle;
				}
				nLoop++;
				dLogicStep = m_dCnstLogicStepArray[nLoop];
			}
			while (dLogicStep > m_dCnstLogicStepArray[RULER_MAXSTEP - 1] && nLoop < RULER_MAXSTEP);


		TooLittle:

			return nCounter;
		}

		#endregion


		override protected bool PtInDelta(Point p)
		{
			if(p.X > this.Width) return false;
			if(p.X < 1) return false;

			return true;
		}

		#endregion
	}


}
