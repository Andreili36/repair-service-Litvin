using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RepairService.Application.Store;
using RepairService.Application.Theme;
using RepairService.Db;
using RepairService.Modules.AppsModule;
using RepairService.Modules.AuthModule;
using RepairService.Shared.Store;
using RepairService.Ui.Hosts;

namespace RepairService.Ui.Forms
{
    public class MainForm : Window
    {
        private readonly OverlayHost _overlay = new();
        private readonly ToastHost _toasts = new();

        private AuthModule? _auth;
        private AppsModule? _apps;

        public DbUnibridge Db { get; }

        public MainForm(DbUnibridge db)
        {
            Db = db;

            Title = "Сервисный центр по ремонту техники";
            Width = 1200;
            Height = 740;
            MinWidth = 900;
            MinHeight = 600;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = ThemeBrushs.Bg;
            Foreground = ThemeBrushs.Text;
            FontFamily = ThemeFonts.Primary;

            var root = new Grid();
            root.Children.Add(_overlay);
            root.Children.Add(_toasts);
            Content = root;

            ShowAuth();
        }

        private void ShowAuth()
        {
            Store.Clear(StoreKeys.CurrentUser);
            Store.Clear(StoreKeys.CurrentUserClaims);

            _auth = new AuthModule(this);
            _auth.OnAuthorized += ShowApps;
            _overlay.Content = _auth.Build();
        }

        private void ShowApps()
        {
            _apps = new AppsModule(this);
            _apps.OnLogout += ShowAuth;
            _apps.OnFatalError += ShowAuth;
            _overlay.Content = _apps.Build();
        }
    }
}