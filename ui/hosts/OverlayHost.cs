using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using RepairService.Services;
using RepairService.Shared.Events;
using RepairService.Ui.Overlays;

namespace RepairService.Ui.Hosts
{
    public class OverlayHost : ContentControl
    {
        private readonly Grid _root = new();
        private readonly ContentPresenter _content = new();
        private readonly Grid _overlayLayer = new();

        public OverlayHost()
        {
            _root.Children.Add(_content);
            _root.Children.Add(_overlayLayer);
            _overlayLayer.Visibility = Visibility.Collapsed;

            base.Content = _root;

            EventBus.Subscribe<ShowMessageEvent>(e =>
                Show(new MessageOverlay(e.id, e.Text, e.Severity)));

            EventBus.Subscribe<ShowConfirmEvent>(e =>
                Show(new ConfirmOverlay(e.id, e.Text, e.OnResult)));

            EventBus.Subscribe<ShowActionEvent>(e =>
                Show(new ActionOverlay(e.id, e.Request)));

            EventBus.Subscribe<OverlayCompletedEvent>(e => Hide(e.Overlay));
        }

        public new object? Content
        {
            get => _content.Content;
            set => _content.Content = value;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            _content.Content = newContent;
        }

        private void Show(OverlayBase overlay)
        {
            var existChildOverlay = _overlayLayer.Children.OfType<OverlayBase>().FirstOrDefault(o => o.Id == overlay.Id);
            if (existChildOverlay is not null && existChildOverlay.Id == overlay.Id) return;

            overlay.Opacity = 0;
            _overlayLayer.Children.Add(overlay);
            _overlayLayer.Visibility = Visibility.Visible;
            overlay.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)));
        }

        private void Hide(OverlayBase overlay)
        {
            if (!_overlayLayer.Children.Contains(overlay)) return;

            var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(120));
            anim.Completed += (_, _) =>
            {
                _overlayLayer.Children.Remove(overlay);
                if (_overlayLayer.Children.Count == 0)
                    _overlayLayer.Visibility = Visibility.Collapsed;
            };
            overlay.BeginAnimation(OpacityProperty, anim);
        }
    }
}