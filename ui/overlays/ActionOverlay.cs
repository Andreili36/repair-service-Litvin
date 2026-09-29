using RepairService.Application.Theme;
using RepairService.Services.Ui;
using RepairService.Shared.Contracts;
using RepairService.Ui.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace RepairService.Ui.Overlays
{
    public class ActionOverlay : OverlayBase
    {
        private readonly ActionRequest _request;
        private readonly Dictionary<string, Func<object?>> _getters = new();
        private readonly Dictionary<string, (Control Element, Func<bool> Check)> _mandatoryChecks = new();
        private readonly List<(ComboBox combo, FormField field)> _combos = new();
        private readonly RSMessage _message = new(true);

        public ActionOverlay(string id, ActionRequest request) : base(id)
        {
            _request = request;
            Loaded += async (_, _) => await PreloadCombosAsync();
        }

        protected override UIElement BuildContent()
        {
            var outer = new Grid();
            outer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            outer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            outer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            outer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var title = RSText.H1(_request.Title);
            title.TextWrapping = TextWrapping.Wrap;
            title.Margin = new Thickness(0, 0, 0, 16);
            Grid.SetRow(title, 0);
            outer.Children.Add(title);

            var fieldsGrid = BuildFieldsGrid();
            var scroll = RSScroll.Vertical(fieldsGrid);
            scroll.Padding = new Thickness(0, 0, 8, 0);
            Grid.SetRow(scroll, 1);
            outer.Children.Add(scroll);

            Grid.SetRow(_message, 2);
            outer.Children.Add(_message);

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 12, 0, 0)
            };

            var cancel = RSButton.Secondary("Отмена");
            cancel.Margin = new Thickness(0, 0, 8, 0);
            cancel.Click += (_, _) => ActionEvent(new CloseResult());

            var submit = RSButton.Primary("Сохранить");
            submit.Click += (_, _) =>
            {
                bool hasMandatoryEmptys = false;
                foreach (var field in _request.Fields.Where(f => f.IsMandatory))
                {
                    if (_mandatoryChecks.TryGetValue(field.Key, out var mandatoryCheck) && !mandatoryCheck.Check())
                    {
                        mandatoryCheck.Element.BorderBrush = ThemeBrushs.Danger;
                        hasMandatoryEmptys = true;
                    }
                }

                if (hasMandatoryEmptys)
                {
                    ActionEvent(new FormMessageResult("Заполните все обязательные поля, чтобы продолжить", Severity.Error));
                    return;
                }

                var result = _getters.ToDictionary(kv => kv.Key, kv => kv.Value());
                SubmitResult submit = _request.OnSubmit(result);
                ActionEvent(submit);
                if (submit.Command != ActionCommand.FormMessage)
                {
                    _message.Hide();
                    _message.SetMessage("");
                }
            };

            buttons.Children.Add(cancel);
            buttons.Children.Add(submit);
            Grid.SetRow(buttons, 3);
            outer.Children.Add(buttons);

            return outer;
        }

        private Grid BuildFieldsGrid()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int row = 0;
            int colIndex = 0;

            foreach (var field in _request.Fields)
            {
                int span = field.ColumnSpan <= 0 ? 1 : Math.Min(field.ColumnSpan, 2);

                if (span == 2 && colIndex == 1)
                {
                    row++;
                    colIndex = 0;
                }

                var block = BuildFieldBlock(field);
                EnsureRow(grid, row);

                if (span == 2)
                {
                    Grid.SetRow(block, row);
                    Grid.SetColumn(block, 0);
                    Grid.SetColumnSpan(block, 3);
                    row++;
                    colIndex = 0;
                }
                else
                {
                    int gridCol = colIndex == 0 ? 0 : 2;
                    Grid.SetRow(block, row);
                    Grid.SetColumn(block, gridCol);
                    colIndex = colIndex == 0 ? 1 : 0;
                    if (colIndex == 0) row++;
                }

                grid.Children.Add(block);
            }

            return grid;
        }

        private FrameworkElement BuildFieldBlock(FormField field)
        {
            var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 12) };
            panel.Children.Add(RSText.Label($"{field.Label}{(field.IsMandatory ? "*" : "")}"));

            UIElement input;
            Func<object?> getter;
            Func<bool>? mandatoryCheck = null;

            switch (field.Kind)
            {
                case FieldKind.Password:
                    var pb = RSInput.Password();
                    if (field.Value != null) pb.Password = field.Value.ToString() ?? "";
                    input = pb;
                    getter = () => pb.Password;
                    mandatoryCheck = () => !string.IsNullOrWhiteSpace(pb.Password);
                    break;

                case FieldKind.Int:
                    var tbInt = RSInput.Text(field.Value?.ToString() ?? "");
                    input = tbInt;
                    getter = () => int.TryParse(tbInt.Text, out var n) ? n : 0;
                    mandatoryCheck = () => !string.IsNullOrWhiteSpace(tbInt.Text);
                    break;

                case FieldKind.Date:
                    var tbDate = RSInput.Text(
                        field.Value is DateTime dt
                            ? dt.ToString("yyyy-MM-dd")
                            : field.Value?.ToString() ?? "");
                    input = tbDate;
                    getter = () => DateTime.TryParse(tbDate.Text, out var d) ? d : DateTime.Today;
                    mandatoryCheck = () => !string.IsNullOrWhiteSpace(tbDate.Text);
                    break;

                case FieldKind.Bool:
                    var cb = new CheckBox
                    {
                        IsChecked = field.Value is bool b && b,
                        Margin = new Thickness(0, 6, 0, 6),
                        Foreground = ThemeBrushs.Text,
                        FontFamily = ThemeFonts.Primary
                    };
                    input = cb;
                    getter = () => cb.IsChecked ?? false;
                    break;

                case FieldKind.Combo:
                    var combo = new ComboBox
                    {
                        Height = 38,
                        FontFamily = ThemeFonts.Primary,
                        FontSize = 14,
                        Background = ThemeBrushs.Card,
                        Foreground = ThemeBrushs.Text,
                        BorderBrush = ThemeBrushs.Border,
                        BorderThickness = new Thickness(1),
                        DisplayMemberPath = nameof(OptionItem.Name),
                        SelectedValuePath = nameof(OptionItem.Id)
                    };
                    _combos.Add((combo, field));
                    input = combo;
                    getter = () => (combo.SelectedItem as OptionItem)?.Id;
                    mandatoryCheck = () => combo.SelectedItem != null;
                    break;

                default:
                    var tb = RSInput.Text(field.Value?.ToString() ?? "");
                    input = tb;
                    getter = () => tb.Text.Trim();
                    mandatoryCheck = () => !string.IsNullOrWhiteSpace(tb.Text);
                    break;
            }
            ((FrameworkElement)input).MaxWidth = 180;
            ((FrameworkElement)input).MinWidth = 180;
            _getters[field.Key] = getter;
            if (field.IsMandatory && mandatoryCheck != null)
            {
                Control controlInput = (Control)input;
                controlInput.LostFocus += (_, _) =>
                {
                    if (mandatoryCheck())
                    {
                        controlInput.BorderBrush = ThemeBrushs.Border;
                    }
                    else
                    {
                        controlInput.BorderBrush = ThemeBrushs.Danger;
                    }
                };
                _mandatoryChecks[field.Key] = (controlInput, mandatoryCheck);
            }

            panel.Children.Add(input);
            return panel;
        }

        private static void EnsureRow(Grid grid, int row)
        {
            while (grid.RowDefinitions.Count <= row)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        private async Task PreloadCombosAsync()
        {
            foreach (var (combo, field) in _combos)
            {
                if (field.OptionsLoader is null) continue;

                var options = await field.OptionsLoader();
                combo.ItemsSource = options;

                if (field.Value != null && long.TryParse(field.Value.ToString(), out var id))
                    combo.SelectedValue = id;
            }
        }

        private void ActionEvent(SubmitResult submitResult)
        {
            switch (submitResult)
            {
                case CloseResult:
                {
                    Close();
                    break;
                }
                case CloseConfirmResult closeResult:
                {
                    ActionEvent(new CloseResult());
                    switch (closeResult.MessageResult)
                    {
                        case OverlayMessageResult messageResult:
                        {
                            ActionEvent(messageResult);
                            break;
                        }
                        case ToastConfirmResult messageResult:
                        {
                            ActionEvent(messageResult);
                            break;
                        }
                    }
                    break;
                }
                case OverlayMessageResult messageResult:
                {
                    UITools.Message(messageResult.Text, messageResult.Severity);
                    break;
                }
                case OverlayMessageConfirmResult messageResult:
                {
                    ActionEvent(new OverlayMessageResult(messageResult.Text, messageResult.Severity));
                    ActionEvent(messageResult.CloseResult);
                    break;
                }
                case FormMessageResult messageResult:
                {
                    _message.SetMessage(messageResult.Text, messageResult.Severity);
                    _message.Show();
                    break;
                }
                case ToastResult toastResult:
                {
                    UITools.Toast(toastResult.Text, toastResult.Severity);
                    break;
                }
                case ToastConfirmResult toastResult:
                {
                    ActionEvent(new ToastResult(toastResult.Text, toastResult.Severity));
                    ActionEvent(toastResult.CloseResult);
                    break;
                }
            }
        }
    }
}