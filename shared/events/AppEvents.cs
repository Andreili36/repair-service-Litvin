using System;
using System.Windows.Input;
using RepairService.Services.Ui;
using RepairService.Shared.Contracts;

namespace RepairService.Shared.Events
{
    public record ShowToastEvent(string Text, Severity Severity);

    public record ShowMessageEvent(string id, string Text, Severity Severity);

    public record ShowConfirmEvent(string id, string Text, Action<bool> OnResult);

    public record ShowActionEvent(string id, ActionRequest Request);

    public record KeyDownEvent( Key Key );
}