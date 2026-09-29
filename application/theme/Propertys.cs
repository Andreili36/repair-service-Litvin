using System.Windows;
using System.Windows.Media;

namespace RepairService.Application.Theme
{
    public static class ThemeBrushs
    {
        public static readonly Brush Bg = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x2E));
        public static readonly Brush Surface = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x36));
        public static readonly Brush Card = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x3E));
        public static readonly Brush Border = new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x50));
        public static readonly Brush Text = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xEA));
        public static readonly Brush TextDim = new SolidColorBrush(Color.FromRgb(0x90, 0x90, 0xA8));
        public static readonly Brush Primary = new SolidColorBrush(ThemeColors.Primary);
        public static readonly Brush PrimaryBg = new SolidColorBrush(
            Color.FromArgb(0x40, ThemeColors.Primary.R, ThemeColors.Primary.G, ThemeColors.Primary.B));
        public static readonly Brush PrimaryHov = new SolidColorBrush(Color.FromRgb(0x6B, 0xA0, 0xFF));

        public static readonly Brush Info = new SolidColorBrush(ThemeColors.Info);
        public static readonly Brush InfoBg = new SolidColorBrush(
            Color.FromArgb(0x40, ThemeColors.Info.R, ThemeColors.Info.G, ThemeColors.Info.B));

        public static readonly Brush Success = new SolidColorBrush(ThemeColors.Success);
        public static readonly Brush SuccessBg = new SolidColorBrush(
            Color.FromArgb(0x40, ThemeColors.Success.R, ThemeColors.Success.G, ThemeColors.Success.B));

        public static readonly Brush Warning = new SolidColorBrush(ThemeColors.Warning);
        public static readonly Brush WarningBg = new SolidColorBrush(
            Color.FromArgb(0x40, ThemeColors.Warning.R, ThemeColors.Warning.G, ThemeColors.Warning.B));

        public static readonly Brush Danger = new SolidColorBrush(ThemeColors.Danger);
        public static readonly Brush DangerBg = new SolidColorBrush(
            Color.FromArgb(0x40, ThemeColors.Danger.R, ThemeColors.Danger.G, ThemeColors.Danger.B));

        public static readonly Brush Secondary = new SolidColorBrush(ThemeColors.Secondary);
        public static readonly Brush SecondaryBg = new SolidColorBrush(
            Color.FromArgb(0x40, ThemeColors.Secondary.R, ThemeColors.Secondary.G, ThemeColors.Secondary.B));

        public static readonly Brush Contrast = new SolidColorBrush(Colors.Black);
        public static readonly Brush ContrastBg = new SolidColorBrush(ThemeColors.Contrast);
    }

    public static class ThemeColors
    {
        public static readonly Color Primary = Color.FromRgb(0x4F, 0x8C, 0xFF);
        public static readonly Color Info = Color.FromRgb(0x38, 0xBD, 0xF8);
        public static readonly Color Success = Color.FromRgb(0x4A, 0xDE, 0x80);
        public static readonly Color Warning = Color.FromRgb(0xFB, 0x92, 0x3C);
        public static readonly Color Danger = Color.FromRgb(0xF8, 0x71, 0x71);
        public static readonly Color Secondary = Color.FromRgb(0xA1, 0xA1, 0xAA);
        public static readonly Color Contrast = Color.FromRgb(0xFF, 0xFF, 0xFF);
    }

    public static class ThemeFonts
    {
        public static readonly FontFamily Primary = new("Segoe UI");
    }

    public static class ThemeDimensions
    {
        public static readonly CornerRadius Radius = new(10);
    }
}