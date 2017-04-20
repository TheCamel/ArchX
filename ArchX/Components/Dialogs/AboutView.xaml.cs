using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace ArchX.Components.Dialogs
{
	/// <summary>
	/// Interaction logic for AboutView.xaml
	/// </summary>
	public partial class AboutView : Window
	{
		public AboutView()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Assembly EntryAssembly = Assembly.GetEntryAssembly();

			// fill the executable version
			this.labelApplicationVersion.Content = EntryAssembly.GetName().Version.ToString();

			//fill with the dependencies
			this.listBoxAssembliesList.Items.Clear();

			foreach (AssemblyName assembly in EntryAssembly.GetReferencedAssemblies())
			{
				this.listBoxAssembliesList.Items.Add(assembly.Name + ", Version=" + assembly.Version);
			}
		}
	}
}
