using RepairService.Application.Theme;
using RepairService.Services;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace RepairService.Ui.Overlays
{
    public abstract class OverlayBase : Grid
    {
        private const double CardVerticalMargin = 40;

        private Border? _card;

        public string Id { get; }

        protected OverlayBase(string id)
        {
            Id = id;

            Children.Add(new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(0xAA, 0, 0, 0))
            });

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _card = new Border
            {
                Background = ThemeBrushs.Card,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(24),
                MinWidth = 320,
                MaxWidth = 460,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Effect = new DropShadowEffect
                {
                    BlurRadius = 24,
                    ShadowDepth = 0,
                    Opacity = 0.25,
                    Color = Colors.Black
                }
            };
            _card.Child = BuildContent();
            Children.Add(_card);

            UpdateCardMaxHeight();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCardMaxHeight();
        }

        private void UpdateCardMaxHeight()
        {
            if (_card is null || ActualHeight <= 0) return;
            _card.MaxHeight = Math.Max(200, ActualHeight - CardVerticalMargin * 2);
        }

        protected abstract UIElement BuildContent();

        protected void Close() => EventBus.Emit(new OverlayCompletedEvent(this));
    }
}