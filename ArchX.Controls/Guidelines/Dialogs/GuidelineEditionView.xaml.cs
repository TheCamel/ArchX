using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArchX.Controls.Guidelines
{
	/// <summary>
	/// Interaction logic for GuidelineEditionView.xaml
	/// </summary>
	public partial class GuidelineEditionView : Window
	{
		public GuidelineEditionView(List<GuideInfo> gl)
		{
			InitializeComponent();

			this.DataContext = this;
			Guides = gl;
		}


		private List<GuideInfo> _Guides = new List<GuideInfo>();
		public List<GuideInfo> Guides
		{
			get { return _Guides; }
			set { _Guides = value; }
		}
	}
}
