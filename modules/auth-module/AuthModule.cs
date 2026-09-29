using RepairService.Application.Store;
using RepairService.Modules.AuthModule.Components;
using RepairService.Modules.AuthModule.Domain;
using RepairService.Modules.AuthModule.Models;
using RepairService.Services.Ui;
using RepairService.Shared.Contracts;
using RepairService.Shared.Events;
using RepairService.Shared.Store;
using RepairService.Ui.Forms;
using RepairService.Ui.Widgets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RepairService.Modules.AuthModule
{
    public class AuthModule
    {
        private readonly MainForm _host;
        private readonly AuthService _service;

        private readonly TextBox _login = RSInput.Text();
        private readonly PasswordBox _password = RSInput.Password();
        private readonly RSMessage _message = new(true);
        private readonly AuthCaptcha _captcha = new();

        public event Action? OnAuthorized;

        public AuthModule(MainForm host)
        {
            _host = host;
            _service = new AuthService(host.Db);
        }

        public FrameworkElement Build()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 60, 40, 40),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 360
            };

            panel.Children.Add(RSText.H1("Вход в систему"));
            panel.Children.Add(RSText.Dim("Сервисный центр по ремонту техники"));
            panel.Children.Add(RSCard.Spacer(24));

            panel.Children.Add(RSText.Label("Логин"));
            panel.Children.Add(_login);
            panel.Children.Add(RSCard.Spacer(12));

            panel.Children.Add(RSText.Label("Пароль"));
            panel.Children.Add(_password);

            panel.Children.Add(RSCard.Spacer(24));

            _captcha.LoadImage(new BitmapImage(new Uri("pack://application:,,,/resources/captcha.png")));
            panel.Children.Add(_captcha);
            panel.Children.Add(RSCard.Spacer(24));

            panel.Children.Add(_message);
            panel.Children.Add(RSCard.Spacer(24));

            var buttons = new Grid();
            buttons.ColumnDefinitions.Add(new ColumnDefinition());
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition());

            var cancel = RSButton.Secondary("Отмена");
            cancel.Click += (_, _) => System.Windows.Application.Current.Shutdown();
            Grid.SetColumn(cancel, 0);

            var ok = RSButton.Primary("Войти");
            ok.Click += (_, _) => DoLogin();
            Grid.SetColumn(ok, 2);

            buttons.Children.Add(cancel);
            buttons.Children.Add(ok);
            panel.Children.Add(buttons);

            var root = new Grid();
            root.Children.Add(RSCard.Card(panel));
            return root;
        }

        private void DoLogin()
        {
            string login = _login.Text.Trim();
            string password = _password.Password;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                _message.SetMessage("Заполните все поля", Severity.Warning);
                _message.Show();
                return;
            }

            AuthUser? user;
            try
            {
                user = _service.Authenticate(login, password, _captcha.IsCompleted, out var error);
                if (user is null)
                {
                    _message.SetMessage(error, Severity.Error);
                    _message.Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                _message.SetMessage($"Нет подключения к БД: {ex.Message}", Severity.Error);
                _message.Show();
                return;
            }

            _message.Hide();
            _message.SetMessage("");

            if (user.ChangePassword)
            {
                ShowChangePasswordDialog(user);
                return;
            }

            CompleteLogin(user);
        }

        private void ShowChangePasswordDialog(AuthUser user)
        {
            UITools.Action("FormChangePassword", new ActionRequest(
                Title: "Смена пароля",
                Fields: new System.Collections.Generic.List<FormField>
                {
                    new() { Key = "current", Label = "Текущий пароль", Kind = FieldKind.Password, IsMandatory = true, ColumnSpan = 2 },
                    new() { Key = "next",    Label = "Новый пароль",   Kind = FieldKind.Password, IsMandatory = true, ColumnSpan = 2 },
                    new() { Key = "confirm", Label = "Подтверждение нового пароля", Kind = FieldKind.Password, IsMandatory = true, ColumnSpan = 2 },
                },
                OnSubmit: data =>
                {
                    var current = data.GetValueOrDefault("current")?.ToString() ?? "";
                    var next = data.GetValueOrDefault("next")?.ToString() ?? "";
                    var confirm = data.GetValueOrDefault("confirm")?.ToString() ?? "";

                    if (current != _password.Password)
                    {
                        return new FormMessageResult("Текущий пароль введён неверно", Severity.Warning);
                    }
                    if (string.IsNullOrWhiteSpace(next) || next.Length < 6)
                    {
                        return new FormMessageResult("Новый пароль слишком короткий", Severity.Warning);
                    }
                    if (next != confirm)
                    {
                        return new FormMessageResult("Новый пароль и подтверждение не совпадают", Severity.Warning);
                    }

                    try
                    {
                        _service.ChangePassword(user.Id, next);
                        user.ChangePassword = false;
                        CompleteLogin(user);
                        return new ToastConfirmResult("Пароль успешно изменён", Severity.Success, new CloseResult());
                    }
                    catch (Exception ex)
                    {
                        return new ToastResult($"Ошибка смены пароля: {ex.Message}", Severity.Error);
                    }
                }));
        }

        private void CompleteLogin(AuthUser user)
        {
            try
            {
                Store.Set(StoreKeys.CurrentUser, _service.GetUserData(user.Id));
                Store.Set(StoreKeys.CurrentUserClaims, _service.GetClaims(user.RoleId));
            }
            catch (Exception ex)
            {
                _message.SetMessage($"Нет подключения к БД: {ex.Message}", Severity.Error);
                _message.Show();
                return;
            }
            UITools.Toast($"Добро пожаловать, {user.FirstName}\nВы успешно авторизовались", Severity.Success);
            OnAuthorized?.Invoke();
        }
    }
}