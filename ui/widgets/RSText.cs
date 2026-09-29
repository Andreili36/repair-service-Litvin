using System.Windows;
using System.Windows.Controls;
using RepairService.Application.Theme;

namespace RepairService.Ui.Widgets
{
    public static class RSText
    {
        public static TextBlock H1(string text) => new()
        {
            Text = text,
            FontSize = 26,
            FontWeight = FontWeights.SemiBold,
            Foreground = ThemeBrushs.Text,
            FontFamily = ThemeFonts.Primary,
            Margin = new Thickness(0, 0, 0, 16)
        };

        public static TextBlock H2(string text) => new()
        {
            Text = text,
            FontSize = 15,
            FontWeight = FontWeights.SemiBold,
            Foreground = ThemeBrushs.Text,
            FontFamily = ThemeFonts
.Primary
        };

        public static TextBlock Label(string text) => new()
        {
            Text = text,
            FontSize = 13,
            Foreground = ThemeBrushs.TextDim,
            FontFamily = ThemeFonts.Primary,
            Margin = new Thickness(0, 0, 0, 6)
        };

        public static TextBlock Dim(string text) => new()
        {
            Text = text,
            FontSize = 12,
            Foreground = ThemeBrushs.TextDim,
            FontFamily = ThemeFonts
.Primary
        };
    }
}