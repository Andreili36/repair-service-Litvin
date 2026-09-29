using System.Windows;
using System.Windows.Controls;
using RepairService.Application.Theme;

namespace RepairService.Ui.Widgets
{
    public static class RSInput
    {
        public static TextBox Text(string value = "") => new()
        {
            Text = value,
            Height = 38,
            Padding = new Thickness(12, 8, 12, 8),
            FontSize = 14,
            FontFamily = ThemeFonts.Primary,
            Background = ThemeBrushs.Card,
            Foreground = ThemeBrushs.Text,
            BorderBrush = ThemeBrushs.Border,
            BorderThickness = new Thickness(1),
            CaretBrush = ThemeBrushs.Text,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        public static PasswordBox Password() => new()
        {
            Height = 38,
            Padding = new Thickness(12, 8, 12, 8),
            FontSize = 14,
            FontFamily = ThemeFonts.Primary,
            Background = ThemeBrushs.Card,
            Foreground = ThemeBrushs.Text,
            BorderBrush = ThemeBrushs.Border,
            BorderThickness = new Thickness(1),
            VerticalContentAlignment = VerticalAlignment.Center
        };
    }
}