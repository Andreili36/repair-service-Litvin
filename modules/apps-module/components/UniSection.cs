using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RepairService.Services.Ui;
using RepairService.Shared.Models;
using RepairService.Shared.Contracts;
using RepairService.Ui.Widgets;

namespace RepairService.Modules.AppsModule.Components
{
    public class UniSection
    {
        private readonly AppsModule _host;
        private readonly SectionDescriptor _descriptor;
        private readonly UserClaim _claim;
        private readonly DataGrid _grid;

        public UniSection(AppsModule host, SectionDescriptor descriptor, UserClaim claim)
        {
            _host = host;
            _descriptor = descriptor;
            _claim = claim;
            _grid = BuildGrid();
        }

        public FrameworkElement Build()
        {
            var root = new Grid();
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var header = new Grid();
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            header.Children.Add(RSText.H1(_descriptor.Title));

            var actions = new StackPanel { Orientation = Orientation.Horizontal };

            if (_claim.CanCreate && _descriptor.FormFields.Count > 0)
            {
                var add = RSButton.Primary("Добавить");
                add.Click += (_, _) => OnAdd();
                actions.Children.Add(add);
                actions.Children.Add(new Border { Width = 8 });
            }

            if (_claim.CanEdit && _descriptor.FormFields.Count > 0)
            {
                var edit = RSButton.Secondary("Изменить");
                edit.Click += (_, _) => OnEdit();
                actions.Children.Add(edit);
                actions.Children.Add(new Border { Width = 8 });
            }

            if (_claim.CanDelete)
            {
                var del = RSButton.Danger("Удалить");
                del.Click += (_, _) => OnDelete();
                actions.Children.Add(del);
                actions.Children.Add(new Border { Width = 8 });
            }

            var refresh = RSButton.Secondary("Обновить");
            refresh.Click += (_, _) => Load();
            actions.Children.Add(refresh);

            Grid.SetColumn(actions, 1);
            header.Children.Add(actions);
            Grid.SetRow(header, 0);
            root.Children.Add(header);

            Load();
            Grid.SetRow(_grid, 1);
            root.Children.Add(_grid);

            return root;
        }

        private DataGrid BuildGrid()
        {
            var grid = RSTable.Table();
            grid.AutoGenerateColumns = false;

            foreach (var col in _descriptor.Columns)
            {
                DataGridColumn column = col.Type switch
                {
                    ColumnType.Bool => new DataGridCheckBoxColumn
                    {
                        Header = col.Header,
                        Binding = new Binding(col.Key) { Mode = BindingMode.OneWay },
                        IsReadOnly = true
                    },
                    _ => new DataGridTextColumn
                    {
                        Header = col.Header,
                        Binding = new Binding(col.Key) { Mode = BindingMode.OneWay },
                    }
                };
                grid.Columns.Add(column);
            }

            return grid;
        }

        private void Load()
        {
            try
            {
                _grid.ItemsSource = _descriptor.LoadData(_host.Db);
            }
            catch (Exception ex)
            {
                UITools.Message("Ошибка загрузки: " + ex.Message, Severity.Error);
                _grid.ItemsSource = null;
            }
        }

        private void OnAdd()
        {
            var fields = CloneFields(_descriptor.FormFields, null);
            UITools.Action($"FormAdd{_descriptor.ObjectCode}", new ActionRequest(
                Title: $"Добавить: {_descriptor.Title}",
                Fields: fields,
                OnSubmit: data =>
                {
                    try
                    {
                        _descriptor.OnCreate(_host.Db, data);
                        Load();
                        return new ToastConfirmResult($"{_descriptor.Title}: запись добавлена", Severity.Success, new CloseResult());
                    }
                    catch (Exception ex)
                    {
                        return new ToastResult($"Ошибка создания: {ex.Message}", Severity.Error);
                    }
                }));
        }

        private void OnEdit()
        {
            if (_grid.SelectedItem is null)
            {
                UITools.Message("Выберите запись.", Severity.Error);
                return;
            }

            var row = _grid.SelectedItem;
            var fields = CloneFields(_descriptor.FormFields, row);

            UITools.Action($"FormChange{_descriptor.ObjectCode}", new ActionRequest(
                Title: $"Редактировать: {_descriptor.Title}",
                Fields: fields,
                OnSubmit: data =>
                {
                    try
                    {
                        _descriptor.OnUpdate(_host.Db, row, data);
                        Load();
                        return new ToastConfirmResult($"{_descriptor.Title}: запись обновлена", Severity.Success, new CloseResult());
                    }
                    catch (Exception ex)
                    {
                        return new ToastResult($"Ошибка обновления: {ex.Message}", Severity.Error);
                    }
                }));
        }

        private void OnDelete()
        {
            if (_grid.SelectedItem is null)
            {
                UITools.Message("Выберите запись.", Severity.Error);
                return;
            }

            var row = _grid.SelectedItem;
            UITools.Confirm($"ConfirmDelete{_descriptor.ObjectCode}", $"Удалить запись из «{_descriptor.Title}»?", ok =>
            {
                if (!ok) return;
                try
                {
                    _descriptor.OnDelete(_host.Db, row);
                    UITools.Message($"{_descriptor.Title}: запись удалена", Severity.Success);
                    Load();
                }
                catch (Exception ex)
                {
                    UITools.Message($"Ошибка удаления: {ex.Message}", Severity.Error);
                }
            });
        }

        private static List<FormField> CloneFields(List<FormField> source, object? row)
        {
            var result = new List<FormField>();
            foreach (var f in source)
            {
                result.Add(new FormField
                {
                    Key = f.Key,
                    Label = f.Label,
                    Kind = f.Kind,
                    OptionsLoader = f.OptionsLoader,
                    Value = row is null
                        ? f.Value
                        : GetValue(row, f.Key)
                });
            }
            return result;
        }

        private static object? GetValue(object row, string key)
        {
            if (row is IDictionary<string, object?> dict)
                return dict.GetValueOrNull(key);

            var prop = row.GetType().GetProperty(key);
            return prop?.GetValue(row);
        }
    }
}