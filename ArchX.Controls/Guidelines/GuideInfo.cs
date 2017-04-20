using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchX.Controls.Guidelines
{
	public enum GuideOrientation { Horizontal, Vertical, Diagonal };

	public class GuideInfo
	{
		public double RealPositionX = 0;
		public double RealPositionXProperty { get { return RealPositionX; } set { RealPositionX = value; } }

		public double RealPositionY = 0;
		public double RealPositionYProperty { get { return RealPositionY; } set { RealPositionY = value; } }

		public double Angle = 0;
		public double AngleProperty { get { return Angle; } set { Angle = value; } }

		public GuideOrientation Orientation { get; set; }

		public bool IsVisible { get; set; }
		public bool IsLocked { get; set; }
		public bool IsSnap { get; set; }
		public bool IsMoving { get; internal set; }

		public int LayerID{ get; set; }

	}
}
