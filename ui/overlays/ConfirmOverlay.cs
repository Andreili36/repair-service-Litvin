using System;
using System.Windows;
using System.Windows.Controls;
using RepairService.Application.Theme;
using RepairService.Ui.Widgets;

namespace RepairService.Ui.Overlays
{
    public class ConfirmOverlay : OverlayBase
    {
        private readonly string _text;
        private readonly Action<bool> _onResult;

        public ConfirmOverlay(string id, string text, Action<bool> onResult) : base(id)
        {
            _text = text;
            _onResult = onResult;
        }

        protected override UIElement BuildContent()
        {
            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = _text,
                TextWrapping = TextWrapping.Wrap,
                Foreground = ThemeBrushs.Text,
                FontSize = 14,
                FontFamily = ThemeFonts.Primary,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var no = RSButton.Secondary("Нет");
            no.Margin = new Thickness(0, 0, 8, 0);
            no.Click += (_, _) => { Close(); _onResult(false); };

            var yes = RSButton.Primary("Да");
            yes.Click += (_, _) => { Close(); _onResult(true); };

            buttons.Children.Add(no);
            buttons.Children.Add(yes);
            stack.Children.Add(buttons);

            return stack;
        }
    }
}