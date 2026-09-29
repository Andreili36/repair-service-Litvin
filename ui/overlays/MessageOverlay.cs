using System.Windows;
using System.Windows.Controls;
using RepairService.Application.Theme;
using RepairService.Services.Ui;
using RepairService.Shared.Events;
using RepairService.Ui.Widgets;

namespace RepairService.Ui.Overlays
{
    public class MessageOverlay : OverlayBase
    {
        private readonly string _text;
        private readonly Severity _severity;

        public MessageOverlay(string id, string text, Severity severity) : base(id)
        {
            _text = text;
            _severity = severity;
        }

        protected override UIElement BuildContent()
        {
            var accent = _severity switch
            {
                Severity.Info => ThemeBrushs.Info,
                Severity.Error => ThemeBrushs.Danger,
                Severity.Success => ThemeBrushs.Success,
                Severity.Warning => ThemeBrushs.Warning,
                Severity.Contrast => ThemeBrushs.Contrast,
                _ => ThemeBrushs.Secondary
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = _text,
                TextWrapping = TextWrapping.Wrap,
                Foreground = accent,
                FontSize = 14,
                FontFamily = ThemeFonts.Primary,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var ok = RSButton.Primary("ОК");
            ok.HorizontalAlignment = HorizontalAlignment.Center;
            ok.Click += (_, _) => Close();
            stack.Children.Add(ok);

            return stack;
        }
    }
}