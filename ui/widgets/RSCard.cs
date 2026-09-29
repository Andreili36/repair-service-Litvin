using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using RepairService.Application.Theme;

namespace RepairService.Ui.Widgets
{
    public static class RSCard
    {
        public static Border Card(FrameworkElement content) => new()
        {
            Background = ThemeBrushs.Card,
            CornerRadius = ThemeDimensions.Radius,
            Padding = new Thickness(20),
            Child = content,
            Effect = new DropShadowEffect
            {
                BlurRadius = 24,
                ShadowDepth = 0,
                Opacity = 0.25,
                Color = Colors.Black
            }
        };

        public static Border Spacer(double height) => new() { Height = height };
    }
}