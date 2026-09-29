using System;
using System.Windows;
using RepairService.Application.Catalogs;
using RepairService.Db;

namespace RepairService
{
    public static class AppLauncher
    {
        [STAThread]
        public static void Main()
        {
            DbUnibridge db;
            try
            {
                var config = DbConfig.Load();
                db = new DbUnibridge(config);
                db.TestConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось подключиться к базе данных.\n\n" + ex.Message,
                    "Ошибка подключения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                new CatalogSyncService(db).Sync(AppCatalogRegistry.All);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось синхронизировать структуру базы данных.\n\n" + ex.Message,
                    "Ошибка структуры БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var app = new System.Windows.Application { ShutdownMode = ShutdownMode.OnMainWindowClose };
            var main = new Ui.Forms.MainForm(db);
            app.MainWindow = main;
            app.Run(main);
        }
    }
}