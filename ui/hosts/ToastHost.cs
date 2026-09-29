using RepairService.Application.Theme;
using RepairService.Services;
using RepairService.Services.Ui;
using RepairService.Shared.Events;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace RepairService.Ui.Hosts
{
    public class ToastHost : ContentControl
    {
        private readonly StackPanel _stack;

        public ToastHost()
        {
            _stack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20)
            };
            base.Content = _stack;
            IsHitTestVisible = false;

            EventBus.Subscribe<ShowToastEvent>(e => ShowToast(e.Text, e.Severity));
        }

        private void ShowToast(string text, Severity severity)
        {
            var accent = severity switch
            {
                Severity.Info => ThemeBrushs.Info,
                Severity.Error => ThemeBrushs.Danger,
                Severity.Success => ThemeBrushs.Success,
                Severity.Warning => ThemeBrushs.Warning,
                Severity.Contrast => ThemeBrushs.Contrast,
                _ => ThemeBrushs.Secondary
            };

            var accentBg = severity switch
            {
                Severity.Info => ThemeBrushs.InfoBg,
                Severity.Error => ThemeBrushs.DangerBg,
                Severity.Success => ThemeBrushs.SuccessBg,
                Severity.Warning => ThemeBrushs.WarningBg,
                Severity.Contrast => ThemeBrushs.ContrastBg,
                _ => ThemeBrushs.Secondary
            };

            var rootToast = new Grid()
            {
                MaxWidth = 340,
                MinWidth = 200,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var backgroundToast = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(0xD9, 0x0, 0x0, 0x0)),
                CornerRadius = ThemeDimensions.Radius
            };

            var toast = new Border
            {
                Background = accentBg,
                CornerRadius = ThemeDimensions.Radius,
                Padding = new Thickness(16, 12, 16, 12),
                BorderBrush = accent,
                BorderThickness = new Thickness(1),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 24,
                    ShadowDepth = 0,
                    Opacity = 0.25,
                    Color = Colors.Black
                },
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = accent,
                    FontFamily = ThemeFonts.Primary,
                    FontSize = 13,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            rootToast.Children.Add(backgroundToast);
            rootToast.Children.Add(toast);
            _stack.Children.Add(rootToast);

            rootToast.Opacity = 0;
            rootToast.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                fade.Completed += (_, _) => _stack.Children.Remove(rootToast);
                rootToast.BeginAnimation(OpacityProperty, fade);
            };
            timer.Start();
        }
    }
}