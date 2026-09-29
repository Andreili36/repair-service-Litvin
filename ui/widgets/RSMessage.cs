using RepairService.Application.Theme;
using RepairService.Services.Ui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace RepairService.Ui.Widgets
{
    public enum Variant { Filled, Outlined, Simple }
    public class RSMessage : Grid
    {
        public RSMessage(bool clossable = false, int life = 0, Variant variant = Variant.Filled)
        {
            _variant = variant;

            HorizontalAlignment = HorizontalAlignment.Center;
            RenderTransform = _transform;

            _life = life > 0 ? TimeSpan.FromMilliseconds(life) : null;

            var messagePanel = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            messagePanel.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            messagePanel.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            _messageRoot.Child = messagePanel;

            if (_variant == Variant.Filled)
            {
                Children.Add(new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(0xD9, 0x0, 0x0, 0x0)),
                    CornerRadius = ThemeDimensions.Radius
                });
            }
            Children.Add(_messageRoot);

            Grid.SetColumn(_message, 0);
            messagePanel.Children.Add(_message);
            if (clossable && _life is null)
            {
                _close.Click += (_, _) => Hide();
                Grid.SetColumn(_close, 1);
                messagePanel.Children.Add(_close);
            }
            else
            {
                _timer.Interval = (TimeSpan)_life;
                _timer.Tick += (_, _) => { _timer.Stop(); Hide(); };
            }

            Visibility = string.IsNullOrEmpty(_message.Text) ? Visibility.Collapsed : Visibility.Visible;
        }

        public void SetMessage(string text, Severity severity = Severity.Info)
        {
            UpdateSeverity(severity);

            _message.Text = text;
        }

        public void Show()
        {
            if (_life is not null)
            {
                _timer.Stop();
                _timer.Start();
            }

            if (Visibility == Visibility.Visible)
                return;

            Visibility = Visibility.Visible;

            _transform.BeginAnimation(TranslateTransform.YProperty, null);
            BeginAnimation(UIElement.OpacityProperty, null);

            _transform.Y = 20;
            Opacity = 0;

            var slide = new DoubleAnimation
            {
                From = 20,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new SineEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            _transform.BeginAnimation(TranslateTransform.YProperty, slide);
            BeginAnimation(UIElement.OpacityProperty, fade);
        }

        public void Hide()
        {
            if (Visibility == Visibility.Collapsed)
                return;

            _transform.BeginAnimation(TranslateTransform.YProperty, null);
            BeginAnimation(UIElement.OpacityProperty, null);

            var slide = new DoubleAnimation
            {
                From = 0,
                To = 20,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseIn
                }
            };

            var fade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new SineEase
                {
                    EasingMode = EasingMode.EaseIn
                }
            };

            fade.Completed += (_, _) =>
            {
                _transform.BeginAnimation(TranslateTransform.YProperty, null);
                BeginAnimation(UIElement.OpacityProperty, null);

                _transform.Y = 20;
                Opacity = 1;

                Visibility = Visibility.Collapsed;
            };


            _transform.BeginAnimation(TranslateTransform.YProperty, slide);
            BeginAnimation(UIElement.OpacityProperty, fade);
        }

        private void UpdateSeverity(Severity severity)
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

            _message.Foreground = accent;
            _close.Foreground = accent;
            if (_variant == Variant.Filled)
            {
                _messageRoot.Background = accentBg;
            }
            if (_variant != Variant.Simple)
            {
                _messageRoot.BorderBrush = accent;
                _messageRoot.BorderThickness = new Thickness(1);
            }
        }

        private readonly TextBlock _message = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            FontFamily = ThemeFonts.Primary,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 10, 0)
        };
        private readonly Button _close = new()
        {
            Content = "✕",
            VerticalAlignment = VerticalAlignment.Center,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            FontFamily = ThemeFonts.Primary,
            FontSize = 13,
            Cursor = Cursors.Hand,
            Template = new ControlTemplate(typeof(Button))
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentPresenter))
            }
        };
        private readonly Border _messageRoot = new()
        {
            CornerRadius = ThemeDimensions.Radius,
            Padding = new Thickness(16, 12, 16, 12)
        };
        private readonly TranslateTransform _transform = new();
        private readonly Variant _variant;
        private readonly DispatcherTimer _timer = new();
        private readonly TimeSpan? _life;
    }
}
