using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ArchX.Controls
{
	/// <summary>
	/// A helper class for DIP (Device Independent Pixels) conversion and scaling operations.
	/// </summary>
	public static class DipHelper
	{
		/// <summary>
		/// Converts millimeters to DIP (Device Independant Pixels).
		/// </summary>
		/// <param name="mm">A millimeter value.</param>
		/// <returns>A DIP value.</returns>
		public static double MmToDip(double mm)
		{
			return CmToDip(mm / 10.0);
		}

		/// <summary>
		/// Converts centimeters to DIP (Device Independant Pixels).
		/// </summary>
		/// <param name="cm">A centimeter value.</param>
		/// <returns>A DIP value.</returns>
		public static double CmToDip(double cm)
		{
			return (cm * 96.0 / 2.54);
		}

		/// <summary>
		/// Converts inches to DIP (Device Independant Pixels).
		/// </summary>
		/// <param name="inch">An inch value.</param>
		/// <returns>A DIP value.</returns>
		public static double InchToDip(double inch)
		{
			return (inch * 96.0);
		}

		public static double DipToInch(double dip)
		{
			return dip / 96D;
		}

		/// <summary>
		/// Converts font points to DIP (Device Independant Pixels).
		/// </summary>
		/// <param name="pt">A font point value.</param>
		/// <returns>A DIP value.</returns>
		public static double PtToDip(double pt)
		{
			return (pt * 96.0 / 72.0);
		}

		/// <summary>
		/// Converts DIP (Device Independant Pixels) to centimeters.
		/// </summary>
		/// <param name="dip">A DIP value.</param>
		/// <returns>A centimeter value.</returns>
		public static double DipToCm(double dip)
		{
			return (dip * 2.54 / 96.0);
		}

		/// <summary>
		/// Converts DIP (Device Independant Pixels) to millimeters.
		/// </summary>
		/// <param name="dip">A DIP value.</param>
		/// <returns>A millimeter value.</returns>
		public static double DipToMm(double dip)
		{
			return DipToCm(dip) * 10.0;
		}

		/// <summary>
		/// Gets the system DPI scale factor (compared to 96 dpi).
		/// From http://blogs.msdn.com/jaimer/archive/2007/03/07/getting-system-dpi-in-wpf-app.aspx
		/// Should not be called before the Loaded event (else XamlException mat throw)
		/// </summary>
		/// <returns>A Point object containing the X- and Y- scale factor.</returns>
		private static Point GetSystemDpiFactor()
		{
			PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
			Matrix m = source.CompositionTarget.TransformToDevice;
			return new Point(m.M11, m.M22);
		}

		private const double DpiBase = 96.0;

		/// <summary>
		/// Gets the system configured DPI.
		/// </summary>
		/// <returns>A Point object containing the X- and Y- DPI.</returns>
		public static Point GetSystemDpi()
		{
			Point sysDpiFactor = GetSystemDpiFactor();
			return new Point(
				 sysDpiFactor.X * DpiBase,
				 sysDpiFactor.Y * DpiBase);
		}

		/// <summary>
		/// Gets the physical pixel density (DPI) of the screen.
		/// </summary>
		/// <param name="diagonalScreenSize">Size - in inch - of the diagonal of the screen.</param>
		/// <returns>A Point object containing the X- and Y- DPI.</returns>
		public static Point GetPhysicalDpi(double diagonalScreenSize)
		{
			Point sysDpiFactor = GetSystemDpiFactor();
			double pixelScreenWidth = SystemParameters.PrimaryScreenWidth * sysDpiFactor.X;
			double pixelScreenHeight = SystemParameters.PrimaryScreenHeight * sysDpiFactor.Y;
			double formatRate = pixelScreenWidth / pixelScreenHeight;

			double inchHeight = diagonalScreenSize / Math.Sqrt(formatRate * formatRate + 1.0);
			double inchWidth = formatRate * inchHeight;

			double xDpi = Math.Round(pixelScreenWidth / inchWidth);
			double yDpi = Math.Round(pixelScreenHeight / inchHeight);

			return new Point(xDpi, yDpi);
		}

		/// <summary>
		/// Converts a DPI into a scale factor (compared to system DPI).
		/// </summary>
		/// <param name="dpi">A Point object containing the X- and Y- DPI to convert.</param>
		/// <returns>A Point object containing the X- and Y- scale factor.</returns>
		public static Point DpiToScaleFactor(Point dpi)
		{
			Point sysDpi = GetSystemDpi();
			return new Point(
				 dpi.X / sysDpi.X,
				 dpi.Y / sysDpi.Y);
		}

		/// <summary>
		/// Gets the scale factor to apply to a WPF application
		/// so that 96 DIP always equals 1 inch on the screen (whatever the system DPI).
		/// </summary>
		/// <param name="diagonalScreenSize">Size - in inch - of the diagonal of the screen</param>
		/// <returns>A Point object containing the X- and Y- scale factor.</returns>
		public static Point GetScreenIndependentScaleFactor(double diagonalScreenSize)
		{
			return DpiToScaleFactor(GetPhysicalDpi(diagonalScreenSize));
		}
	}
}
