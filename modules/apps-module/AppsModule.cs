using System.Windows;
using System.Windows.Controls;
using RepairService.Application.Store;
using RepairService.Application.Theme;
using RepairService.Db;
using RepairService.Modules.AppsModule.Catalogs;
using RepairService.Modules.AppsModule.Components;
using RepairService.Services.Ui;
using RepairService.Shared.Models;
using RepairService.Shared.Store;
using RepairService.Ui.Forms;

namespace RepairService.Modules.AppsModule
{
    public class AppsModule
    {
        private readonly MainForm _host;
        private readonly List<CatalogEntry> _availableCatalogs;
        private readonly List<UserClaim> _claims;
        private readonly ContentControl _contentHost = new();
        private CatalogEntry? _currentCatalog;

        public DbUnibridge Db => _host.Db;

        public event Action? OnLogout;
        public event Action? OnFatalError;

        public AppsModule(MainForm host)
        {
            _host = host;

            var user = Store.Get<User>(StoreKeys.CurrentUser);
            _claims = Store.Get<List<UserClaim>>(StoreKeys.CurrentUserClaims) ?? new();

            try
            {
                new CatalogSyncService(Db).Sync(AppsCatalogRegistry.All);
            }
            catch (Exception ex)
            {
                UITools.Message("Ошибка структуры БД модуля: " + ex.Message, Severity.Error);
                _availableCatalogs = new();
                OnFatalError?.Invoke();
                return;
            }

            var allCatalogs = new List<CatalogEntry>();
            allCatalogs.AddRange(AppCatalogs.Descriptors());
            allCatalogs.AddRange(AppsCatalogRegistry.Descriptors());

            _availableCatalogs = allCatalogs
                .Where(c => _claims.Any(cl => cl.ObjectCode == c.ObjectCode && cl.CanView))
                .ToList();
        }

        public FrameworkElement Build()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(240) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var menuBorder = new Border
            {
                Background = ThemeBrushs.Surface,
                Padding = new Thickness(16, 24, 16, 24)
            };
            menuBorder.Child = new AppsMenu(this, _availableCatalogs).Build();
            Grid.SetColumn(menuBorder, 0);
            grid.Children.Add(menuBorder);

            var contentBorder = new Border
            {
                Background = ThemeBrushs.Bg,
                Padding = new Thickness(24)
            };
            contentBorder.Child = _contentHost;
            Grid.SetColumn(contentBorder, 1);
            grid.Children.Add(contentBorder);

            if (_availableCatalogs.Count > 0)
                NavigateTo(_availableCatalogs[0]);

            return grid;
        }

        public void NavigateTo(CatalogEntry catalog)
        {
            _currentCatalog = catalog;
            var claim = _claims.First(c => c.ObjectCode == catalog.ObjectCode);
            var descriptor = catalog.DescriptorFactory();
            var section = new UniSection(this, descriptor, claim);
            _contentHost.Content = section.Build();
        }

        public void RefreshCurrent()
        {
            if (_currentCatalog != null)
                NavigateTo(_currentCatalog);
        }

        public void RaiseLogout() => OnLogout?.Invoke();
    }
}