using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RepairService.Application.Store;
using RepairService.Application.Theme;
using RepairService.Modules.AppsModule.Catalogs;
using RepairService.Shared.Models;
using RepairService.Shared.Store;
using RepairService.Ui.Widgets;

namespace RepairService.Modules.AppsModule.Components
{
    public class AppsMenu
    {
        private readonly AppsModule _host;
        private readonly List<CatalogEntry> _catalogs;

        public AppsMenu(AppsModule host, List<CatalogEntry> catalogs)
        {
            _host = host;
            _catalogs = catalogs;
        }

        public FrameworkElement Build()
        {
            var panel = new StackPanel();
            var user = Store.Get<User>(StoreKeys.CurrentUser);

            panel.Children.Add(new TextBlock
            {
                Text = user?.FullName ?? "",
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                Foreground = ThemeBrushs.Text,
                FontFamily = ThemeFonts.Primary,
                Margin = new Thickness(8, 0, 0, 2)
            });

            panel.Children.Add(new TextBlock
            {
                Text = user?.RoleName ?? "",
                FontSize = 12,
                Foreground = ThemeBrushs.TextDim,
                FontFamily = ThemeFonts.Primary,
                Margin = new Thickness(8, 0, 0, 24)
            });

            foreach (var catalog in _catalogs)
            {
                var cat = catalog;
                var btn = RSButton.Menu(cat.Title);
                btn.Click += (_, _) => _host.NavigateTo(cat);
                panel.Children.Add(btn);
            }

            var logout = RSButton.Menu("Выход");
            logout.Foreground = ThemeBrushs.Danger;
            logout.Click += (_, _) => _host.RaiseLogout();
            panel.Children.Add(logout);

            var scroll = RSScroll.Vertical(panel);
            scroll.Padding = new Thickness(0, 0, 4, 0);
            return scroll;
        }
    }
}