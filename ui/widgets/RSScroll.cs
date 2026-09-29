using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace RepairService.Ui.Widgets
{
    public static class RSScroll
    {
        public static ScrollViewer Vertical(System.Windows.FrameworkElement content)
        {
            var viewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = content
            };
            viewer.Resources.Add(typeof(ScrollBar), RSScrollBar.Thin());
            return viewer;
        }
    }
}