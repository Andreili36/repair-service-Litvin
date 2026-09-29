using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using RepairService.Application.Theme;

namespace RepairService.Ui.Widgets
{
    public static class RSTable
    {
        public static DataGrid Table() => new()
        {
            AutoGenerateColumns = true,
            CanUserAddRows = false,
            CanUserDeleteRows = false,
            IsReadOnly = true,
            HeadersVisibility = DataGridHeadersVisibility.Column,
            GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
            Background = ThemeBrushs.Card,
            RowBackground = ThemeBrushs.Card,
            AlternatingRowBackground = ThemeBrushs.Surface,
            Foreground = ThemeBrushs.Text,
            BorderBrush = ThemeBrushs.Border,
            BorderThickness = new Thickness(1),
            FontFamily = ThemeFonts.Primary,
            FontSize = 13,
            RowHeight = 34,
            HorizontalGridLinesBrush = ThemeBrushs.Border,
            SelectionMode = DataGridSelectionMode.Single,
            ColumnHeaderStyle = ColumnHeaderStyle(),
            RowHeaderStyle = RowHeaderStyle(),
            ColumnHeaderHeight = 40
        };

        private static Style ColumnHeaderStyle()
        {
            var style = new Style(typeof(DataGridColumnHeader));

            style.Setters.Add(new Setter(Control.ForegroundProperty, ThemeBrushs.Text));
            style.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.SemiBold));
            style.Setters.Add(new Setter(Control.FontSizeProperty, 13d));
            style.Setters.Add(new Setter(Control.FontFamilyProperty, ThemeFonts.Primary));
            style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(12, 0, 12, 0)));
            style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Left));

            var border = new FrameworkElementFactory(typeof(Border));
            border.Name = "border";
            border.SetValue(Border.BackgroundProperty, ThemeBrushs.Surface);
            border.SetValue(Border.BorderBrushProperty, ThemeBrushs.Border);
            border.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 1, 1));
            border.SetValue(Border.PaddingProperty, new Thickness(12, 0, 12, 0));

            var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
            presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            border.AppendChild(presenter);

            var template = new ControlTemplate(typeof(DataGridColumnHeader)) { VisualTree = border };

            var hover = new Trigger
            {
                Property = DataGridColumnHeader.IsMouseOverProperty,
                Value = true
            };
            hover.Setters.Add(new Setter(Border.BackgroundProperty, ThemeBrushs.Card, "border"));
            template.Triggers.Add(hover);

            style.Setters.Add(new Setter(Control.TemplateProperty, template));
            return style;
        }

        private static Style RowHeaderStyle()
        {
            var style = new Style(typeof(DataGridRowHeader));
            style.Setters.Add(new Setter(Control.BackgroundProperty, ThemeBrushs.Surface));
            style.Setters.Add(new Setter(Control.ForegroundProperty, ThemeBrushs.Text));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, ThemeBrushs.Border));
            return style;
        }
    }
}