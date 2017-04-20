using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArchX.Controls
{
	public class RulerBarVertical : RulerBar
	{
		#region Constructor

		public RulerBarVertical()
		{
			Orientation = RulerOrientation.Vertical;
			Width = 30;
		}

		#endregion

		#region ----------METHODS----------

		#region ----------size & render----------
		double m_dExtendY;
		double m_dOriginY;
		double m_dResolutionY;
		int m_nPositionX;
		double m_dRealY;
		
		/// <summary>
		/// Participates in rendering operations.
		/// </summary>
		/// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			m_dExtendY = Container.PageManager.Extend.Y;
			m_dOriginY = Container.PageManager.Origin.Y;
			m_dResolutionY = Container.PageManager.Resolution.Y;

			//------------------------------

			int m_nStepCount = FindStep(m_dLogicStepArray, m_dPixStepArray);

			//--------------

			for (int k = m_nStepCount - 1; k >= 0; k--)	//from little step to big
			{
				m_nPixelDelta = m_dPixStepArray[k];
				m_dRealStep = m_dLogicStepArray[k];
				m_nMidCounter = 0;

				if (k == 0) //BIG - standard black pen
				{
					m_crActivColor = (IsEnabled ? Color.FromRgb(0, 0, 0) : Colors.Gray);

					Activpen = new Pen(new SolidColorBrush(m_crActivColor), 1.0);
				}
				if (k == 1) //MIDDLE
				{
					m_crActivColor = (IsEnabled ? Color.FromRgb(0, 0, 250) : Colors.Gray);

					Activpen = new Pen(new SolidColorBrush(m_crActivColor), 0.5);
				}
				if (k == 2) //LITTLE
				{
					m_crActivColor = (IsEnabled ? Color.FromRgb(250, 0, 0) : Colors.Gray);
					Activpen = new Pen(new SolidColorBrush(m_crActivColor), 0.4);
				}

				//		m_nPositionX = (RULER_1_6_HEIGHT)+(2+k)*(RULER_1_6_HEIGHT);
				m_nPositionX = Convert.ToInt32( (3 + k) * (ActualWidth / 6) + 2);

				//FROM ORIGIN TO POSITIF
				m_dRealY = 0.0;

				while (m_dRealY < m_dExtendY +  m_dOriginY)
				{
					if (m_dRealY < m_dOriginY)
						goto OutsideViewPlus;

					//perform convertion from double to int for better interpolation
					Container.PageManager.YLogicToDot(ref m_nPixelPos, m_dRealY);

					//test if in the activ view
					if ((m_nPixelPos < 0) || (m_nPixelPos > ActualHeight))
						goto OutsideViewPlus;

					if (k == 0)
					{
						FormattedText ft = GetFormatedText(m_dRealY, m_dRealStep);

						if (ft.Width < m_nPixelDelta)
						{
							drawingContext.PushTransform(new RotateTransform(-90.0, m_nPositionX - ft.Width, m_nPixelPos));
							drawingContext.DrawText(ft, new Point(m_nPositionX - ft.Width, m_nPixelPos));
							drawingContext.Pop();
						}
					}

					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 6)
						{
							FormattedText ft = GetFormatedText(m_dRealY, m_dRealStep);

							if (ft.Width < m_nPixelDelta * 4)
							{
								drawingContext.PushTransform(new RotateTransform(-90.0, m_nPositionX - ft.Width, m_nPixelPos));
								drawingContext.DrawText(ft, new Point(m_nPositionX - ft.Width, m_nPixelPos));
								drawingContext.Pop();
							}
						}
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}

					drawingContext.DrawLine(Activpen,
						new Point(ActualWidth, m_nPixelPos),
						new Point(m_nPositionX, m_nPixelPos));
					goto endPlus;

				OutsideViewPlus:
					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}
				endPlus:
					m_dRealY += m_dRealStep;
				}//while

				m_nMidCounter = 0;

				//FROM ORIGIN TO NEGATIF
				m_dRealY = 0.0;
				while (m_dRealY >= m_dOriginY - m_dExtendY)
				{
					if (m_dRealY > m_dExtendY + m_dOriginY)
						goto OutsideViewMinus;

					//perform convertion from double to int for better interpolation
					Container.PageManager.YLogicToDot(ref m_nPixelPos, m_dRealY);

					//test if in the activ view
					if ((m_nPixelPos < 0) && (m_nPixelPos > ActualWidth))
						goto OutsideViewMinus;

					if (k == 0)
					{
						FormattedText ft = GetFormatedText(m_dRealY, m_dRealStep);

						if (ft.Width < m_nPixelDelta)
						{
							drawingContext.PushTransform(new RotateTransform(-90.0, m_nPositionX - ft.Width, m_nPixelPos));
							drawingContext.DrawText(ft, new Point(m_nPositionX - ft.Width, m_nPixelPos ));
							drawingContext.Pop();
						}
					}

					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 6)
						{
							FormattedText ft = GetFormatedText(m_dRealY, m_dRealStep);

							if (ft.Width < m_nPixelDelta * 4)
							{
								drawingContext.PushTransform(new RotateTransform(-90.0, m_nPositionX - ft.Width, m_nPixelPos));
								drawingContext.DrawText(ft, new Point(m_nPositionX - ft.Width, m_nPixelPos));
								drawingContext.Pop();
							}
						}
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}

					drawingContext.DrawLine(Activpen,
						new Point(ActualWidth, m_nPixelPos),
						new Point(m_nPositionX, m_nPixelPos));
					goto endMinus;

				OutsideViewMinus:
					if (k == 1)
					{
						m_nMidCounter++;
						if (m_nMidCounter == 10)
							m_nMidCounter = 0;
					}
				endMinus:
					m_dRealY -= m_dRealStep;
				}//while

			} //for(int k 
		}

		private int FindStep(double[] lpLogStepArray, int[] lpPixStepArray)
		{
			double dLogicStep = m_dCnstLogicStepArray[0];
			int nLoop = 0;
			int nCounter = 0;
			int nPixelDelta;

			do
			{
				if (dLogicStep < m_dExtendY)
				{
					nPixelDelta = (int)Math.Ceiling(dLogicStep / m_dResolutionY);

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
			if (p.Y > this.Height) return false;
			if (p.Y < 1) return false;

			return true;
		}

		#endregion
	}

	
}
