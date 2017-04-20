using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArchX.Controls.Guidelines
{
	public class GuideLineManager
	{
		#region --------------CONSTRUCTOR--------------
		internal GuideLineManager(Ruler parent)
		{
			_Container = parent;
		}

		#endregion

		#region --------------PROPERTIES--------------

		private List<Guideline> _Guides = new List<Guideline>();
		internal List<Guideline> Guides
		{
			get { return _Guides; }
			set { _Guides = value; }
		}

		private Ruler _Container;

		#endregion

		#region --------------METHODS--------------

		#region ----------hit and test----------

		public Guideline GetGuide(Point p, RulerOrientation mode)
		{
			if (_Guides.Exists(g => (int)g.Info.Orientation == (int)mode && g.HitTest(p)))
			{
				return _Guides.First(g => (int)g.Info.Orientation == (int)mode && g.HitTest(p));
			}

			return _Container.CreateGuide(p, mode);
		}

		public Cursor HitTestGuide(Point p, RulerOrientation mode)
		{
			if (_Guides.Exists(g => (int)g.Info.Orientation == (int)mode && g.HitTest(p)))
			{
				return _Guides.First(g => (int)g.Info.Orientation == (int)mode && g.HitTest(p)).Cursor;
			}

			return Cursors.Arrow;
		}

		#endregion

		//		BOOL CItemManager::GetSnapPosition( TGE_Vektor& RealVector )
		//{
		//	CRulerItem *pt;
		//	int i = 0;

		//	if(Intersection_GetNearst(RealVector))
		//		return TRUE;

		//	m_nArraySize = m_ItemsArray.GetSize();

		//	while (i < m_nArraySize)
		//	{ 
		//		pt = (CRulerItem*)m_ItemsArray.GetAt( i++ );

		//		if( !pt->IsDisplayed()) continue;

		//		if(pt->IsSnap() && !pt->IsMoving())
		//			if( pt->IsOnGuide(RealVector, m_pRuler->m_pRulerInfo->dPixCapture) )
		//			{
		//				pt->GetNearestPos(RealVector, m_pRuler->m_pRulerInfo->dPixCapture);
		//				return TRUE;
		//			}
		//	}
		//	return FALSE;
		//}

		public Guideline GetSnapGuide(Point hitPoint)
		{
			foreach (Guideline gl in Guides)
			{
				if (!gl.IsDisplayed) continue;

				if (gl.Info.IsSnap && !gl.Info.IsMoving)
					if (gl.IsOnGuide(hitPoint, _Container.dPicCapture))
					{
						return gl;
					}
			}
			return null;
		}

		#endregion

		#region ----------intersections ----------

		private List<Vector> _Intersections = new List<Vector>();

		public void Intersection_Calculate()
		{
			_Intersections.Clear();

			foreach (Guideline glMain in this._Guides)
			{
				if (glMain.Info.IsMoving) continue;
				if (!glMain.Info.IsSnap) continue;
				if (!glMain.IsDisplayed) continue;

				if (glMain.Info.Orientation == GuideOrientation.Horizontal)
					foreach (Guideline subgl in this._Guides)
					{
						if (subgl == glMain) continue;
						if (subgl.Info.IsMoving) continue;
						if (!subgl.Info.IsSnap) continue;
						if (!subgl.IsDisplayed) continue;

						if (subgl.Info.Orientation == GuideOrientation.Vertical)
							Intersection_Add(subgl.Info.RealPositionX, glMain.Info.RealPositionY);

						if (subgl.Info.Orientation == GuideOrientation.Diagonal)
							Intersection_Add(subgl.GetXfor(glMain.Info.RealPositionY), glMain.Info.RealPositionY);
					}

				if (glMain.Info.Orientation == GuideOrientation.Vertical)

					foreach (Guideline subgl in this._Guides)
					{
						if (subgl == glMain) continue;
						if (subgl.Info.IsMoving) continue;
						if (!subgl.Info.IsSnap) continue;
						if (!subgl.IsDisplayed) continue;

						if (subgl.Info.Orientation == GuideOrientation.Diagonal)
							Intersection_Add(glMain.Info.RealPositionX, subgl.GetYfor(glMain.Info.RealPositionX));
					}

				if (glMain.Info.Orientation == GuideOrientation.Diagonal)

					foreach (Guideline subgl in this._Guides)
					{
						if (subgl == glMain) continue;
						if (subgl.Info.IsMoving) continue;
						if (!subgl.Info.IsSnap) continue;
						if (!subgl.IsDisplayed) continue;

						if (subgl.Info.Orientation == GuideOrientation.Diagonal)
						{
							GuidelineDiagonal glm = glMain as GuidelineDiagonal;
							GuidelineDiagonal gls = subgl as GuidelineDiagonal;
							double x;
							double DeltaA;
							double DeltaB;
							DeltaA = glm.CoefA - gls.CoefA;
							DeltaB = gls.CoefB - glm.CoefB;
							if (DeltaA > 0)
								x = DeltaB / DeltaA;
							else
								continue;
							Intersection_Add(x, subgl.GetYfor(x));
						}
					}
			}
		}

		private void Intersection_Add(double X, double Y)
		{
			Vector lpt = new Vector(X, Y);
			_Intersections.Add(lpt);
		}

		public bool Intersection_GetNearst(ref Point realVector)
		{
			double dDelta = 0;
			double dX = 0;
			double dY= 0;

			_Container.PageManager.XDotToLogic(realVector.X, ref dX);
			_Container.PageManager.YDotToLogic(realVector.Y, ref dY);
			_Container.PageManager.YDotToLogicLength(_Container.dPicCapture, ref dDelta);

			foreach (Vector vec in _Intersections)
			{
				if (
					(dX < vec.X + dDelta) &&
					(dX > vec.X - dDelta) &&
					(dY < vec.Y + dDelta) &&
					(dY > vec.Y - dDelta))
				{
					_Container.PageManager.XLogicToDot(ref dX, vec.X);
					_Container.PageManager.YLogicToDot(ref dY, vec.Y);
					realVector.X = dX;
					realVector.Y = dY;
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}
